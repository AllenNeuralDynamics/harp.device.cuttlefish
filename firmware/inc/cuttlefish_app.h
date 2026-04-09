#ifndef CUTTLEFISH_APP_H
#define CUTTLEFISH_APP_H
#include <pico/stdlib.h>
#include <cstring>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <etl/vector.h>
#include <pwm_settings.h>
#include <edge_event_queue.h>
#include <schedule_ctrl_queues.h>
#include <core1_main.h>
#include <pico/multicore.h>
#include <hardware/irq.h>
#include <hardware/gpio.h>
#include <hardware/timer.h>
#ifdef DEBUG
    #include <stdio.h>
    #include <cstdio> // for printf
#endif

using enum reg_type_t;
using Harp = HarpCore; // make an alias.

// Setup for Harp App
extern const size_t APP_REG_COUNT;

inline constexpr uint8_t RISING_EDGE_EVENTS_ADDRESS =
    Harp::APP_REG_START_ADDRESS + 5;
inline constexpr uint8_t FALLING_EDGE_EVENTS_ADDRESS =
    Harp::APP_REG_START_ADDRESS + 7;
inline constexpr uint8_t PWM_STATE_ADDRESS =
    Harp::APP_REG_START_ADDRESS + 8;

extern uint8_t pwm_task_mask;
extern PWMScheduler pwm_schedule;
extern RegSpec app_reg_specs[];
extern HarpCApp& app;

#pragma pack(push, 1)
struct app_regs_t
{
    volatile uint8_t port_dir; // 1 = output; 0 = input.
    volatile uint8_t port_state; // current gpio state. Readable and writeable.
    volatile uint8_t port_set;
    volatile uint8_t port_clear;

    volatile uint8_t enable_rising_edge_events;
    volatile uint8_t rising_edge_events;
    volatile uint8_t enable_falling_edge_events;
    volatile uint8_t falling_edge_events;

    uint8_t pwm_state;
    pwm_settings_t pwm_settings[NUM_GPIOS];
    uint8_t pwm_ready;
};
#pragma pack(pop)

extern app_regs_t app_regs;


inline uint32_t time_us_32_fast()
{return timer_hw->timerawl;}


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


void read_enable_rising_edge_events(uint8_t reg_address);
void write_enable_rising_edge_events(msg_t& msg);
void read_enable_falling_edge_events(uint8_t reg_address);
void write_enable_falling_edge_events(msg_t& msg);

void read_reg_error(uint8_t reg_address);
void write_reg_error(msg_t& msg);

void write_pwm_state(msg_t& msg);

/**
 * \brief App register handler function to read pwm settings from the given PWM
 *  output (1 per IO)
 * \note this handler function is shared across all `pwm_settings` registers.
 */
void read_any_pwm_settings(uint8_t reg_address);
/**
 * \brief App register handler function to write pwm settings to the given PWM
 *  output (1 per IO).
 * \note this handler function is shared across all `pwm_settings` registers.
 */
void write_any_pwm_settings(msg_t& msg);

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
