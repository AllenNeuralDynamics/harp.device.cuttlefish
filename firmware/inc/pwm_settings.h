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
#pragma pack(pop)


#endif // PWM_SETTINGS_H
