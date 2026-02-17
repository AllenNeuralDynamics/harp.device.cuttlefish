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

print("Configuring TTL pin 0, 1, and 2 as output.")
device.send(WriteU8HarpMessage(AppRegs.PortDir, int(0x07)).frame)
print("Configuring RISING interrupts on pins 0, 1, 2.")
device.send(WriteU8HarpMessage(AppRegs.RisingEdgeEvents, int(0x07)).frame)
sleep(0.5)
for i in range(3):
    print("Writing: 0x01", end = " ")
    reply = device.send(WriteU8HarpMessage(AppRegs.PortState, int(0x01)).frame)
    print(f" Read back: {hex(reply.payload[0])}")
    for event_msg in device.get_events():
        print(event_msg)
    sleep(0.5)
    print("Writing: 0x00", end=" ")
    reply = device.send(WriteU8HarpMessage(AppRegs.PortState, int(0x00)).frame)
    print(f" Read back: {hex(reply.payload[0])}")
    sleep(0.5)
