#ifndef PWM_SCHEDULER_H
#define PWM_SCHEDULER_H
#include <stdint.h>
#include <pico/stdlib.h>
#include <hardware/irq.h>
#include <pwm_task.h>
#include <analog_pwm_task.h>
#include <etl/priority_queue.h>
#include <etl/deque.h>
#include <hardware/timer.h>
#ifdef DEBUG
    #include <cstdio> // for printf
#endif

/**
 * \brief called (possibly from an ISR) when the schedule changes the output
 *  level of any pins driven by PWMTasks or AnalogPWMTasks.
 * \details weakly defined as a no-op. Override (and place in RAM) to report
 *  edges from the schedule rather than from physical pin changes. Edges
 *  reflect the output level (i.e: with inversion applied). For
 *  AnalogPWMTasks, edges reflect the nominal output rather than the
 *  high-frequency output.
 * \param rise_pins GPIO mask of pins that switched HIGH.
 * \param fall_pins GPIO mask of pins that switched LOW.
 */
void handle_scheduled_edges(uint32_t rise_pins, uint32_t fall_pins);

#define NUM_ENTRIES (64)
#define NUM_TTL_IOS (8)


class PWMScheduler
{
public:

/**
 * \brief struct that contains the state of the GPIO port at a specified time.
 * \details `PortEvent`s are queued into a FIFO as the scheduler computes them
 *  and dequeued by ISR which also applies the port state at the specified time.
 */
    struct PortEvent
    {
        uint32_t mask;      /// port mask
        uint32_t state;     /// port state
        uint32_t time_us;   /// time (in [us]) when the state takes place.
    };

    PWMScheduler();
    ~PWMScheduler();

/**
 * \brief create and schedule a PWMTask.
 * \warning only meant to be called before starting the schedule.
 */
    void create_and_schedule_pwm_task(uint32_t delay_us, uint32_t t_on_us,
                                      uint32_t t_period_us, uint32_t pin_mask,
                                      uint32_t count, bool invert);

/**
 * \brief create and schedule an AnalogPWMTask.
 * \warning only meant to be called before starting the schedule.
 */
    void create_and_schedule_analog_pwm_task(uint32_t delay_us, uint32_t t_on_us,
                                             uint32_t t_period_us,
                                             uint32_t pin_mask, uint32_t count,
                                             bool invert, float frequency_hz,
                                             float duty_cycle);

/**
 * \brief schedule an existing PWMTask.
 * \warning only meant to be called before starting the schedule.
 */
    void schedule_pwm_task(PWMTask& task);

/**
 * \brief schedule an existing AnalogPWMTask.
 * \warning only meant to be called before starting the schedule.
 */
    void schedule_analog_pwm_task(AnalogPWMTask& task);

/**
 * \brief clear the priority queue and reschedule all stored tasks.
 * \warning only meant to be called before starting the schedule.
 */
    void rebuild_schedule();
/**
 * \brief cancel any active alarms and clear the queue.
 */
    void reset();

/**
 * \brief start or restart the schedule.
 */
    void start();


/**
 * \brief stop the schedule, but do not clear uploaded PWMTasks.
 */
    void stop();


/**
 * \brief true if the scheduler is done executing the schedule and no longer
 *  needs to be updated.
 */
    bool finished()
    {return port_event_queue_.empty() && !alarm_queued_;}

    inline void clear()
    {reset();}

    friend void set_new_ttl_pin_state(void);
    friend void handle_missed_deadline();
    friend void sync_schedule();


/**
 * \brief called periodically. Sets up next PWMTask to occur on a timer.
 */
    void update();

    void cancel_alarm();

/**
 * \brief apply a port state to all pins in the mask, including pins driven by
 *  AnalogPWMTasks.
 */
    static __force_inline void apply_port_state(uint32_t mask, uint32_t state)
    {
        uint32_t analog_mask = mask & analog_pin_mask_;
        gpio_put_masked(mask & ~analog_mask, state);
        uint32_t turning_on = mask & state & ~on_mask_;
        uint32_t turning_off = mask & ~state & on_mask_;
        on_mask_ = (on_mask_ & ~mask) | (mask & state);
        uint32_t invert_mask = invert_mask_;
        if (analog_mask)
        {
            // Restart the hardware PWM period of slices turning on so each
            // burst starts at the beginning of a period. Skip this if the
            // other pin of the slice is already on to avoid disturbing its
            // output.
            uint32_t analog_turning_on = turning_on & analog_mask;
            uint32_t already_on = on_mask_ & analog_pin_mask_
                                  & ~analog_turning_on;
            // Note: avoid __builtin_ctz, which calls a function in flash.
            for (uint32_t pin = 0, pins = analog_turning_on; pins;
                 ++pin, pins >>= 1)
            {
                if ((pins & 1u) && !((already_on >> (pin ^ 1u)) & 1u))
                    AnalogPWMTask::restart_period(pin);
            }
            for (uint32_t pin = 0, pins = analog_mask; pins; ++pin, pins >>= 1)
            {
                if (pins & 1u)
                    AnalogPWMTask::set_output(pin, (state >> pin) & 1u,
                                              (invert_mask >> pin) & 1u);
            }
        }
        if (turning_on | turning_off)
            handle_scheduled_edges((turning_on & ~invert_mask)
                                   | (turning_off & invert_mask),
                                   (turning_off & ~invert_mask)
                                   | (turning_on & invert_mask));
    }

/**
 * \brief absolute time before which the priority queue needs to be updated.
 */
    uint64_t next_update_time_us_;

private:
    etl::vector<PWMTask, NUM_TTL_IOS> pwm_tasks_; // Container to hold PWMTasks.
                                                  // We will access them
                                                  // (usually) through the pq_;
    etl::vector<AnalogPWMTask, NUM_TTL_IOS> analog_pwm_tasks_;
/**
 * \brief the priority queue
 * \details We need to use references wrappers to access mutable versions of
 *      queue elements. This is consistent with the std::priority_queue
 *      implementation which returns const references to top().
 */
    etl::priority_queue<std::reference_wrapper<PWMTask>,
                        NUM_ENTRIES,
                        etl::vector<std::reference_wrapper<PWMTask>, NUM_ENTRIES>,
                        etl::greater<std::reference_wrapper<PWMTask>>> pq_;

private:
    static volatile int32_t alarm_num_;
    static etl::deque<PortEvent, NUM_TTL_IOS> port_event_queue_;
    static volatile uint32_t next_gpio_port_state_;
    static volatile uint32_t next_gpio_port_mask_;
    static volatile bool alarm_queued_;
    static volatile uint32_t analog_pin_mask_; /// pins driven by AnalogPWMTasks.
    static volatile uint32_t invert_mask_; /// pins of inverted tasks.
    static volatile uint32_t on_mask_; /// pins of tasks that are currently on.
};
#endif // PWM_SCHEDULER_H
