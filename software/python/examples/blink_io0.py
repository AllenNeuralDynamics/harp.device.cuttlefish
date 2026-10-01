"""Toggle IO0 on and off 10 times, 500 ms apart."""

from time import sleep

from harp import serial
from harp.device import core, cuttlefish

PORT = "COM95"  # Adjust to the serial port of your board.
N_BLINKS = 10
PERIOD_S = 0.5

# Leaving the `with` block disconnects from the board.
with serial.open_device(cuttlefish, port=PORT) as device:
    # Configure IO0 as an output
    response = device.read(core.WhoAmI)
    print(response.payload)
    print(device.read(core.DeviceName).payload)

    device.write(cuttlefish.PinDirection, cuttlefish.Pins.PIN0)

    for _ in range(N_BLINKS):
        device.write(cuttlefish.PinSet, cuttlefish.Pins.PIN0)  # IO0 high
        sleep(PERIOD_S)
        device.write(cuttlefish.PinClear, cuttlefish.Pins.PIN0)  # IO0 low
        sleep(PERIOD_S)
