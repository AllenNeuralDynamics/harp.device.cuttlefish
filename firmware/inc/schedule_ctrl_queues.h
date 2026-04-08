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

// Container to unpack pwm task specs from a received harp message.
#pragma pack(push, 1)
struct pwm_specs_core_msg_t
{
    size_t pin; // Limit 1 pin per PWMTask.
    pwm_settings_t specs;

    // Custom constructor that works off of references.
    pwm_specs_core_msg_t(size_t& pin, pwm_settings_t& specs)
    :pin(pin), specs(specs) {}

    // Enable default constructor.
    pwm_specs_core_msg_t() = default;
};
#pragma pack(pop)

struct core1_state
{
    uint32_t pwm_state;
    bool scheduler_error;
    // TODO: some sort of schedule-failed signal.

    bool schedule_is_running()
    {return pwm_state > 0;}
};

extern queue_t pwm_settings_queue;
extern queue_t core1_ctrl_queue;
extern queue_t core1_state_queue;
extern queue_t schedule_error_queue;

#endif // SCHEDULE_CTRL_QUEUES_H
