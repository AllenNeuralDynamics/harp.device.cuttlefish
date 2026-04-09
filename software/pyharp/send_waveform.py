#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import WriteU8HarpMessage, WriteU8ArrayMessage
from pyharp.messages import MessageType
from pyharp.messages import CommonRegisters as Regs
from struct import pack, unpack
import logging
import os
from time import perf_counter
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
settings = \
(
    0,          # offset_us
    500,     # on_duration_us
    500,    # off_duration_us
    1000,          # cycles. (0 = repeat forever.)
    False       # invert.
)
data_fmt = "<LLLLB"

print("Configuring device with PWM task.")
reply = device.send(WriteU8ArrayMessage(AppRegs.PWMSettings0,
                                        data_fmt, settings).frame)
print(reply)
print()

print("Enabling schedule.")
reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, int(True)).frame)
print(reply)
print()

# Receive EVENT indicating that 1000 pulse sequence has finished.
start_time = perf_counter()
while (perf_counter() - start_time) < 3:
    for event_msg in device.get_events():
        print(event_msg)
        print()

# Send STOP just in case (although sequence should've already ended).
print("Disabling schedule.")
reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, 0).frame)
print(reply)
print()
