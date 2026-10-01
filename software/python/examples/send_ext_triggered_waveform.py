"""Start a PWM waveform on pin 0 when a rising edge is detected on pin 3.

Note: earlier firmware exposed dedicated "arm external trigger" registers.
The current device schema has no such registers, so triggering
is instead performed in software: we subscribe to RisingEdgeEvents and start
the PWM schedule from the event handler as soon as a rising edge on pin 3 is
reported.
"""

import argparse
from time import sleep

from harp.device import cuttlefish
from harp.protocol import HarpMessage
from harp.serial import open_device

TRIGGER_PIN = cuttlefish.Pins.PIN3
OUTPUT_PIN = cuttlefish.Pins.PIN7


DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", default=DEFAULT_PORT, help="Serial port of the device.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        settings = cuttlefish.PwmSettings0Payload(
            offset_us=0,
            on_duration_us=500_000,
            off_duration_us=500_000,
            cycles=0,  # 0 = loop forever.
            invert=False,
        )

        print("Configuring device with PWM task on pin 0.")
        dev.write(cuttlefish.PwmSettings0, settings)

        print("Setting pins 0-6 as inputs. Setting pin 7 to output and HIGH.")
        dev.write(cuttlefish.PinDirection, OUTPUT_PIN)
        dev.write(cuttlefish.PinState, OUTPUT_PIN)

        print("Arming software trigger from input on pin 3 on RISING edge.")
        dev.write(cuttlefish.EnableRisingEdgeEvents, TRIGGER_PIN)

        def on_rising_edge(msg: HarpMessage) -> None:
            print(msg)
            if cuttlefish.Pins(msg.payload) & TRIGGER_PIN:
                print("External trigger seen on pin 3. Starting PWM schedule.")
                dev.write(cuttlefish.PwmState, True)

        print("Waiting to see external trigger.")
        try:
            with dev.subscribe(cuttlefish.RisingEdgeEvents, on_rising_edge):
                while True:
                    sleep(0.1)
        except KeyboardInterrupt:
            print("Disabling outputs.")
            dev.write(cuttlefish.PinState, cuttlefish.Pins(0))
            print("Disabling task.")
            dev.write(cuttlefish.PwmState, False)
            print("Disabling rising edge events.")
            dev.write(cuttlefish.EnableRisingEdgeEvents, cuttlefish.Pins(0))


if __name__ == "__main__":
    main()
