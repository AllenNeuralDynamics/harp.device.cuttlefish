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
# offset_us, on_time_us, period_us, port_mask, cycles, invert.
pwm_task_settings = (
    (0, 500, 500,     0, False),
    (0, 1000, 1500,   0, False),
    (0, 350, 350,     0, False),
#    (0, 5000, 5000,   0, False),
#    (0, 10000, 10000, 0, False),
#    (0, 7500, 7500,   0, False),
#    (0, 2000, 2000,   0, False),
#    (0, 50000, 50000, 0, False)
)
data_fmt = "<LLLLB"

print("Configuring device with PWM task.")
for index, settings in enumerate(pwm_task_settings):
    reply = device.send(WriteU8ArrayMessage(AppRegs.PWMSettings0 + index,
                                            data_fmt, settings).frame)
    print(reply)
    print()

print("Enabling many tasks.")
reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, 1).frame)
print(reply)
print()
sleep(3)

print("Disabling all tasks.")
reply = device.send(WriteU8HarpMessage(AppRegs.PWMState, 0).frame)
print(reply)
