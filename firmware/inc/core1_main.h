#ifndef CORE1_MAIN_H
#define CORE1_MAIN_H
#include <pico/stdlib.h>
#include <config.h>
#include <pwm_task.h>
#include <pwm_scheduler.h>
#include <schedule_ctrl_queues.h>
#if defined(DEBUG) || defined(PROFILE_CPU)
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

extern core1_state_t state;
extern bool schedule_failed;

extern PWMScheduler scheduler;

void core1_main();

#endif // CORE1_MAIN_H
