#include <cuttlefish_app.h>

app_regs_t app_regs;

// Define "specs" per-register
RegSpec app_reg_specs[]
{
    RegSpec::U8(&app_regs.port_dir,
        HarpCore::read_reg_generic, write_port_dir),
    RegSpec::U8(&app_regs.port_state,
        read_port_state, write_port_state),
    RegSpec::U8(&app_regs.port_set,
        HarpCore::read_reg_generic, write_port_set),
    RegSpec::U8(&app_regs.port_clear,
        HarpCore::read_reg_generic, write_port_clear),
    RegSpec::U8(&app_regs.enable_rising_edge_events,
        read_enable_rising_edge_events, write_enable_rising_edge_events),
    RegSpec::U8(&app_regs.rising_edge_events,
        read_rising_edge_events, write_rising_edge_events),
    RegSpec::U8(&app_regs.enable_falling_edge_events,
        read_enable_falling_edge_events, write_enable_falling_edge_events),
    RegSpec::U8(&app_regs.falling_edge_events,
        read_falling_edge_events, write_falling_edge_events),
    RegSpec::U8(&app_regs.pwm_start,
        read_pwm_start, write_pwm_start),
    RegSpec::U8(&app_regs.pwm_stop,
        read_pwm_stop, write_pwm_stop),
    RegSpec::U8Array(&app_regs.pwm_settings[0], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[1], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[2], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[3], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[4], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[5], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[6], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings),
    RegSpec::U8Array(&app_regs.pwm_settings[7], sizeof(pwm_settings_t),
        read_any_pwm_settings, write_any_pwm_settings)
};

const size_t APP_REG_COUNT = sizeof(app_reg_specs);


void write_port_dir(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Set Buffer ctrl pins and corresponding IO pin to both match.
    // Omit setting direction of pins used by existing PWM Tasks.
    gpio_put_masked(0x000000FF << PORT_DIR_BASE,
                    uint32_t(app_regs.port_dir) << PORT_DIR_BASE);
    gpio_set_dir_masked(0x000000FF << PORT_BASE,
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
    // write to output pins
    gpio_put_masked(uint32_t(app_regs.port_dir) << PORT_BASE,
                    uint32_t(app_regs.port_state) << PORT_BASE);
    // Read back what we just wrote since it's fast.
    // Add delay for change to take effect. (May be related to slew rate).
    asm volatile("nop \n nop \n nop");
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void write_port_set(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    gpio_put_masked(uint32_t(app_regs.port_set) << PORT_BASE,
                    uint32_t(app_regs.port_set) << PORT_BASE);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void write_port_clear(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    gpio_put_masked(uint32_t(app_regs.port_clear) << PORT_BASE, 0);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_enable_rising_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}


void write_enable_rising_edge_events(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t i = 0; i < NUM_GPIOS; ++i)
    {
        bool enabled = ((app_regs.enable_rising_edge_events >> i) & 1u);
        gpio_set_irq_enabled(i + PORT_BASE, GPIO_IRQ_EDGE_RISE, enabled);
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_rising_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}


void write_rising_edge_events(msg_t& msg)
{HarpCore::write_to_read_only_reg_error(msg);}


void read_enable_falling_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}


void write_enable_falling_edge_events(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t i = 0; i < NUM_GPIOS; ++i)
    {
        bool enabled = ((app_regs.enable_falling_edge_events >> i) & 1u);
        gpio_set_irq_enabled(i + PORT_BASE, GPIO_IRQ_EDGE_FALL, enabled);
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_falling_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}


void write_falling_edge_events(msg_t& msg)
{HarpCore::write_to_read_only_reg_error(msg);}


void read_pwm_start(uint8_t reg_address)
{
    // TODO
}


void write_pwm_start(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // TODO
    // FIXME: we need to know the schedule state of core1.
    // TODO: Error if we have never specified PWM Settings for the specified
    //  pins yet.
    // Send start cmd to core1.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_pwm_stop(uint8_t reg_address)
{
    // TODO
    // FIXME: we need to know the schedule state of core1.
}


void write_pwm_stop(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // TODO
    // FIXME: we need to know the schedule state of core1.
    // Send stop cmd to core1.
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}


void read_any_pwm_settings(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}


void write_any_pwm_settings(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    uint8_t pwm_index = msg.header.address - PWM_SETTINGS_0_APP_ADDRESS;
    // FIXME: we need to know the schedule state of core1.
/*
    if (last_core1_state.schedule_error)
    {
        if (!HarpCore::is_muted())
            HarpCore::send_harp_reply(WRITE_ERROR, msg.header.address);
        return;
    }
*/
    // Push new pwm settings to core1.
    pwm_settings_t& settings = app_regs.pwm_settings[pwm_index];
    size_t pwm_pin = pwm_index + PORT_BASE;
    pwm_specs_core_msg_t pwm_settings_msg(pwm_pin, settings);
    if (!queue_try_add(&pwm_msg_queue, &pwm_settings_msg))
    {
        if (!HarpCore::is_muted())
            HarpCore::send_harp_reply(WRITE_ERROR, msg.header.address);
        return;
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
    // TODO: do we also need to configure the Buffer to convert the TTL to output?
}


void handle_edge_event_callback(void)
{
    // FYI raw interrupt state for all 30 GPIOs is split across 4 registers
    // (INTR0, ..., INTR3).
    // Since Cuttlefish only has 8 consecutive GPIOS offset by a multiple of 8,
    // we can get away with doing a single register read to access the entire
    // interrupt state (rising, falling, high low) for the 8 pins of interest.
    EdgeEvent event;
    event.timestamp_us = time_us_64(); // ISR safe.
    event.rise_pins = 0;
    event.fall_pins = 0;
    uint32_t intr_state = io_bank0_hw->intr[PORT_BASE >> 3];
    uint32_t gpio_flags;
    // Split up rising/falling edge events.
    for (size_t i = 0; i < NUM_GPIOS; ++i)
    {
        gpio_flags = intr_state >> (i * 4); // 4 flags per gpio pin.
        event.rise_pins |= (((gpio_flags >> 3) & 1u) << i + PORT_BASE);
        event.fall_pins |= (((gpio_flags >> 2) & 1u) << i + PORT_BASE);
    }
    // Push the event
    queue_try_add(&edge_event_queue, &event);
    // Clear the INTR[n] state since we dealt with all pin changes.
    // Clear by "writing a 1" to the set bits.
    io_bank0_hw->intr[PORT_BASE >> 3] = 0xFFFFFFFF; // >> 3: floor-divide by 8
}


void update_app_state()
{
    // Check for pin state changes pushed to edge event queue.
    // Drain queue. Warn if pin change rate is too fast.
    EdgeEvent event;
    while (queue_try_remove(&edge_event_queue, &event))
    {
        if (HarpCore::is_muted())
            continue;
        // Copy to EVENT-only register and filter for enabled pins.
        app_regs.rising_edge_events = uint8_t(event.rise_pins >> PORT_BASE) &
                                       app_regs.enable_rising_edge_events;
        app_regs.falling_edge_events = uint8_t(event.fall_pins >> PORT_BASE) &
                                        app_regs.enable_falling_edge_events;
        // Push queued messages from rising or falling edge events register.
        if (app_regs.rising_edge_events)
        {
            uint64_t harp_time_us = HarpCore::system_to_harp_us_64(event.timestamp_us);
            HarpCore::send_harp_reply(EVENT, RISING_EDGE_EVENTS_ADDRESS, harp_time_us);
        }
        if (app_regs.falling_edge_events) // filter
        {
            uint64_t harp_time_us = HarpCore::system_to_harp_us_64(event.timestamp_us);
            HarpCore::send_harp_reply(EVENT, FALLING_EDGE_EVENTS_ADDRESS, harp_time_us);
        }
    }
    // TODO: Update core1 state.
    //queue_try_remove(&core1_state_queue, &core1_state);

}

void reset_app()
{
    // init all pins used as GPIOs.
    gpio_init_mask((0x000000FF << PORT_DIR_BASE) | (0x000000FF << PORT_BASE));
    // Reset unbuffered IO pins to inputs.
    gpio_set_dir_masked(0x000000FF << PORT_BASE, 0);
    // Reset Buffer ctrl pins to all outputs and drive an input setting.
    gpio_set_dir_masked(0x000000FF << PORT_DIR_BASE, 0xFFFFFFFF);
    gpio_put_masked(0x000000FF << PORT_DIR_BASE, 0);
    // Reset Harp register struct elements.
    app_regs.port_dir = 0x00; // all inputs
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);

    // TODO: apply Dummy settings to the pwm_settings_t Registers.
    // TODO: reset core1.

    // Drain the EdgeEvent queue.
    EdgeEvent dummy_event;
    while (queue_try_remove(&edge_event_queue, &dummy_event)) {}
    // Detach any existing interrupt handlers.
    for (size_t i = 0; i < NUM_GPIOS; ++i)
        gpio_set_irq_enabled(i + PORT_BASE,
                             GPIO_IRQ_EDGE_RISE | GPIO_IRQ_EDGE_FALL, false);
    // Attach interrupt handlers.
    irq_add_shared_handler(IO_IRQ_BANK0, handle_edge_event_callback,
                           GPIO_IRQ_CALLBACK_ORDER_PRIORITY);
    irq_set_enabled(IO_IRQ_BANK0, true);

}
