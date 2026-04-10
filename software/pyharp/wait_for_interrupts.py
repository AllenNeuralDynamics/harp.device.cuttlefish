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
    device = Device("/dev/ttyACM0", "ibl.bin")
else: # assume Windows.
    device = Device("COM95", "ibl.bin")

#print("Configuring all pins as inputs.")
#device.send(WriteU8HarpMessage(AppRegs.PortDir, int(0x00)).frame)
print("Configuring RisingEdge interrupts on pin 0.")
device.send(WriteU8HarpMessage(AppRegs.EnableRisingEdgeEvents, int(0x01)).frame)
print("Setting up a 3 pulse PWM task on pin 0")
settings = (0, 500000, 500000, 3, False)
data_fmt = "<LLLLB"
device.send(WriteU8ArrayMessage(AppRegs.PWMSettings0, data_fmt, settings).frame)
print("Starting pulse sequence.")
print()
device.send(WriteU8HarpMessage(AppRegs.PWMState, 1).frame)
sleep(0.1)
try:
    while (True):
        for event_msg in device.get_events():
            print(event_msg)
            print()
except KeyboardInterrupt:
    pass
finally:
    print("Disabling all rising edge events")
    device.send(WriteU8HarpMessage(AppRegs.EnableRisingEdgeEvents, int(0x00)).frame)

