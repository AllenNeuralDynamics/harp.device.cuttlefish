#ifndef PWM_SCHEDULER_H
#define PWM_SCHEDULER_H
#include <stdint.h>
#include <pico/stdlib.h>
#include <hardware/irq.h>
#include <pwm_task.h>
#include <etl/priority_queue.h>
#ifdef DEBUG
    #include <cstdio> // for printf
#endif


#define NUM_ENTRIES (64)
#define NUM_TTL_IOS (8)
#define MAX_QUEUEABLE_ALARMS (4)


class PWMScheduler
{
public:
    PWMScheduler();
    ~PWMScheduler();

    void schedule_pwm_task(PWMTask& task)
    {
        pq_.push(task);
#ifdef DEBUG
        printf("Pushed PWMTask: (%d, %d, %d, 0x%08x)\r\n",
               task.delay_us_, task.on_time_us_, task.period_us_, task.pin_mask());
#endif
    }
    void start();
    void clear();

    friend int64_t set_new_ttl_pin_state(alarm_id_t id, void* user_data);

/**
 * \brief called periodically. Sets up next PWMTask to occur on a timer.
 */
    void update();

/**
 * \brief absolute time before which the priority queue needs to be updated.
 */
    uint64_t next_update_time_us_;

private:

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

    volatile uint32_t next_gpio_port_state_;
    volatile uint32_t next_gpio_port_mask_;
    volatile bool alarm_queued_;
};
#endif // PWM_SCHEDULER_H
