#ifndef CORE1_MAIN_H
#define CORE1_MAIN_H
#include <pico/stdlib.h>
#include <config.h>
#include <pwm_task.h>
#include <pwm_scheduler.h>
#include <schedule_ctrl_queues.h>
#include <hardware/timer.h>
#if defined(DEBUG) || defined(PROFILE_CPU)
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

#if defined(PROFILE_CPU)
#define SYST_CSR (*(volatile uint32_t*)(PPB_BASE + 0xe010))
#define SYST_CVR (*(volatile uint32_t*)(PPB_BASE + 0xe018))

#define PRINT_LOOP_INTERVAL_US (16666) // ~60Hz

uint64_t time_us_64_unsafe();
#endif

enum core1_state_t: uint32_t
{
    RESET,
    READY,
    RUNNING,
};

extern core1_state_t state;
extern bool schedule_failed;
extern uint8_t active_pwm_pins;

extern PWMScheduler scheduler;

void core1_main();

#endif // CORE1_MAIN_H
