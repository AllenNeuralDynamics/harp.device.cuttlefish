"""Gate a 20 kHz, 25% duty cycle PWM output on pin 0 with a 10 Hz, 50% duty
cycle nominal PWM waveform until Enter is pressed."""

from cmath import exp

from harp.device import cuttlefish
from harp.serial import open_device

PORT = "/dev/ttyACM0"

LASER_SETTINGS_REGISTERS = [
    cuttlefish.AnalogPwmSettings0,
    cuttlefish.AnalogPwmSettings1,
    cuttlefish.AnalogPwmSettings2,
    cuttlefish.AnalogPwmSettings3
]
AGGREGATE_FREQUENCY_HZ = 60
DELTA_T1_US = 666
PWM_FREQUENCY_HZ = 20000


agg_interval_us = round(1/AGGREGATE_FREQUENCY_HZ * 1.0e6)
base_interval_us = agg_interval_us * len(LASER_SETTINGS_REGISTERS)
exposure_time_us = agg_interval_us - 2*DELTA_T1_US


def main() -> None:
    with open_device(cuttlefish, port=PORT) as dev:
        dev.write(cuttlefish.PwmState, False)
        for index, laser_reg in enumerate(LASER_SETTINGS_REGISTERS):
            settings = cuttlefish.AnalogPwmSettingsPayload(
                offset_us=index*agg_interval_us + DELTA_T1_US,
                on_duration_us=exposure_time_us,
                off_duration_us=base_interval_us - exposure_time_us - 2*DELTA_T1_US,
                cycles=0,  # Continue until stopped.
                invert=False,
                analog_frequency_hz=20000,
                analog_duty_cycle=0.25,
            )
            dev.write(laser_reg, settings)
        print("Starting analog PWM on pin 0. Press Enter to stop.")
        dev.write(cuttlefish.PwmState, True)
        try:
            input()
        finally:
            dev.write(cuttlefish.PwmState, False)
            print("Schedule stopped.")


if __name__ == "__main__":
    main()
