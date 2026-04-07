#include <pico/stdlib.h>
#include <cstring>
#include <config.h>
#include <harp_c_app.h>
#include <harp_synchronizer.h>
#include <cuttlefish_app.h>
#include <edge_event_queue.h>
#include <schedule_ctrl_queues.h>
#include <pico/multicore.h>
#include <hardware/structs/bus_ctrl.h>
#include <core1_main.h>

queue_t pwm_msg_queue;
// Keep timing critical core0 to ISR data structures in RAM.
__not_in_flash("edge_event_queue") queue_t edge_event_queue;
// Keep timing critical core-to-core communication data structures in RAM.
__not_in_flash("core1_ctrl_queue") queue_t core1_ctrl_queue;
__not_in_flash("core1_state_queue") queue_t core1_state_queue;
__not_in_flash("schedule_error_queue") queue_t schedule_error_queue;

// Create Core.
HarpCApp& app = HarpCApp::init(HARP_DEVICE_ID,
                               HW_VERSION_MAJOR, HW_VERSION_MINOR,
                               HW_ASSEMBLY_VERSION,
                               FW_VERSION_MAJOR, FW_VERSION_MINOR,
                               UNUSED_SERIAL_NUMBER, "Cuttlefish",
                               (uint8_t*)GIT_HASH,
                               app_reg_specs, APP_REG_COUNT, update_app_state,
                               reset_app);

// Core0 main.
int main()
{
    // Init Synchronizer.
    HarpSynchronizer::init(SYNC_UART, HARP_SYNC_RX_PIN);
    app.set_synchronizer(&HarpSynchronizer::instance());
    // Configure core1 to have high priority on the bus.
    bus_ctrl_hw->priority = 0x00000010;
    // Initialize queue for edge event message handling
    queue_init(&edge_event_queue, sizeof(EdgeEvent), 32);
    // Initialize queues for multicore communication.
    queue_init(&core1_ctrl_queue, sizeof(pwm_ctrl_msg_t), 8);
    queue_init(&core1_state_queue, sizeof(uint32_t), 8);
    queue_init(&schedule_error_queue, sizeof(uint8_t), 2);

#if defined(DEBUG) || defined(PROFILE_CPU)
#warning "Initializing printf from UART will slow down core1 main loop."
    stdio_uart_init_full(DEBUG_UART, 921600, DEBUG_UART_TX_PIN, -1);
#endif
/* // Punt this for later release.
    multicore_reset_core1();
    (void)multicore_fifo_pop_blocking(); // Wait until core1 is ready.
    multicore_launch_core1(core1_main);
*/
    reset_app(); // Setup GPIO states. Get scheduler ready.
    // Loop forever.
    while(true)
        app.run();
}
