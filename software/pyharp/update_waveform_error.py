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
settings = (0, 500, 500, 0, False)
data_fmt = "<LLLLB"

# Validate that attempting to alter pwm settings *while the schedule is running*
# will throw a write error.
print("Configuring device with PWM task.")
reply = device.send(WriteU8ArrayMessage(AppRegs.PWMSettings0,
                                        data_fmt, settings).frame)
assert reply.message_type == MessageType.WRITE
print("Enabling task.")
reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, int(True)).frame)
assert reply.message_type == MessageType.WRITE
sleep(0.5)
# Try to alter settings while the schedule is running. This should throw
# an WRITE_ERROR
print("Trying to change settings while schedule is running.", end=" ")
reply = device.send(WriteU8ArrayMessage(AppRegs.PWMSettings0,
                                        data_fmt, settings).frame)
assert reply.message_type == MessageType.WRITE_ERROR, \
    """Error: the device did not throw a WRITE_ERROR when we attemp to change
       settings while the schedule is running"""
print("Device rejected settings. OK!")
sleep(0.5)
print("Disabling schedule.")
reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, 0).frame)
assert reply.message_type == MessageType.WRITE
print()
