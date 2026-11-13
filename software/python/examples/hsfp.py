"""Configure PWM tasks for hyperspectral photometry."""

import argparse
from time import sleep

from harp.device import cuttlefish
from harp.serial import open_device
from math import ceil


DEFAULT_PORT = "/dev/ttyACM0"

CAMERA_FPS = 150
CMOS_IDLE_TIME_US = 600
CAMERA_TRIGGER_SETTINGS_REGISTER = cuttlefish.PwmSettings0
LASER_SETTINGS_REGISTERS = (
    cuttlefish.PwmSettings1,
    cuttlefish.PwmSettings2,
    cuttlefish.PwmSettings3,
    cuttlefish.PwmSettings4,
    cuttlefish.PwmSettings5)


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--port", default=DEFAULT_PORT, help="Serial port of the device."
    )
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    with open_device(cuttlefish, port=args.port) as dev:
        camera_period_us = ceil(1/150*1e6)
        laser_period_us = camera_period_us * len(LASER_SETTINGS_REGISTERS)
        print(f"Configuring Camera Trigger Settings")
        camera_on_duration_us = camera_period_us - CMOS_IDLE_TIME_US
        reply = dev.write(
            CAMERA_TRIGGER_SETTINGS_REGISTER,
            cuttlefish.PwmSettingsPayload(
                offset_us=0,
                on_duration_us=camera_on_duration_us,
                off_duration_us=CMOS_IDLE_TIME_US,
                cycles=0,
                invert=False))

        for i, settings_reg in enumerate(LASER_SETTINGS_REGISTERS):
            print(f"Configuring Laser Settings on {settings_reg}")
            laser_on_duration_us = camera_period_us - CMOS_IDLE_TIME_US
            reply = dev.write(settings_reg,
                cuttlefish.PwmSettingsPayload(
                    offset_us=i*camera_period_us,
                    on_duration_us=laser_on_duration_us,
                    off_duration_us=laser_period_us - laser_on_duration_us,
                    cycles=0,
                    invert=False))

        sleep(1)

        print("Enabling schedule.")
        reply = dev.write(cuttlefish.PwmState, True)
        print(reply)
        print()
        sleep(10)

        print("Disabling schedule.")
        reply = dev.write(cuttlefish.PwmState, False)
        print(reply)


if __name__ == "__main__":
    main()
