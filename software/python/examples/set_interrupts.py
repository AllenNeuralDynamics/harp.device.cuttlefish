"""Enable rising-edge events on pins 0-2 and toggle pin 0 while printing events."""

import argparse
from time import sleep

from harp.device import cuttlefish
from harp.protocol import HarpMessage
from harp.serial import open_device


DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", default=DEFAULT_PORT, help="Serial port of the device.")
    return parser.parse_args()


def on_rising_edge(msg: HarpMessage) -> None:
    print(msg)


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        print("Configuring TTL pins 0, 1, and 2 as outputs.")
        dev.write(cuttlefish.PinDirection, cuttlefish.Pins(0x07))
        print("Configuring RISING interrupts on pins 0, 1, 2.")
        dev.write(cuttlefish.EnableRisingEdgeEvents, cuttlefish.Pins(0x07))
        sleep(0.5)
        with dev.subscribe(cuttlefish.RisingEdgeEvents, on_rising_edge):
            for _ in range(3):
                print("Writing: 0x01", end=" ")
                reply = dev.write(cuttlefish.PinState, cuttlefish.Pins(0x01))
                print(f" Read back: {hex(int(reply.payload))}")
                sleep(0.5)
                print("Writing: 0x00", end=" ")
                reply = dev.write(cuttlefish.PinState, cuttlefish.Pins(0x00))
                print(f" Read back: {hex(int(reply.payload))}")
                sleep(0.5)


if __name__ == "__main__":
    main()
