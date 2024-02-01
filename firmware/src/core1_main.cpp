#include <core1_main.h>

#if defined(PROFILE_CPU)
// Note: this fn should not be called inside an interrupt.
inline uint64_t time_us_64_unsafe()
{
    uint64_t time = timer_hw->timelr; // Locks time until we read TIMEHR.
    return (uint64_t(timer_hw->timehr) << 32) | time;
}
#endif

PWMScheduler pwm_schedule;
etl::vector<PWMTask, NUM_TTL_IOS> pwm_tasks; // Allocate space for up to 8 tasks.

// Core1 main.
void core1_main()
{
#if defined(DEBUG) || defined(PROFILE_CPU)
#warning "Initializing uart printing will slow down main loop."
    stdio_uart_init_full(uart0, 921600, UART_TX_PIN, -1); // use uart0 tx only.
    printf("hello, from core1\r\n");
#endif
#if defined(PROFILE_CPU)
    // Configure SYSTICK register to tick with CPU clock (125MHz) and enable it.
    SYST_CSR |= (1 << 2) | (1 << 0);
    uint32_t loop_start_cpu_cycle;
    uint32_t prev_print_time_us;
    uint32_t curr_time_us;
    uint32_t cpu_cycles;
    uint32_t max_cpu_cycles = 0;
#endif
    // Retrieve waveforms from the shared queue.
    uint8_t cmd = 0;
    pwm_task_specs_t task_specs;
    do
    {
        while (!queue_is_empty(&pwm_task_setup_queue))
        {
            queue_remove_blocking(&pwm_task_setup_queue, &task_specs);
            pwm_tasks.push_back(PWMTask(task_specs.offset_us,
                                        task_specs.on_time_us,
                                        task_specs.period_us,
                                        task_specs.port_mask,
                                        task_specs.cycles,
                                        task_specs.invert));
            pwm_schedule.schedule_pwm_task(pwm_tasks.back());
        }
        if (!queue_is_empty(&cmd_signal_queue))
            queue_try_remove(&cmd_signal_queue, &cmd);
    } while (!cmd);
    //pwm_tasks.push_back(PWMTask(0, 5, 10, (1u << LED1)|(1u << PORT_BASE))); // 10 KHz
    //pwm_tasks.push_back(PWMTask(0, 20, 40, (1u << LED1)|(1u << PORT_BASE))); // 10 KHz
    //pwm_tasks.push_back(PWMTask(0, 100, 200, (1u << LED0)));
    //pwm_schedule.schedule_pwm_task(pwm_tasks[0]);
    //pwm_schedule.schedule_pwm_task(pwm_tasks[1]);

    // Start after receiving the start signal.
    pwm_schedule.start();
    while(true)
    {
#if defined(PROFILE_CPU)
#warning "Profiling this loop may cause the first iteration to take too long."
        loop_start_cpu_cycle = SYST_CVR;
        curr_time_us = time_us_64_unsafe();
#endif
        pwm_schedule.update(); // internally only updates as needed.
#if defined(PROFILE_CPU)
        // SYSTICK is 24bit and counts down.
        cpu_cycles = ((loop_start_cpu_cycle << 8) - (SYST_CVR << 8)) >> 8;
        if (cpu_cycles > max_cpu_cycles)
            max_cpu_cycles = cpu_cycles;
        if ((curr_time_us - prev_print_time_us) < PRINT_LOOP_INTERVAL_US)
            continue;
        prev_print_time_us = curr_time_us;
        printf("max cycles/loop: %lu\r\n", max_cpu_cycles);
        max_cpu_cycles = 0;
#endif
    }
}
