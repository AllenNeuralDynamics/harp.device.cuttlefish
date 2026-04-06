#ifndef CONFIG_H
#define CONFIG_H

inline constexpr size_t NUM_GPIOS = 8;
inline constexpr size_t PORT_BASE = 8;
inline constexpr size_t PORT_DIR_BASE = 16;

#define DEBUG_UART (uart0)
#define SYNC_UART (uart1)
inline constexpr size_t LED0 = 24;
inline constexpr size_t LED1 = 25;

inline constexpr size_t HARP_DEVICE_ID = 0x057B;
inline constexpr size_t DEBUG_UART_TX_PIN = 0;

inline constexpr size_t HARP_SYNC_RX_PIN = 5;

inline constexpr size_t HW_VERSION_MAJOR = 1;
inline constexpr size_t HW_VERSION_MINOR = 0;
inline constexpr size_t HW_ASSEMBLY_VERSION = 0;

inline constexpr size_t FW_VERSION_MAJOR = 0;
inline constexpr size_t FW_VERSION_MINOR = 0;
inline constexpr size_t FW_VERSION_PATCH = 1;

inline constexpr size_t UNUSED_SERIAL_NUMBER = 0; // Deprecated in favor of R_UUID




#endif // CONFIG_H
