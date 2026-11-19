#ifndef ANALOG_PWM_TASK_H
#define ANALOG_PWM_TASK_H
#include <stdint.h>
#include <pwm_task.h>
#include <pico/stdlib.h>
#include <hardware/gpio.h>
#include <hardware/pwm.h>
#include <hardware/structs/io_bank0.h>

/**
 * \brief A PWMTask that, while the nominal (low-frequency) output is HIGH,
 *  produces a high-frequency PWM output via the hardware PWM peripheral
 *  (a "virtual analog" output). While the nominal output is LOW, the pin is
 *  held at its (possibly inverted) idle level.
 * \details The nominal waveform is gated onto the pin via the GPIO output
 *  override, so the hardware PWM peripheral runs freely and state changes
 *  take effect immediately. The hardware PWM counter is restarted when the
 *  nominal output switches HIGH (unless the other pin sharing the same
 *  hardware PWM slice is already producing an output) such that each burst
 *  begins at the start of a high-frequency period.
 * \warning both pins of a PWM slice (GPIO 2n and 2n+1) share the same
 *  frequency. Tasks on a shared slice must use the same frequency.
 */
class AnalogPWMTask: public PWMTask
{
public:
    static constexpr float MIN_FREQUENCY_HZ = 10.0f;
    static constexpr float MAX_FREQUENCY_HZ = 1'000'000.0f;

/**
 * \brief hardware PWM slice settings that produce a desired frequency.
 */
    struct slice_config_t
    {
        uint8_t clkdiv;  /// integer clock divider (1-255).
        uint16_t wrap;   /// counter TOP value. Period = (wrap + 1) ticks.

        bool operator==(const slice_config_t& other) const = default;
    };

/**
 * \brief true if the frequency and duty cycle are within range.
 */
    static bool valid_settings(float frequency_hz, float duty_cycle);

/**
 * \brief compute the hardware PWM slice settings for the specified frequency.
 *  Integer clock dividers are used to avoid the jitter of fractional dividers.
 * \return false if the frequency is out of range.
 */
    static bool compute_slice_config(float frequency_hz, slice_config_t& config);

    AnalogPWMTask(uint32_t t_delay_us, uint32_t t_on_us, uint32_t t_period_us,
                  uint32_t pin_mask, uint32_t count, bool invert,
                  float frequency_hz, float duty_cycle);

/**
 * \brief (re)apply all settings, including GPIO and hardware PWM setup.
 * \note pin_mask must specify a single pin.
 */
    void apply_settings(uint32_t t_delay_us, uint32_t t_on_us,
                        uint32_t t_period_us, uint32_t pin_mask,
                        uint32_t count, bool invert,
                        float frequency_hz, float duty_cycle);

/**
 * \brief stop output (drive the idle level).
 */
    inline void stop()
    {set_output(pin_, false, invert_);}

/**
 * \brief release the pin from the hardware PWM peripheral and configure it as
 *  an input.
 */
    void release();

/**
 * \brief gate the high-frequency output on the specified pin on (or off).
 * \details Inversion is applied via the GPIO output override. When off, the
 *  pin is held at its idle level (LOW, or HIGH if inverted).
 */
    static inline void set_output(uint32_t pin, bool on, bool invert)
    {
        uint32_t outover = on? (invert? GPIO_OVERRIDE_INVERT: GPIO_OVERRIDE_NORMAL)
                             : (invert? GPIO_OVERRIDE_HIGH: GPIO_OVERRIDE_LOW);
        hw_write_masked(&io_bank0_hw->io[pin].ctrl,
                        outover << IO_BANK0_GPIO0_CTRL_OUTOVER_LSB,
                        IO_BANK0_GPIO0_CTRL_OUTOVER_BITS);
    }

/**
 * \brief restart the hardware PWM period of the slice for the specified pin.
 */
    static inline void restart_period(uint32_t pin)
    {pwm_hw->slice[pwm_gpio_to_slice_num(pin)].ctr = 0;}

private:
    friend class PWMScheduler;
    friend void sync_schedule();

    uint32_t pin_;
    float frequency_hz_;
    float duty_cycle_;
};
#endif // ANALOG_PWM_TASK_H
