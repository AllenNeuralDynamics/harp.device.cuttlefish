#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import WriteU8HarpMessage, WriteU8ArrayMessage
from pyharp.messages import MessageType
from pyharp.messages import CommonRegisters as Regs
from struct import pack, unpack
import logging
import os
from time import sleep, perf_counter
from app_registers import AppRegs

#logging.basicConfig(level=logging.DEBUG)


# Open the device and print the info on screen
# Open serial connection and save communication to a file
if os.name == 'posix': # check for Linux.
    #device = Device("/dev/harp_device_00", "ibl.bin")
    device = Device("/dev/ttyACM0", "ibl.bin")
else: # assume Windows.
    device = Device("COM95", "ibl.bin")

# Provision device with a square wave.
# offset_us, on_duration_us, off_duration_us, cycles (0 = loop forever), invert
settings_sequence = \
(
    (0, 500, 500, 10, False),
    (0, 1000, 1000, 10, False)

)
data_fmt = "<LLLLB"

# Validate that we can alter settings that apply to the same pin while the
# schedule is not running.
# Confirm the results with a logic analyzer that the pwm settings are different.
for settings in settings_sequence:
    print("Configuring device with PWM task.")
    reply = device.send(WriteU8ArrayMessage(AppRegs.PWMSettings0,
                                            data_fmt, settings).frame)
    print(reply)
    assert reply.message_type == MessageType.WRITE
    print()
    print("Enabling task.")
    reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, int(True)).frame)
    print(reply)
    assert reply.message_type == MessageType.WRITE
    print()
    sleep(0.5)
    print("Disabling schedule.")
    reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, 0).frame)
    print(reply)
    assert reply.message_type == MessageType.WRITE
    print()
