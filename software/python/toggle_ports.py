#!/usr/bin/env -S uv run --script
# /// script
# requires-python = ">=3.11"
# dependencies = ["harp"]
# ///
"""Configure all pins as outputs and toggle them HIGH/LOW a few times."""

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
        all_pins = device.Pins(0xFF)
        print("Configuring TTL pins 0, 1, 2, 3, 4, 5, 6, 7 as outputs.")
        dev.write(device.PinDirection, all_pins)
        sleep(1)
        for _ in range(3):
            print("Writing: 0xFF", end=" ")
            reply = dev.write(device.PinState, all_pins)
            print(f" Read back: {hex(int(reply.payload))}")
            sleep(0.5)
            print("Writing: 0x00", end=" ")
            reply = dev.write(device.PinState, device.Pins(0))
            print(f" Read back: {hex(int(reply.payload))}")
            sleep(0.5)


if __name__ == "__main__":
    main()
