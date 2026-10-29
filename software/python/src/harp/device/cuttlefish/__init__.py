# This file was automatically generated and should not be edited directly.
# To make changes, edit the device metadata and regenerate the interface.

import enum
from typing import Any, ClassVar

import numpy as np
from harp.protocol import (
    AnonymousPayload,
    BitMask,
    BoolConverter,
    Field,
    GroupMask,
    IdentityConverter,
    PayloadType,
    RegisterBase,
    StructPayload,
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
    "PwmSettings0Payload",
    "PwmSettings1Payload",
    "PwmSettings2Payload",
    "PwmSettings3Payload",
    "PwmSettings4Payload",
    "PwmSettings5Payload",
    "PwmSettings6Payload",
    "PwmSettings7Payload",
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


class PwmSettings0Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings0 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings1Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings1 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings2Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings2 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings3Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings3 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings4Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings4 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings5Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings5 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings6Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings6 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


class PwmSettings7Payload(StructPayload[np.uint8], length=17):
    """Represents the payload of the PwmSettings7 register."""

    offset_us: np.uint32 = Field(IdentityConverter(np.uint32))
    """How long (in microseconds) the output remains LOW before switching HIGH in one period."""

    on_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=4)
    """How long (in microseconds) the output remains HIGH in one period."""

    off_duration_us: np.uint32 = Field(IdentityConverter(np.uint32), offset=8)
    """How long (in microseconds) the output remains LOW in one period."""

    cycles: np.uint32 = Field(IdentityConverter(np.uint32), offset=12)
    """How many pulses to produce, or zero to pulse until disabled."""

    invert: bool = Field(BoolConverter(), offset=16)
    """Whether the output is inverted (on-time refers to the output being LOW instead)."""


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


class PwmSettings0(RegisterBase[PwmSettings0Payload]):
    """Configure the settings of the PWM output on pin 0."""

    address: ClassVar[int] = 41
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings0Payload


class PwmSettings1(RegisterBase[PwmSettings1Payload]):
    """Configure the settings of the PWM output on pin 1."""

    address: ClassVar[int] = 42
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings1Payload


class PwmSettings2(RegisterBase[PwmSettings2Payload]):
    """Configure the settings of the PWM output on pin 2."""

    address: ClassVar[int] = 43
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings2Payload


class PwmSettings3(RegisterBase[PwmSettings3Payload]):
    """Configure the settings of the PWM output on pin 3."""

    address: ClassVar[int] = 44
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings3Payload


class PwmSettings4(RegisterBase[PwmSettings4Payload]):
    """Configure the settings of the PWM output on pin 4."""

    address: ClassVar[int] = 45
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings4Payload


class PwmSettings5(RegisterBase[PwmSettings5Payload]):
    """Configure the settings of the PWM output on pin 5."""

    address: ClassVar[int] = 46
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings5Payload


class PwmSettings6(RegisterBase[PwmSettings6Payload]):
    """Configure the settings of the PWM output on pin 6."""

    address: ClassVar[int] = 47
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings6Payload


class PwmSettings7(RegisterBase[PwmSettings7Payload]):
    """Configure the settings of the PWM output on pin 7."""

    address: ClassVar[int] = 48
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettings7Payload


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
