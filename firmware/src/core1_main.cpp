#include <core1_main.h>

core1_state_t state;
bool schedule_failed;
uint8_t active_pwm_pins;

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


//void populate_schedule()
//{
//    while (queue_try_remove(&pwm_msg_queue, &msg))
//    {
//        // TODO: If new, push to schedule.
//        //  Otherwise, update settings according to what pin they apply to.
//        if ((1u << msg.pin) & active_pwm_pins) // FIXME.
//        {
//            // Update existing settings.
//        }
//        else
//        {
//            active_pwm_pins |= (1u << msg.pin); // FIXME.
//            //scheduler.schedule_pwm_task(msg.specs); // FIXME: constructor.
//        }
//    }
//}


void run_task_loop()
{
//    core1_state_t next_state;
//    pwm_specs_core_msg_t msg;
//
//    // TODO: get inputs from core0 control queue.
//
//    // state-transition logic and calculate next-state.
//    switch (state)
//    {
//        case RESET:
//            active_pwm_pins = 0;
//            schedule_failed = false;
//            scheduler.reset(); // FIXME: doesn't reset cleanly yet.
//            next_state = READY;
//            break;
//        case READY:
//            populate_schedule();
//            next_state = (GO)? RUNNING : READY;
//            if (next_state == RUNNING)
//                scheduler.start();
//            break;
//        case RUNNING:
            scheduler.update();
//            if (schedule_failed)
//                next_state = RESET;
//            break;
//        default:
//            break;
//    }
//
//    // Handle output logic.
//
//    // Update state.
//    state = next_state;
}


// Core1 main.
void core1_main()
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
