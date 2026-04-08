#include <core1_main.h>

__not_in_flash("core1_state") core1_state_t state;
__not_in_flash("schedule_failed") bool schedule_failed;
__not_in_flash("scheduler")PWMScheduler scheduler;


// Override default behavior of this function defined weakly elsewhere.
void handle_missed_deadline()
{
    gpio_init(LED1);
    gpio_set_dir(LED1, 1); // output
    gpio_put(LED1, 1); // turn on auxilary LED.
    schedule_failed = true;
    // TODO: push an error message back to core0.
}


void sync_schedule()
{
    /// friend funtion to PWMScheduler and PWMTask.
    /// Should only be called before running the schedule.
    pwm_specs_core_msg_t settings;
    while (queue_try_remove(&pwm_settings_queue, &settings))
    {
        // If the pin is already assigned to a specific set of settings in the
        // schedule, update them. Otherwise, create a new PWMTask and
        // push it into the schedule.
        //
        // Note that since the schedule is presorted, we need to pop out
        // all tasks until we find the right one, update it, and then push
        // them all back--gross.
        bool updated_existing_task = false;
        etl::vector<std::reference_wrapper<PWMTask>, NUM_TTL_IOS> tmp_tasks;
        while (!scheduler.pq_.empty())
        {
            PWMTask& task = scheduler.pq_.top().get();
            if (task.pin_mask_ != (1u << settings.pin))
            {
                scheduler.pq_.pop();
                tmp_tasks.push_back(task);
                continue;
            }
            // Update existing task specs.
            task.delay_us_ = settings.specs.offset_us;
            task.on_time_us_ = settings.specs.on_duration_us;
            task.period_us_ = settings.specs.period_us();
            task.count_ = settings.specs.cycles;
            task.invert_ = bool(settings.specs.invert);
            updated_existing_task = true;
            break;
        }
        while (!tmp_tasks.empty())
        {
            PWMTask& task = tmp_tasks.back();
            scheduler.pq_.push(task);
            tmp_tasks.pop_back();
        }
        if (!updated_existing_task)
        {
            scheduler.schedule_pwm_task(settings.specs.offset_us,
                                        settings.specs.on_duration_us,
                                        settings.specs.period_us(),
                                        1u << settings.pin,
                                        settings.specs.cycles,
                                        bool(settings.specs.invert));
        }
    }
}


void __not_in_flash_func(run_task_loop)()
{
    using enum pwm_ctrl_msg_t;

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
            if (new_ctrl_msg && (ctrl_msg == START))
                next_state = RUNNING;
            break;
        case RUNNING:
            if (schedule_failed || scheduler.finished() ||
                ((new_ctrl_msg) && (ctrl_msg == STOP)))
                next_state = RESET;
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
                // TODO: tell core0 we started (timestamp).
                //queue_try_add(&core1_state_queue, &next_state);
                scheduler.start();
            }
            break;
        case RUNNING:
            scheduler.update();
            if (next_state == RESET) // TODO: Tell core0 we finished (timestamp).
                queue_try_add(&core1_state_queue, &next_state);
            break;
    }

    // Update state.
    state = next_state;
}


// Core1 main.
void __not_in_flash_func(core1_main)()
{
    state = RESET;
    while (true)
        run_task_loop();
}
