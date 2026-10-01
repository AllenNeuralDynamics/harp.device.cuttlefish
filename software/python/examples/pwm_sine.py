"""Sweep the duty cycle of IO0's PWM output along a sine wave.

The firmware only accepts new PWM settings while the schedule is stopped, so each
step of the sine stops the schedule, loads a new duty cycle, and starts it again.
"""

from math import pi, sin
from pathlib import Path
from time import sleep

from harp import serial
from harp.device.core import EnableFlag
from harp.device.schema import create_device_module

PORT = "COM95"  # Adjust to the serial port of your board.
PWM_PERIOD_US = 1000  # 1 kHz carrier.
STEPS_PER_SINE = 50  # Duty cycle values per sine period.
STEP_S = 0.04  # Time spent at each duty cycle value (sine period = 2 s).
N_SINES = 5

# Build the device interface straight from the schema in this repository.
DEVICE_YML = Path(__file__).resolve().parents[3] / "device.yml"
cuttlefish = create_device_module(DEVICE_YML.read_text())


def duty_to_settings(duty: float):
    """Convert a duty cycle between 0 and 1 to a PWM settings payload."""
    # Keep both phases at least 1 us long so the output keeps pulsing.
    on_us = min(max(round(duty * PWM_PERIOD_US), 1), PWM_PERIOD_US - 1)
    return cuttlefish.PwmSettings0Payload(
        offset_us=0,
        on_duration_us=on_us,
        off_duration_us=PWM_PERIOD_US - on_us,
        cycles=0,  # Pulse until stopped.
        invert=False,
    )


def sine_duty(step: int) -> float:
    return 0.5 + 0.5 * sin(2 * pi * step / STEPS_PER_SINE)


# Leaving the `with` block disconnects from the board.
with serial.open_device(cuttlefish, port=PORT) as device:
    for _ in range(N_SINES):
        for step in range(STEPS_PER_SINE):
            device.write(cuttlefish.PwmSettings0, duty_to_settings(sine_duty(step)))
            device.write(cuttlefish.PwmState, EnableFlag.ENABLED)  # Start PWM0
            sleep(STEP_S)
            device.write(cuttlefish.PwmState, EnableFlag.DISABLED)  # Stop PWM0
