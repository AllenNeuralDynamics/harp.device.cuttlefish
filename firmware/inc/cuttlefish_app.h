#ifndef CUTTLEFISH_APP_H
#define CUTTLEFISH_APP_H
#include <pico/stdlib.h>
#include <cstring>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <harp_synchronizer.h>
#include <etl/vector.h>
//#include <pwm_scheduler.h>
//#include <pwm_task.h>
#include <pwm_settings.h>
#include <edge_event_queue.h>
#include <schedule_ctrl_queues.h>
#include <core1_main.h>
#include <pico/multicore.h>
#include <hardware/irq.h>
#include <hardware/gpio.h>
#ifdef DEBUG
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

// Setup for Harp App
inline constexpr size_t reg_count = 6;
inline constexpr uint8_t RISING_EDGE_EVENTS_ADDRESS = APP_REG_START_ADDRESS + 4;

extern uint8_t pwm_task_mask;
extern PWMScheduler pwm_schedule;
extern RegSpecs app_reg_specs[reg_count];
extern RegFnPair reg_handler_fns[reg_count];
extern HarpCApp& app;

#pragma pack(push, 1)
struct app_regs_t
{
    volatile uint8_t port_dir; // 1 = output; 0 = input.
    volatile uint8_t port_state; // current gpio state. Readable and writeable.
    volatile uint8_t port_set;
    volatile uint8_t port_clear;

    volatile uint8_t rising_edge_events;
    volatile uint8_t falling_edge_events;

    volatile PWMSettings pwm_settings[NUM_GPIOS];
};
#pragma pack(pop)

extern app_regs_t app_regs;


void reset_schedule();

/**
 * \brief declare pins inputs or outputs.
 * \warning this status can be overwritten if a pin is later assigned to a PWMTask
 */
void write_port_dir(msg_t& msg);

/**
 * \brief read the 8-channel port simultaneousy
 */
void read_port_state(uint8_t reg_address);

/**
 * \brief write to the 8-channel port as a group. (Values for pins specified
 *  as inputs will be ignored.)
 */
void write_port_state(msg_t& msg);

void write_port_set(msg_t& msg);
void write_port_clear(msg_t& msg);

void read_rising_edge_events(uint8_t msg_address);
void write_rising_edge_events(msg_t& msg);

void read_falling_edge_events(uint8_t msg_address);
void write_falling_edge_events(msg_t& msg);

/**
 * \brief a single callback to handle all GPIO pin change events including
 *  simultaneous events.
 * \warning this function replaces the pico-sdk's gpio_default_irq_handler().
 *  That means we can't use gpio_set_irq_enabled_with_callback() to assign
 *  pin change interrupts to single GPIO pin changes.
 */
void handle_edge_event_callback(void);

/**
 * \brief update the app state. Called in a loop.
 */
void update_app_state();

/**
 * \brief reset the app.
 */
void reset_app();

#endif // CUTTLEFISH_APP_H
