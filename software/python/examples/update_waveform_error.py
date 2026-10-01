"""Verify that changing PWM settings while the schedule is running is rejected."""

import argparse
from time import sleep

from harp.device import cuttlefish
from harp.protocol import MessageType
from harp.serial import open_device


DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--port", default=DEFAULT_PORT, help="Serial port of the device.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    # Disable automatic error-raising so we can inspect the WRITE-with-error reply.
    with open_device(cuttlefish, port=args.port, raise_on_error=False) as dev:
        settings = cuttlefish.PwmSettings0Payload(
            offset_us=0,
            on_duration_us=500,
            off_duration_us=500,
            cycles=0,  # 0 = loop forever.
            invert=False,
        )

        print("Configuring device with PWM task.")
        reply = dev.write(cuttlefish.PwmSettings0, settings)
        assert reply.message_type == MessageType.Write and not reply.has_error

        print("Enabling task.")
        reply = dev.write(cuttlefish.PwmState, True)
        assert reply.message_type == MessageType.Write and not reply.has_error
        sleep(0.5)

        print("Trying to change settings while schedule is running.", end=" ")
        reply = dev.write(cuttlefish.PwmSettings0, settings)
        assert reply.message_type == MessageType.Write and reply.has_error, (
            "Error: the device did not report an error when we attempted to change "
            "settings while the schedule is running"
        )
        print("Device rejected settings. OK!")
        sleep(0.5)

        print("Disabling schedule.")
        reply = dev.write(cuttlefish.PwmState, False)
        assert reply.message_type == MessageType.Write and not reply.has_error
        print()


if __name__ == "__main__":
    main()
