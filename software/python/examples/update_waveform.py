"""Reconfigure PWM settings on the same pin between runs while the schedule is
stopped, confirming settings can be changed freely when not running."""

import argparse
from time import sleep

from harp.device import cuttlefish
from harp.protocol import MessageType
from harp.serial import open_device

DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--port", default=DEFAULT_PORT, help="Serial port of the device."
    )
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        # offset_us, on_duration_us, off_duration_us, cycles (0 = loop forever), invert
        settings_sequence = (
            (0, 500, 500, 10, False),
            (0, 1000, 1000, 10, False),
        )

        # Confirm with a logic analyzer that the pwm settings differ between runs.
        for (
            offset_us,
            on_duration_us,
            off_duration_us,
            cycles,
            invert,
        ) in settings_sequence:
            settings = cuttlefish.PwmSettings0Payload(
                offset_us=offset_us,
                on_duration_us=on_duration_us,
                off_duration_us=off_duration_us,
                cycles=cycles,
                invert=invert,
            )
            print("Configuring device with PWM task.")
            reply = dev.write(cuttlefish.PwmSettings0, settings)
            print(reply)
            assert reply.message_type == MessageType.Write
            print()

            print("Enabling task.")
            reply = dev.write(cuttlefish.PwmState, True)
            print(reply)
            assert reply.message_type == MessageType.Write
            print()
            sleep(0.5)

            print("Disabling schedule.")
            reply = dev.write(cuttlefish.PwmState, False)
            print(reply)
            assert reply.message_type == MessageType.Write
            print()


if __name__ == "__main__":
    main()
