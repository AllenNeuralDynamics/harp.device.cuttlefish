#include <cuttlefish_app.h>

app_regs_t app_regs;
uint8_t pwm_task_mask; // Record of pins dedicated to running PWMTasks.

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
{HarpCore::write_reg_generic(msg);}

void read_falling_edge_events(uint8_t reg_address)
{HarpCore::read_reg_generic(reg_address);}

void write_falling_edge_events(msg_t& msg)
{HarpCore::write_reg_generic(msg);}



void update_app_state()
{
    //FIXME: Check for pin state changes in ISR.
    // Drain queue. Warn if pin change rate is too fast.
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
    app_regs.port_dir = 0x00; // all inputs
    app_regs.port_state = uint8_t(gpio_get_all() >> PORT_BASE);
}
