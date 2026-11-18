#ifndef SCHEDULE_CTRL_QUEUES_H
#define SCHEDULE_CTRL_QUEUES_H
#include <pico/util/queue.h>
#include <pwm_settings.h>
#ifdef DEBUG
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

enum class pwm_ctrl_msg_t
{
    START,
    STOP
};

enum class core1_state_t: uint32_t
{
    RESET,
    READY,
    RUNNING
};


/**
 * \brief Container to unpack pwm task specs from a received harp message.
 * \details this container is packed because it will be received packed and
 *  deserialized from an external device (connected PC).
 */
#pragma pack(push, 1)
struct pwm_specs_core_msg_t
{
    size_t pin; // Limit 1 pin per PWMTask.
    pwm_settings_t specs;
    bool analog; // true if this pin should be driven by an AnalogPWMTask.
    float analog_frequency_hz; // Only used if analog is true.
    float analog_duty_cycle; // Only used if analog is true.

    // Custom constructor that works off of references.
    pwm_specs_core_msg_t(size_t& pin, pwm_settings_t& specs)
    :pin(pin), specs(specs), analog{false}, analog_frequency_hz{0},
     analog_duty_cycle{0} {}

    pwm_specs_core_msg_t(size_t& pin, analog_pwm_settings_t& analog_specs)
    :pin(pin), specs(analog_specs.pwm), analog{true},
     analog_frequency_hz{analog_specs.frequency_hz},
     analog_duty_cycle{analog_specs.duty_cycle} {}

    // Enable default constructor.
    pwm_specs_core_msg_t() = default;
};
#pragma pack(pop)


/**
 * \brief For core1 to communicate timstamped state changes to core0.
 *  Necessary for core0 to dispatch Harp messages.
 */
struct core1_next_state_msg_t
{
    core1_state_t next_state;
    uint64_t timestamp_us;
};

enum class schedule_error_t: uint8_t
{
    NONE = 0,
    MISSED_DEADLINE = 1
};

/**
 * \brief For core1 to report timestamped schedule errors to core0.
 */
struct schedule_error_msg_t
{
    schedule_error_t error;
    uint64_t timestamp_us;
};

extern queue_t pwm_settings_queue;
extern queue_t core1_ctrl_queue;
extern queue_t core1_next_state_queue;
extern queue_t schedule_error_queue;

#endif // SCHEDULE_CTRL_QUEUES_H
