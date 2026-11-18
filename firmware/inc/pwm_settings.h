#ifndef PWM_SETTINGS_H
#define PWM_SETTINGS_H
#include <cstdint>

#pragma pack(push, 1)
struct pwm_settings_t
{
    uint32_t offset_us;
    uint32_t on_duration_us;
    uint32_t off_duration_us;
    uint32_t cycles;
    uint8_t invert;

    uint32_t period_us() const
    {return on_duration_us + off_duration_us;}
};

/**
 * \brief PWM settings plus the high-frequency output (produced while the
 *  nominal output is HIGH) settings.
 */
struct analog_pwm_settings_t
{
    pwm_settings_t pwm;
    float frequency_hz; /// high-frequency PWM output frequency.
    float duty_cycle;   /// high-frequency PWM output duty cycle [0.0, 1.0].
};
#pragma pack(pop)

static_assert(sizeof(pwm_settings_t) == 17);
static_assert(sizeof(analog_pwm_settings_t) == 25);

#endif // PWM_SETTINGS_H
