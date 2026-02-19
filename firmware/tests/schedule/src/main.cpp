#include <pwm_settings.h>
#include <array>
#include <pico/stdlib.h>
#include <pwm_scheduler.h>
#include <pwm_settings.h>
#include "hardware/clocks.h"
#include <hardware/clocks.h>



__not_in_flash("scheduler") PWMScheduler scheduler;

std::array<pwm_settings_t, 8> pwm_settings
{{{0, 500, 500, 12, 0},     // offset, on time, off time, cycles, invert
  {75, 500, 500, 12, 0},
  {150, 500, 500, 12, 0},
  {450, 500, 500, 12, 0},
  {0, 500, 500, 0, 0},
  {0, 500, 500, 0, 0},
  {0, 500, 500, 0, 0},
  {0, 500, 500, 0, 0}
}};

//{{{0, 5000, 5000, 9, 0},     // offset, on time, off time, cycles, invert
//  {0, 5000, 5000, 9, 0},
//  {0, 5000, 5000, 9, 0},
//  {0, 5000, 5000, 9, 0},
//  {0, 5000, 5000, 9, 0},
//  {0, 5000, 5000, 9, 0},
//  {0, 5000, 5000, 9, 0},
//  {0, 5000, 5000, 9, 0}
//}};

// Did not work. 50us between initial schedule time is too fast?
//{{{0, 500, 500, 0, 0},     // offset, on time, off time, cycles, invert
//  {50, 500, 500, 0, 0},
//  {350, 500, 500, 0, 0},
//  {450, 500, 500, 0, 0},
//  {0, 500, 500, 0, 0},
//  {0, 500, 500, 0, 0},
//  {0, 500, 500, 0, 0},
//  {0, 500, 500, 0, 0}
//}};

// Did not work.
//{{{0, 1000, 1000, 0, 0},     // offset, on time, off time, cycles, invert
//  {0, 1000, 1000, 0, 0},
//  {0, 2000, 2000, 0, 0},
//  {0, 3000, 3000, 0, 0},
//  {0, 5000, 5000, 0, 0},
//  {0, 3333, 3333, 0, 0},
//  {0, 3333, 6666, 0, 0},
//  {0, 1500, 1500, 0, 0}
//}};


int main()
{
    //set_sys_clock_khz(200000, true); // No overclocking for now.

    stdio_init_all();
    while (!stdio_usb_connected()){ sleep_ms(100);} // Wait for user to open com port.
    printf("Hello, from the CuTTLefish testbench!\r\n");

    // Populate the schedule
    uint32_t pin_mask = 1u << 8;
    for (const auto& settings: pwm_settings)
    {
        // one task per pin.
        scheduler.schedule_pwm_task(settings.offset_us,
                                    settings.on_duration_us,
                                    settings.period_us(),
                                    pin_mask,
                                    settings.cycles,
                                    bool(settings.invert));
        pin_mask <<= 1;
    }
    sleep_ms(100);

    scheduler.start();
    while (true)
        scheduler.update();
}
