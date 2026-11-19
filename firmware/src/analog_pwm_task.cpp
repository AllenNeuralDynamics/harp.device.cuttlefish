#include <analog_pwm_task.h>
#include <hardware/clocks.h>
#include <cmath>

bool AnalogPWMTask::valid_settings(float frequency_hz, float duty_cycle)
{
    // Note: comparisons with NaN are false.
    return (frequency_hz >= MIN_FREQUENCY_HZ)
           && (frequency_hz <= MAX_FREQUENCY_HZ)
           && (duty_cycle >= 0.0f) && (duty_cycle <= 1.0f);
}

bool AnalogPWMTask::compute_slice_config(float frequency_hz,
                                         slice_config_t& config)
{
    if (!((frequency_hz >= MIN_FREQUENCY_HZ)
          && (frequency_hz <= MAX_FREQUENCY_HZ)))
        return false;
    // Total sys clock ticks per period.
    double ticks = double(clock_get_hz(clk_sys)) / double(frequency_hz);
    // Smallest integer divider that fits the period into a 16-bit counter.
    // Limit the period to 65535 ticks so that a 100% duty cycle is reachable
    // with a 16-bit compare level.
    uint32_t clkdiv = uint32_t(std::ceil(ticks / 65535.0));
    if (clkdiv < 1)
        clkdiv = 1;
    if (clkdiv > 255)
        return false;
    uint32_t top = uint32_t(std::lround(ticks / clkdiv));
    if (top < 2)
        return false;
    if (top > 65535)
        top = 65535;
    config.clkdiv = uint8_t(clkdiv);
    config.wrap = uint16_t(top - 1);
    return true;
}

AnalogPWMTask::AnalogPWMTask(uint32_t t_delay_us, uint32_t t_on_us,
                             uint32_t t_period_us, uint32_t pin_mask,
                             uint32_t count, bool invert,
                             float frequency_hz, float duty_cycle)
: PWMTask(t_delay_us, t_on_us, t_period_us, pin_mask, count, invert)
{
    apply_settings(t_delay_us, t_on_us, t_period_us, pin_mask, count, invert,
                   frequency_hz, duty_cycle);
}

void AnalogPWMTask::apply_settings(uint32_t t_delay_us, uint32_t t_on_us,
                                   uint32_t t_period_us, uint32_t pin_mask,
                                   uint32_t count, bool invert,
                                   float frequency_hz, float duty_cycle)
{
    // Setup timing and GPIO as an output (driven LOW) first.
    PWMTask::apply_settings(t_delay_us, t_on_us, t_period_us, pin_mask, count,
                            invert);
    pin_ = __builtin_ctz(pin_mask);
    frequency_hz_ = frequency_hz;
    duty_cycle_ = duty_cycle;
    slice_config_t config;
    if (!compute_slice_config(frequency_hz, config)) // Should be pre-validated.
        config = {255, 0xFFFF};
    float clamped_duty = std::fmin(std::fmax(duty_cycle, 0.0f), 1.0f);
    uint32_t top = uint32_t(config.wrap) + 1;
    uint32_t level = uint32_t(std::lround(clamped_duty * top)); // <= 0xFFFF.
    uint slice = pwm_gpio_to_slice_num(pin_);
    pwm_set_clkdiv_int_frac(slice, config.clkdiv, 0);
    pwm_set_wrap(slice, config.wrap);
    pwm_set_chan_level(slice, pwm_gpio_to_channel(pin_), level);
    pwm_set_enabled(slice, true);
    // Hand the pin over to the PWM peripheral while holding the idle level.
    // Note: gpio_set_function() clears OUTOVER, so write FUNCSEL and OUTOVER
    //  together (pad settings were already configured by gpio_init).
    uint32_t idle_outover = invert_? GPIO_OVERRIDE_HIGH: GPIO_OVERRIDE_LOW;
    hw_write_masked(&io_bank0_hw->io[pin_].ctrl,
                    (GPIO_FUNC_PWM << IO_BANK0_GPIO0_CTRL_FUNCSEL_LSB)
                    | (idle_outover << IO_BANK0_GPIO0_CTRL_OUTOVER_LSB),
                    IO_BANK0_GPIO0_CTRL_FUNCSEL_BITS
                    | IO_BANK0_GPIO0_CTRL_OUTOVER_BITS);
}

void AnalogPWMTask::release()
{
    pwm_set_chan_level(pwm_gpio_to_slice_num(pin_), pwm_gpio_to_channel(pin_),
                       0);
    gpio_set_function(pin_, GPIO_FUNC_SIO);
    gpio_set_outover(pin_, GPIO_OVERRIDE_NORMAL);
    PWMTask::release();
}
