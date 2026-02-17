#include <cuttlefish_app.h>

app_regs_t app_regs;

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.port_dir, sizeof(app_regs.port_dir), U8}, // 32
    {(uint8_t*)&app_regs.port_state, sizeof(app_regs.port_state), U8},  // 33
    {(uint8_t*)&app_regs.port_set, sizeof(app_regs.port_set), U8},
    {(uint8_t*)&app_regs.port_clear, sizeof(app_regs.port_clear), U8},

    {(uint8_t*)&app_regs.rising_edge_events, sizeof(app_regs.rising_edge_events), U8},
    {(uint8_t*)&app_regs.falling_edge_events, sizeof(app_regs.rising_edge_events), U8},

};

RegFnPair reg_handler_fns[reg_count]
{
    {HarpCore::read_reg_generic, write_port_dir},           // 32
    {read_port_state, write_port_state},                    // 33
    {HarpCore::read_reg_generic, write_port_set},    // "write-only"
    {HarpCore::read_reg_generic, write_port_clear},  // "write-only"

    {read_rising_edge_events, write_rising_edge_events},
    {read_falling_edge_events, write_falling_edge_events},

};

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

void read_rising_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}

void write_rising_edge_events(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t i = 0; i < NUM_GPIOS; ++i)
    {
        bool enabled = ((app_regs.rising_edge_events >> i) & 1u);
        gpio_set_irq_enabled(i + PORT_BASE, GPIO_IRQ_EDGE_RISE, enabled);
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_falling_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}

void write_falling_edge_events(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    for (size_t i = 0; i < NUM_GPIOS; ++i)
    {
        bool enabled = ((app_regs.falling_edge_events >> i) & 1u);
        gpio_set_irq_enabled(i + PORT_BASE, GPIO_IRQ_EDGE_FALL, enabled);
    }
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void handle_edge_event_callback(void)
{
    gpio_put(LED1, !gpio_get(LED1));
    // FYI raw interrupt state for all 30 GPIOs is split across 4 registers
    // (INTR0, ..., INTR3).
    // Since Cuttlefish only has 8 consecutive GPIOS offset by a multiple of 8,
    // we can get away with doing a single register read to access the entire
    // interrupt state (rising, falling, high low) for the 8 pins of interest.
    EdgeEvent event;
    uint32_t intr_state = io_bank0_hw->intr[PORT_BASE >> 3];
    event.timestamp_us = time_us_64(); // ISR safe.
    // Split up rising/falling edge events.
    uint32_t gpio_flags;
    for (size_t i = 0; i < NUM_GPIOS; ++i)
    {
        gpio_flags = intr_state >> (i * 4); // 4 flags per gpio pin.
        event.rise_pins |= ((gpio_flags >> GPIO_IRQ_EDGE_RISE) & 1u) << i;
        event.fall_pins |= ((gpio_flags >> GPIO_IRQ_EDGE_FALL) & 1u) << i;
    }
    // push the event
    queue_try_add(&edge_event_queue, &event);
    // Clear the INTR[n] state since we dealt with all pin changes.
    // Clear by "writing a 1" to the set bits.
    io_bank0_hw->intr[PORT_BASE >> 3] = 0xFFFFFFFF;
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
        uint8_t rise_pins = uint8_t(event.rise_pins >> PORT_BASE);
        uint8_t fall_pins = uint8_t(event.fall_pins >> PORT_BASE);
        if (rise_pins & app_regs.rising_edge_events) // apply bitmask.
        {
            // Push queued messages from rising or falling edge events register.
            uint64_t harp_time_us = HarpCore::system_to_harp_us_64(event.timestamp_us);
            HarpCore::send_harp_reply(EVENT, RISING_EDGE_EVENTS_ADDRESS, harp_time_us);
        }
        if (fall_pins & app_regs.falling_edge_events) // apply bitmask
        {
            uint64_t harp_time_us = HarpCore::system_to_harp_us_64(event.timestamp_us);
            HarpCore::send_harp_reply(EVENT, RISING_EDGE_EVENTS_ADDRESS, harp_time_us);
        }
    }

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

    // For DEBUGGING
    gpio_init(LED1);
    gpio_set_dir(LED1, GPIO_OUT);
    gpio_put(LED1, 1);

    // Drain the queue.
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
