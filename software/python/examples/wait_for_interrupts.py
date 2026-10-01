"""Enable rising-edge events on pin 0, run a 3-pulse PWM task, and print events
until interrupted."""

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


def on_event(msg: HarpMessage) -> None:
    print(msg)


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        print("Configuring RisingEdge interrupts on pin 0.")
        dev.write(cuttlefish.EnableRisingEdgeEvents, cuttlefish.Pins(0x01))
        print("Setting up a 3 pulse PWM task on pin 0.")
        settings = cuttlefish.PwmSettings0Payload(
            offset_us=0,
            on_duration_us=500_000,
            off_duration_us=500_000,
            cycles=3,
            invert=False,
        )
        dev.write(cuttlefish.PwmSettings0, settings)
        print("Starting pulse sequence.")
        print()
        with dev.subscribe(cuttlefish.RisingEdgeEvents, on_event), \
                dev.subscribe(cuttlefish.PwmState, on_event):
            dev.write(cuttlefish.PwmState, True)
            sleep(0.1)
            try:
                while True:
                    sleep(0.1)
            except KeyboardInterrupt:
                pass
            finally:
                print("Disabling all rising edge events.")
                dev.write(cuttlefish.EnableRisingEdgeEvents, cuttlefish.Pins(0x00))


if __name__ == "__main__":
    main()
