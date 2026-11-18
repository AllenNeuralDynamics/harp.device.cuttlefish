#include "core1_main.h"
#include <etl/algorithm.h>

__not_in_flash("core1_next_state") core1_state_t state;
__not_in_flash("schedule_failed") bool schedule_failed;
__not_in_flash("scheduler")PWMScheduler scheduler;


/// Do not call this func inside and outside an ISR context on either core.
inline uint64_t time_us_64_unsafe()
{
    uint64_t time = timer_hw->timelr; // Locks time_lr/hr until we read TIMEHR.
    return (uint64_t(timer_hw->timehr) << 32) | time;
}


// Override default behavior of this function defined weakly elsewhere.
void handle_missed_deadline()
{
    schedule_failed = true;
    schedule_error_msg_t msg{schedule_error_t::MISSED_DEADLINE,
                             time_us_64_unsafe()};
    queue_try_add(&schedule_error_queue, &msg);
}


void sync_schedule()
{
    /// friend function to PWMScheduler, PWMTask, and AnalogPWMTask.
    /// Should only be called before running the schedule.
    pwm_specs_core_msg_t settings;
    bool schedule_changed = false;
    while (queue_try_remove(&pwm_settings_queue, &settings))
    {
        // If the pin is already assigned to a task of the same type, update
        // its settings. Otherwise, remove any task of the other type assigned
        // to this pin and create a new task.
        // Note: tasks hold no resources upon destruction, so they can be
        //  safely erased. Their pins are reconfigured by the new task.
        schedule_changed = true;
        uint32_t pin_mask = 1u << settings.pin;
        const pwm_settings_t& specs = settings.specs;
        auto& pwm_tasks = scheduler.pwm_tasks_;
        auto& analog_tasks = scheduler.analog_pwm_tasks_;
        auto pwm_it = etl::find_if(pwm_tasks.begin(), pwm_tasks.end(),
            [pin_mask](const PWMTask& t){return t.pin_mask_ == pin_mask;});
        auto analog_it = etl::find_if(analog_tasks.begin(), analog_tasks.end(),
            [pin_mask](const AnalogPWMTask& t){return t.pin_mask_ == pin_mask;});
        if (settings.analog)
        {
            if (pwm_it != pwm_tasks.end())
                pwm_tasks.erase(pwm_it);
            if (analog_it != analog_tasks.end())
                analog_it->apply_settings(specs.offset_us, specs.on_duration_us,
                                          specs.period_us(), pin_mask,
                                          specs.cycles, bool(specs.invert),
                                          settings.analog_frequency_hz,
                                          settings.analog_duty_cycle);
            else
                analog_tasks.emplace_back(specs.offset_us,
                                          specs.on_duration_us,
                                          specs.period_us(), pin_mask,
                                          specs.cycles, bool(specs.invert),
                                          settings.analog_frequency_hz,
                                          settings.analog_duty_cycle);
        }
        else
        {
            if (analog_it != analog_tasks.end())
                analog_tasks.erase(analog_it);
            if (pwm_it != pwm_tasks.end())
                pwm_it->apply_settings(specs.offset_us, specs.on_duration_us,
                                       specs.period_us(), pin_mask,
                                       specs.cycles, bool(specs.invert));
            else
                pwm_tasks.emplace_back(specs.offset_us, specs.on_duration_us,
                                       specs.period_us(), pin_mask,
                                       specs.cycles, bool(specs.invert));
        }
    }
    // Containers may have been modified, so references in the priority queue
    // may be stale. Reschedule everything.
    if (schedule_changed)
        scheduler.rebuild_schedule();
}


void __not_in_flash_func(run_task_loop)()
{
    using enum core1_state_t;

    core1_state_t next_state = state;
    pwm_specs_core_msg_t msg;

    //Get input from core0 control queue.
    pwm_ctrl_msg_t ctrl_msg;
    bool new_ctrl_msg = queue_try_remove(&core1_ctrl_queue, &ctrl_msg);

    // state-transition logic and calculate next-state.
    switch (state)
    {
        case RESET:
            next_state = READY;
            break;
        case READY:
            if (new_ctrl_msg && (ctrl_msg == pwm_ctrl_msg_t::START))
                next_state = RUNNING;
            break;
        case RUNNING:
            if (schedule_failed)
                next_state = RESET;
            else if (scheduler.finished() ||
                    ((new_ctrl_msg) && (ctrl_msg == pwm_ctrl_msg_t::STOP)))
                next_state = READY;
            break;
        default:
            break;
    }

    // Handle output logic based on curr state or state transition.
    switch (state)
    {
        case RESET:
            schedule_failed = false;
            scheduler.reset();
            break;
        case READY:
            sync_schedule();
            if (next_state == RUNNING)
            {
                // Tell core0 we started.
                core1_next_state_msg_t msg{next_state, time_us_64_unsafe()};
                scheduler.start(); // Start ASAP to maximize timestamp accuracy.
                queue_try_add(&core1_next_state_queue, &msg);
            }
            break;
        case RUNNING:
            scheduler.update();
            if (next_state == READY)
                scheduler.stop(); // Must call stop to re-setup all tasks.
            if ((next_state == RESET) || (next_state == READY))
            {
                // Tell core0 we stopped or got reset.
                core1_next_state_msg_t msg{next_state, time_us_64_unsafe()};
                queue_try_add(&core1_next_state_queue, &msg);
            }
            break;
    }

    // Update state.
    state = next_state;
}


// Core1 main.
void __not_in_flash_func(core1_main)()
{
    state = core1_state_t::RESET;
    while (true)
        run_task_loop();
}
