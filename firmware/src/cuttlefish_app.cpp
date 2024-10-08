#include <cuttlefish_app.h>

app_regs_t app_regs;
uint8_t pwm_task_mask; // Record of pins dedicated to running PWMTasks.

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.port_dir, sizeof(app_regs.port_dir), U8}, // 32
    {(uint8_t*)&app_regs.port_state, sizeof(app_regs.port_state), U8},  // 33
    {(uint8_t*)&app_regs.pwm_task, sizeof(app_regs.pwm_task), U8},  // 34
    {(uint8_t*)&app_regs.arm_ext_trigger, sizeof(app_regs.arm_ext_trigger), U8},
    {(uint8_t*)&app_regs.ext_trigger_edge, sizeof(app_regs.ext_trigger_edge), U8},
    {(uint8_t*)&app_regs.arm_ext_trigger, sizeof(app_regs.arm_ext_trigger), U8}, // 37
    {(uint8_t*)&app_regs.ext_untrigger_edge, sizeof(app_regs.ext_untrigger_edge), U8}, // 38
    {(uint8_t*)&app_regs.sw_trigger, sizeof(app_regs.sw_trigger), U8}, // 39
    {(uint8_t*)&app_regs.sw_untrigger, sizeof(app_regs.sw_untrigger), U8}, // 40
    {(uint8_t*)&app_regs.schedule_ctrl, sizeof(app_regs.schedule_ctrl), U8} // 41

    // More specs here if we add additional registers.
};

RegFnPair reg_handler_fns[reg_count]
{
    {HarpCore::read_reg_generic, write_port_dir},           // 32
    {read_port_state, write_port_state},                    // 33
    {HarpCore::read_reg_generic, write_pwm_task},           // 34
    {HarpCore::read_reg_generic, write_arm_ext_trigger},    // 35
    {HarpCore::read_reg_generic, write_ext_trigger_edge},   // 36
    {HarpCore::read_reg_generic, write_arm_ext_untrigger},    // 37
    {HarpCore::read_reg_generic, write_ext_untrigger_edge}, // 38
    {HarpCore::read_reg_generic, write_sw_trigger},         // 39
    {HarpCore::read_reg_generic, write_sw_untrigger},       // 40
    {HarpCore::read_reg_generic, write_schedule_ctrl}       // 41
    // More handler function pairs here if we add additional registers.
};

void reset_schedule()
{
    // Abort any existing schedule on core1.
    uint8_t abort_schedule = 1u << 1; // FIXME: use bitfields via struct interface.
    // Signal to core1 to trash all PWMTasks and reset their GPIO pins to input.
    queue_try_add(&cmd_signal_queue, &abort_schedule);
    // Reset the corresponding PORT pins to input.
    gpio_put_masked(uint32_t(pwm_task_mask) << PORT_DIR_BASE, 0);
    // Represent this change in Harp register. // TODO: validate this.
    app_regs.port_dir &= uint8_t(~pwm_task_mask >> PORT_DIR_BASE);
    pwm_task_mask = 0; // Clear reserved pwm task pins.
}

void write_port_dir(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Set Buffer ctrl pins and corresponding IO pin to both match.
    // Omit setting direction of pins used by existing PWM Tasks.
    // Note: additional 0x000000FF masking is because bitwise '~' on a uint8_t
    //  will perform integral promotion automatically for a 32-bit system.
    gpio_put_masked((0x000000FF & uint32_t(~pwm_task_mask)) << PORT_DIR_BASE,
                    uint32_t(app_regs.port_dir) << PORT_DIR_BASE);
    gpio_set_dir_masked((0x000000FF & uint32_t(~pwm_task_mask)) << PORT_BASE,
                        uint32_t(app_regs.port_dir) << PORT_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_port_state(uint8_t reg_address)
{
    // Include the state of pins driven by PWMTasks.
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(READ, reg_address);
}

void write_port_state(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Exclude pins controlled by PWM tasks.
    uint8_t output_pins = app_regs.port_dir & ~pwm_task_mask;
    // write to output pins (not including pins controlled by PWM Tasks).
    gpio_put_masked(uint32_t(output_pins) << PORT_BASE,
                    uint32_t(app_regs.port_state) << PORT_BASE);
    // Read back what we just wrote since it's fast.
    // Add delay for change to take effect. (May be related to slew rate).
    asm volatile("nop \n nop \n nop");
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);
#if defined(DEBUG)
    printf("Wrote to GPIOs. New GPIO state: 0x%08lx\r\n", gpio_get_all());
#endif
    // Reply with the actual value that we wrote.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void write_pwm_task(msg_t& msg)
{
    // FIXME: writing a pwm task config while the schedule is running should
    //     throw an error.
    // Note: we do not copy payload to register. We will send this data directly
    // to the PWM scheduler without storing a Harp register representation.

    // Interpret byte array as packed function arguments to create a PWMTask.
    pwm_task_specs_t& specs = *((pwm_task_specs_t*)msg.payload);
    pwm_task_mask |= specs.port_mask; // Track pins controlled by PWM Tasks.
    // Set Buffer ctrl pins to output for the PWM task pin.
    gpio_put_masked(uint32_t(pwm_task_mask) << PORT_DIR_BASE, 0xFFFFFFFF);
    // Set corresponding IO pins to outputs for the PWM Task.
    gpio_set_dir_masked(uint32_t(pwm_task_mask) << PORT_BASE, 0xFFFFFFFF);
    // Send the task to core1.
    queue_try_add(&pwm_task_setup_queue, &specs);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_arm_ext_trigger(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    uint8_t& ext_trigger_mask = *((uint8_t*)msg.payload);
    // Set Buffer ctrl pins and corresponding IO pins to inputs for the trigger.
    gpio_set_dir_masked(uint32_t(app_regs.arm_ext_trigger) << PORT_DIR_BASE |
                        uint32_t(app_regs.arm_ext_trigger) << PORT_BASE,
                        0);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_ext_trigger_edge(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Check if ext trigger has already been configured. Re-configure if so.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_arm_ext_untrigger(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    uint8_t& ext_untrigger_mask = *((uint8_t*)msg.payload);
    // FIXME: implement this.
    // Set pin as input if not already set as such.
    // TODO: consider using interrupt to start the schedule.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_ext_untrigger_edge(msg_t& msg)
{
    // FIXME: implement this.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_sw_trigger(msg_t& msg)
{
    const uint8_t& start = *((uint8_t*)msg.payload);
    if (start)
        queue_try_add(&cmd_signal_queue, &start);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_sw_untrigger(msg_t& msg)
{
    // FIXME: implement this.
    //const uint8_t& start = *((uint8_t*)msg.payload);
    //if (start)
    //    queue_try_add(&cmd_signal_queue, &start);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_schedule_ctrl(msg_t& msg)
{
    const uint8_t& schedule_ctrl = *((uint8_t*)msg.payload);
    if (schedule_ctrl & 0x01)
    {
        // Reset the schedule.
        reset_schedule();
    }
    if (schedule_ctrl & 0x02)
    {
        // TODO: dump schedule.
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void update_app_state()
{
    uint8_t input_pins = ~app_regs.port_dir;
    uint8_t old_port_state = app_regs.port_state;
    // Update raw port state.
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);
    // Filter for input pins that changed.
    uint8_t changed_input_pins = ((old_port_state ^ app_regs.port_state) & input_pins);
    // Check to see if any pins trigger the PWM Task schedule.
    uint8_t rising_edge_pins = changed_input_pins & app_regs.port_state;
    uint8_t falling_edge_pins = changed_input_pins & old_port_state;
    uint8_t trigger = app_regs.arm_ext_trigger &
        ((rising_edge_pins & app_regs.ext_trigger_edge) |
         (falling_edge_pins & ~app_regs.ext_trigger_edge));
    // Start PWM Task schedule in core1 if externally triggered.
    if (trigger)
    {
        uint8_t start = 1;
        queue_try_add(&cmd_signal_queue, &start);
    }
    if (HarpCore::is_muted())
        return;
    // Issue event from port state register if any *input* channel has changed.
    // Exclude PWM Task pins.
    if (changed_input_pins & ~pwm_task_mask)
        HarpCore::send_harp_reply(EVENT, 33); // port_state address. FIXME: magic #
    // Check pwm schedule state.
    uint8_t schedule_error = 0;
    if (!queue_is_empty(&schedule_error_signal_queue))
        queue_try_remove(&schedule_error_signal_queue, &schedule_error);
    app_regs.error_state |= schedule_error;
    if (schedule_error != 0)
        HarpCore::send_harp_reply(EVENT, 43); // error_state address. FIXME: magic #
}

void reset_app()
{
    pwm_task_mask = 0; // Clear local tracking of pwm task pins.
    // init all pins used as GPIOs.
    gpio_init_mask((0x000000FF << PORT_DIR_BASE) | (0x000000FF << PORT_BASE));
    // Reset unbuffered IO pins to inputs.
    gpio_set_dir_masked(0x000000FF << PORT_BASE, 0);
    // Reset Buffer ctrl pins to all outputs and drive an input setting.
    gpio_set_dir_masked(0x000000FF << PORT_DIR_BASE, 0xFFFFFFFF);
    gpio_put_masked(0x000000FF << PORT_DIR_BASE, 0);
    // Reset Harp register struct elements.
    app_regs.port_dir = 0xFF; // all outputs.
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);
    app_regs.arm_ext_trigger = 0x00;
    app_regs.ext_trigger_edge = 0xFF; // Rising edge.
    app_regs.arm_ext_untrigger = 0x00;
    app_regs.ext_untrigger_edge = 0xFF; // Rising edge.
    app_regs.status = 0;    // FIXME: update status in other locations.
    app_regs.error_state = 0;
    reset_schedule();
}
