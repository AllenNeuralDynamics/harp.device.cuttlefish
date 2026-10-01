# This file was automatically generated and should not be edited directly.
# To make changes, edit the device metadata and regenerate the interface.

import enum
from typing import Any, ClassVar

import numpy as np
from harp.protocol import (
    AnonymousPayload,
    BitMask,
    GroupMask,
    PayloadType,
    RegisterBase,
    RegisterU8Array,
)
from harp.device.core import (
    EnableFlag,
    REGISTER_MAP as _CORE_REGISTER_MAP,
)


__all__ = [
    "DEVICE_NAME",
    "WHO_AM_I",
    "Pins",
    "PinDirectionPayload",
    "PinStatePayload",
    "PinSetPayload",
    "PinClearPayload",
    "EnableRisingEdgeEventsPayload",
    "RisingEdgeEventsPayload",
    "EnableFallingEdgeEventsPayload",
    "FallingEdgeEventsPayload",
    "PwmStatePayload",
    "PinDirection",
    "PinState",
    "PinSet",
    "PinClear",
    "EnableRisingEdgeEvents",
    "RisingEdgeEvents",
    "EnableFallingEdgeEvents",
    "FallingEdgeEvents",
    "PwmState",
    "PwmSettings0",
    "PwmSettings1",
    "PwmSettings2",
    "PwmSettings3",
    "PwmSettings4",
    "PwmSettings5",
    "PwmSettings6",
    "PwmSettings7",
    "REGISTER_MAP",
]

DEVICE_NAME: str = "Cuttlefish"
WHO_AM_I: int = 1403


class Pins(enum.IntFlag):
    """Available pins on the device"""

    PIN0 = 0x1
    PIN1 = 0x2
    PIN2 = 0x4
    PIN3 = 0x8
    PIN4 = 0x10
    PIN5 = 0x20
    PIN6 = 0x40
    PIN7 = 0x80


class PinDirectionPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the PinDirection register."""

    __value__: Pins = BitMask(enum=Pins)


class PinStatePayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the PinState register."""

    __value__: Pins = BitMask(enum=Pins)


class PinSetPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the PinSet register."""

    __value__: Pins = BitMask(enum=Pins)


class PinClearPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the PinClear register."""

    __value__: Pins = BitMask(enum=Pins)


class EnableRisingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the EnableRisingEdgeEvents register."""

    __value__: Pins = BitMask(enum=Pins)


class RisingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the RisingEdgeEvents register."""

    __value__: Pins = BitMask(enum=Pins)


class EnableFallingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the EnableFallingEdgeEvents register."""

    __value__: Pins = BitMask(enum=Pins)


class FallingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the FallingEdgeEvents register."""

    __value__: Pins = BitMask(enum=Pins)


class PwmStatePayload(AnonymousPayload[np.uint8]):
    """Represents the payload of the PwmState register."""

    __value__: EnableFlag = GroupMask(enum=EnableFlag, mask=0xFF)


class PinDirection(RegisterBase[Pins]):
    """Set the direction of the pins. 0 = input; 1 = output"""

    address: ClassVar[int] = 32
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PinDirectionPayload


class PinState(RegisterBase[Pins]):
    """Read or write the state of the pins."""

    address: ClassVar[int] = 33
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PinStatePayload


class PinSet(RegisterBase[Pins]):
    """Set pins specified in the mask to logic HIGH by setting the corresponding bit."""

    address: ClassVar[int] = 34
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PinSetPayload


class PinClear(RegisterBase[Pins]):
    """Set specified pins in the mask to logic LOW by setting the corresponding bit."""

    address: ClassVar[int] = 35
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PinClearPayload


class EnableRisingEdgeEvents(RegisterBase[Pins]):
    """Enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic LOW to logic HIGH."""

    address: ClassVar[int] = 36
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = EnableRisingEdgeEventsPayload


class RisingEdgeEvents(RegisterBase[Pins]):
    """Event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH."""

    address: ClassVar[int] = 37
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = RisingEdgeEventsPayload


class EnableFallingEdgeEvents(RegisterBase[Pins]):
    """Enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the the corresponding pins transitions from logic HIGH to logic LOW."""

    address: ClassVar[int] = 38
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = EnableFallingEdgeEventsPayload


class FallingEdgeEvents(RegisterBase[Pins]):
    """Event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic HIGH to logic LOW."""

    address: ClassVar[int] = 39
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = FallingEdgeEventsPayload


class PwmState(RegisterBase[EnableFlag]):
    """Write a nonzero value to this register to start the PWM schedule. Write zero to stop the schedule. Receive an event with payload=0 when the pwm schedule has finished."""

    address: ClassVar[int] = 40
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmStatePayload


class PwmSettings0(RegisterU8Array):
    """Struct to configure PWM0 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 41
    length: int = 17


class PwmSettings1(RegisterU8Array):
    """Struct to configure PWM1 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 42
    length: int = 17


class PwmSettings2(RegisterU8Array):
    """Struct to configure PWM2 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 43
    length: int = 17


class PwmSettings3(RegisterU8Array):
    """Struct to configure PWM3 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 44
    length: int = 17


class PwmSettings4(RegisterU8Array):
    """Struct to configure PWM4 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 45
    length: int = 17


class PwmSettings5(RegisterU8Array):
    """Struct to configure PWM5 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 46
    length: int = 17


class PwmSettings6(RegisterU8Array):
    """Struct to configure PWM6 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 47
    length: int = 17


class PwmSettings7(RegisterU8Array):
    """Struct to configure PWM7 settings: offset_us (U32), on_duration_us (U32), off_duration_us (U32), cycles (U32), invert (U8)"""

    address: ClassVar[int] = 48
    length: int = 17


REGISTER_MAP: dict[int, type[RegisterBase[Any]]] = {
    **_CORE_REGISTER_MAP,
    32: PinDirection,
    33: PinState,
    34: PinSet,
    35: PinClear,
    36: EnableRisingEdgeEvents,
    37: RisingEdgeEvents,
    38: EnableFallingEdgeEvents,
    39: FallingEdgeEvents,
    40: PwmState,
    41: PwmSettings0,
    42: PwmSettings1,
    43: PwmSettings2,
    44: PwmSettings3,
    45: PwmSettings4,
    46: PwmSettings5,
    47: PwmSettings6,
    48: PwmSettings7,
}
