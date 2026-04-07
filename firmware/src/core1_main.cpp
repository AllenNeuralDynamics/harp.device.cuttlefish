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
    pwm_specs_core_msg_t msg;
    while (queue_try_remove(&pwm_msg_queue, &msg))
    {
        // TODO: If new, push to schedule.
        //  Otherwise, update settings according to what pin they apply to.
    }
}


void __not_in_flash_func(run_task_loop)()
{
    using enum pwm_ctrl_msg_t;

    core1_state_t next_state;
    pwm_specs_core_msg_t msg;

    //Get input from core0 control queue.
    pwm_ctrl_msg_t ctrl_msg;
    bool new_ctrl_msg = queue_try_remove(&core1_ctrl_queue, &msg);

    // state-transition logic and calculate next-state.
    switch (state)
    {
        case RESET:
            next_state = READY;
            break;
        case READY:
            if (!new_ctrl_msg)
                break;
            next_state = (ctrl_msg == START)? RUNNING : READY;
            break;
        case RUNNING:
            // TODO: also check if the schedule finished.
            if (schedule_failed || ((new_ctrl_msg) && (ctrl_msg == STOP)))
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
            scheduler.reset(); // FIXME: doesn't reset cleanly yet. should also stop.
            break;
        case READY:
            if (next_state == RUNNING)
                scheduler.start();
            else
                sync_schedule();
            break;
        case RUNNING:
            scheduler.update();
            if (next_state == RESET) // Tell core0 we finished.
                queue_try_add(&core1_state_queue, &next_state);
            break;
    }

    // Update state.
    state = next_state;
}


// Core1 main.
void __not_in_flash_func(core1_main)()
{
    // Set DEBUG LED
    gpio_init(LED1);
    gpio_set_dir(LED1, 1); // output
    gpio_put(LED1, 0); // Set off.

    schedule_failed = false; // TODO: consider a struct for this.
    state = RESET;
    while (true)
        run_task_loop();
}
