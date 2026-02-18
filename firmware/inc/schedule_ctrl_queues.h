#ifndef SCHEDULE_CTRL_QUEUES_H
#define SCHEDULE_CTRL_QUEUES_H
#include <pico/util/queue.h>
#include <pwm_settings.h>
#ifdef DEBUG
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

// Container to unpack pwm task specs from a received harp message.
#pragma pack(push, 1)
struct pwm_specs_core_msg_t
{
    uint32_t pin_mask;
    pwm_settings_t specs;
};
#pragma pack(pop)

extern queue_t pwm_task_setup_queue;
extern queue_t cmd_signal_queue;
extern queue_t schedule_error_signal_queue;

#endif // SCHEDULE_CTRL_QUEUES_H
