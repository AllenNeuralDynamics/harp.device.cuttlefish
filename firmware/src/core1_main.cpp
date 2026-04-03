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


void run_task_loop()
{

    core1_state_t next_state;
    pwm_specs_core_msg_t msg;

    // Handle in-state logic and calculate next-state.
    switch (state)
    {
        case RESET:
            active_pwm_pins = 0;
            schedule_failed = false;
            scheduler.reset(); // FIXME: doesn't reset cleanly yet.
            while (queue_try_remove(&pwm_msg_queue, &msg))
            {
                // TODO: If new, push to schedule. Otherwise, update settings.
                if ((1u << msg.pin) & active_pwm_pins) // FIXME.
                {
                    // Update existing settings.
                }
                else
                {
                    active_pwm_pins |= (1u << msg.pin); // FIXME.
                    //scheduler.schedule_pwm_task(msg.specs); // FIXME: constructor.
                }
            }
            next_state = READY;
            break;
        case READY:
/*
            // TODO: keep checking the queue.
            next_state = (GO)? RUNNING : READY;
            if (next_state == READY)
                break;
            scheduler.start();
            #pragma unroll
            for (size_t i = 0; i < NUM_TTL_IOS; ++i)
            {   scheduler.update();}
*/
            break;
        case RUNNING:
            scheduler.update();
            if (schedule_failed)
                next_state = RESET;
            break;
        default:
            break;
    }

    // Handle output logic.
    switch (state)
    {


    };

    // Update state.
    state = next_state;
}


/*
void old_run_task_loop()
{
    // Retrieve PWMTask specs from the shared queue.
    uint8_t cmd = 0;
    uint8_t abort_schedule = 0;
    pwm_specs_core_msg_t msg;
    // Load and wait for schedule start cmds.
    while(true)
    {
        if(!queue_is_empty(&pwm_task_setup_queue))
        {
            queue_remove_blocking(&pwm_task_setup_queue, &msg);
            scheduler.schedule_pwm_task(msg.specs.offset_us,
                                           msg.specs.on_duration_us,
                                           msg.specs.period_us(),
                                           (1u << msg.pin), // FIXME: did we deal with offset?
                                           msg.specs.cycles,
                                           bool(msg.specs.invert));
        }
        if (!queue_is_empty(&cmd_signal_queue))
            queue_try_remove(&cmd_signal_queue, &cmd);
        if (cmd == 1) // start signal.
            break;
        if (cmd == 1u << 1) // bail early.
        {
            scheduler.reset();
            return;
        }
    }
    // Start after receiving the start signal.
    while(true)
    {
        // Wait for schedule restart cmds.
        while(!cmd) // Skip the first time.
            queue_try_remove(&cmd_signal_queue, &cmd);
        cmd = 0; // Clear received cmd for next iteration.

        scheduler.start();
        #pragma unroll
        for (size_t i = 0; i < NUM_TTL_IOS; ++i)
        {   scheduler.update();}
        while(true)
        {
            scheduler.update(); // internally only updates as needed.
            // Check for stop signal, scheduler errors, or an idle scheduler.
            if (!queue_try_remove(&cmd_signal_queue, &cmd))
                continue;
            abort_schedule = cmd & 0x02;
            if (schedule_failed || abort_schedule)
                break;
        }
        if (schedule_failed || abort_schedule)
            break;
    }
    if (schedule_failed)
    {
        // Dispatch error message to core0.
        uint8_t error = 1;
        queue_try_add(&schedule_error_signal_queue, &error);
    }
    if (abort_schedule)
    {

    }
    // FIXME: cleanup GPIOs. Add this to each task in the destructor.
    // Clear existing tasks for next iteration.
    scheduler.reset();
}
*/




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
