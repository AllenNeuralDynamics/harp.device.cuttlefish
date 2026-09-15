#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.11"
# dependencies = ["harp"]
# ///
"""Configure three PWM tasks in sequence and run the schedule for a few seconds."""

import argparse
from time import sleep

import device
from harp.serial import open_device


DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", default=DEFAULT_PORT, help="Serial port of the device.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    with open_device(device, port=args.port) as dev:
        # (register, payload class, offset_us, on_duration_us, off_duration_us,
        #  cycles (0 = loop forever), invert)
        pwm_tasks = (
            (device.PwmSettings0, device.PwmSettings0Payload, 0, 500, 500, 0, False),
            (device.PwmSettings1, device.PwmSettings1Payload, 0, 1000, 1500, 100, False),  # Finite sequence!
            (device.PwmSettings2, device.PwmSettings2Payload, 0, 350, 350, 0, False),
        )

        print("Configuring device with PWM task.")
        for register, payload_cls, offset_us, on_duration_us, off_duration_us, cycles, invert in pwm_tasks:
            settings = payload_cls(
                offset_us=offset_us,
                on_duration_us=on_duration_us,
                off_duration_us=off_duration_us,
                cycles=cycles,
                invert=invert,
            )
            reply = dev.write(register, settings)
            print(reply)
            print()

        sleep(1)

        print("Enabling schedule.")
        reply = dev.write(device.PwmState, True)
        print(reply)
        print()
        sleep(3)

        print("Disabling schedule.")
        reply = dev.write(device.PwmState, False)
        print(reply)


if __name__ == "__main__":
    main()
