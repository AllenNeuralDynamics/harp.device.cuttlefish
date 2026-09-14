#ifndef CONFIG_H
#define CONFIG_H
#include "core_registers.h" // for semver_t

inline constexpr size_t NUM_GPIOS = 8;
inline constexpr size_t PORT_BASE = 8;
inline constexpr size_t PORT_DIR_BASE = 16;
inline constexpr size_t PORT_MASK = 0x000000FF << PORT_BASE;
inline constexpr size_t PORT_DIR_MASK = 0x000000FF << PORT_DIR_BASE;

#define DEBUG_UART (uart0)
#define SYNC_UART (uart1)
inline constexpr uint32_t LED0 = 24;
inline constexpr uint32_t HARP_CORE_LED_PIN = 25;

inline constexpr semver_t FW_VERSION = {0, 1, 0};
inline constexpr semver_t HW_VERSION = {1, 0, 0};

inline constexpr size_t HARP_DEVICE_ID = 1403;
inline constexpr size_t DEBUG_UART_TX_PIN = 0;

inline constexpr size_t HARP_SYNC_RX_PIN = 5;

inline constexpr size_t UNUSED_SERIAL_NUMBER = 0; // Deprecated in favor of R_UUID

inline constexpr size_t INTERCORE_COM_TIMEOUT_US = 1000;



#endif // CONFIG_H
