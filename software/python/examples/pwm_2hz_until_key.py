"""Run a 2 Hz PWM on pin 1 and print all events until Enter is pressed."""

from harp.device import cuttlefish
from harp.device import core
from harp.protocol import HarpMessage
from harp.serial import open_device

PORT = "COM4"
PWM_PERIOD_US = 500_000  # 2 Hz.
INPUT_PIN = cuttlefish.Pins.PIN2


def on_event(msg: HarpMessage[cuttlefish.Pins]) -> None:
    print(f"State of IO2: {bool(msg.payload & INPUT_PIN)}")


def main() -> None:
    with open_device(cuttlefish, port=PORT) as dev:
        opreg = dev.write(
            core.OperationControl,
            core.OperationControlPayload(
                dump_registers=False,
                mute_replies=False,
                visual_indicators=True,
                operation_led=True,
                heartbeat=True,
                operation_mode=core.OperationMode.ACTIVE,
            ),
        )
        dev.write(core.OperationControl, opreg.payload)
        settings = cuttlefish.PwmSettings1Payload(
            offset_us=0,
            on_duration_us=PWM_PERIOD_US // 2,
            off_duration_us=PWM_PERIOD_US // 2,
            cycles=0,  # Continue until stopped.
            invert=False,
        )
        dev.write(cuttlefish.PwmSettings1, settings)
        dev.write(cuttlefish.EnableRisingEdgeEvents, INPUT_PIN)
        print(f"Starting a continuous 2 Hz PWM on pin 1 ({cuttlefish.Pins.PIN1}).")
        print("Printing all device events. Press Enter to stop.")
        # with dev.subscribe(register=cuttlefish.PinState, handler=on_event):
        with dev.subscribe_all(print):
            dev.write(cuttlefish.PwmState, True)
            try:
                input()
            finally:
                dev.write(cuttlefish.PwmState, False)
                print("PWM stopped.")


if __name__ == "__main__":
    main()
