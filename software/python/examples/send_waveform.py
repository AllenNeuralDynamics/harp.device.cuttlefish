"""Run a single finite PWM sequence, printing the completion event."""

import argparse
from time import perf_counter, sleep

from harp.device import cuttlefish
from harp.protocol import HarpMessage
from harp.serial import open_device

DEFAULT_PORT = "/dev/ttyACM0"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--port", default=DEFAULT_PORT, help="Serial port of the device."
    )
    return parser.parse_args()


def on_pwm_state_event(msg: HarpMessage) -> None:
    print(msg)


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        settings = cuttlefish.PwmSettings0Payload(
            offset_us=0,
            on_duration_us=500,
            off_duration_us=500,
            cycles=1000,  # 0 = loop forever.
            invert=False,
        )

        print("Configuring device with PWM task.")
        reply = dev.write(cuttlefish.PwmSettings0, settings)
        print(reply)
        print()

        with dev.subscribe(cuttlefish.PwmState, on_pwm_state_event):
            print("Enabling schedule.")
            reply = dev.write(cuttlefish.PwmState, True)
            print(reply)
            print()

            # Receive EVENT indicating that the 1000 pulse sequence has finished.
            start_time = perf_counter()
            while (perf_counter() - start_time) < 3:
                sleep(0.05)

        # Send STOP just in case (although the sequence should've already ended).
        print("Disabling schedule.")
        reply = dev.write(cuttlefish.PwmState, False)
        print(reply)
        print()


if __name__ == "__main__":
    main()
