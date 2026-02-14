#ifndef PWM_SETTINGS_H
#define PWM_SETTINGS_H

#pragma pack(push, 1)
struct PWMSettings
{
    uint32_t offset_us;
    uint32_t on_duration_us;
    uint32_t off_duration_us;
    uint32_t cycles;
    uint8_t invert;
};
#pragma pack(pop)


#endif // PWM_SETTINGS_H
