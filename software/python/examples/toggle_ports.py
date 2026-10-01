"""Configure all pins as outputs and toggle them HIGH/LOW a few times."""

import argparse
from time import sleep

from harp.device import cuttlefish
from harp.serial import open_device


DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", default=DEFAULT_PORT, help="Serial port of the device.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        all_pins = cuttlefish.Pins(0xFF)
        print("Configuring TTL pins 0, 1, 2, 3, 4, 5, 6, 7 as outputs.")
        dev.write(cuttlefish.PinDirection, all_pins)
        sleep(1)
        for _ in range(3):
            print("Writing: 0xFF", end=" ")
            reply = dev.write(cuttlefish.PinState, all_pins)
            print(f" Read back: {hex(int(reply.payload))}")
            sleep(0.5)
            print("Writing: 0x00", end=" ")
            reply = dev.write(cuttlefish.PinState, cuttlefish.Pins(0))
            print(f" Read back: {hex(int(reply.payload))}")
            sleep(0.5)


if __name__ == "__main__":
    main()
