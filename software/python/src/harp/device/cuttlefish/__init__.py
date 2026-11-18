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
    RegisterU8,
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
    "PwmSettingsPayload",
    "AnalogPwmSettingsPayload",
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
    "AnalogPwmSettings0",
    "AnalogPwmSettings1",
    "AnalogPwmSettings2",
    "AnalogPwmSettings3",
    "AnalogPwmSettings4",
    "AnalogPwmSettings5",
    "AnalogPwmSettings6",
    "AnalogPwmSettings7",
    "ScheduleStatus",
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
    """Represents the PinDirection payload."""

    __value__: Pins = BitMask(enum=Pins)


class PinStatePayload(AnonymousPayload[np.uint8]):
    """Represents the PinState payload."""

    __value__: Pins = BitMask(enum=Pins)


class PinSetPayload(AnonymousPayload[np.uint8]):
    """Represents the PinSet payload."""

    __value__: Pins = BitMask(enum=Pins)


class PinClearPayload(AnonymousPayload[np.uint8]):
    """Represents the PinClear payload."""

    __value__: Pins = BitMask(enum=Pins)


class EnableRisingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the EnableRisingEdgeEvents payload."""

    __value__: Pins = BitMask(enum=Pins)


class RisingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the RisingEdgeEvents payload."""

    __value__: Pins = BitMask(enum=Pins)


class EnableFallingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the EnableFallingEdgeEvents payload."""

    __value__: Pins = BitMask(enum=Pins)


class FallingEdgeEventsPayload(AnonymousPayload[np.uint8]):
    """Represents the FallingEdgeEvents payload."""

    __value__: Pins = BitMask(enum=Pins)


class PwmStatePayload(AnonymousPayload[np.uint8]):
    """Represents the PwmState payload."""

    __value__: EnableFlag = GroupMask(enum=EnableFlag, mask=0xFF)


class PwmSettingsPayload(StructPayload[np.uint8], length=17):
    """Represents the PwmSettings payload."""

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


class AnalogPwmSettingsPayload(StructPayload[np.uint8], length=25):
    """Represents the AnalogPwmSettings payload."""

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

    analog_frequency_hz: np.float32 = Field(IdentityConverter(np.float32), offset=17)
    """Frequency (in Hz) of the high-frequency PWM output produced while the nominal output is HIGH. Valid range: 10Hz to 1MHz (recommended: 1KHz to 100KHz)."""

    analog_duty_cycle: np.float32 = Field(IdentityConverter(np.float32), offset=21)
    """Duty cycle (0.0 to 1.0) of the high-frequency PWM output produced while the nominal output is HIGH."""


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
    """Enable Events from the RisingEdgeEvents register for the specified pins in the mask when any of the corresponding pins transitions from logic LOW to logic HIGH."""

    address: ClassVar[int] = 36
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = EnableRisingEdgeEventsPayload


class RisingEdgeEvents(RegisterBase[Pins]):
    """Event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableRisingEdgeEvents register transitions from logic LOW to logic HIGH. For pins configured with PwmSettings or AnalogPwmSettings, events are produced from the scheduled output changes (for AnalogPwmSettings, the nominal output rather than every edge of the high-frequency output)."""

    address: ClassVar[int] = 37
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = RisingEdgeEventsPayload


class EnableFallingEdgeEvents(RegisterBase[Pins]):
    """Enable Events from the FallingEdgeEvents register for the specified pins in the mask when any of the corresponding pins transitions from logic HIGH to logic LOW."""

    address: ClassVar[int] = 38
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = EnableFallingEdgeEventsPayload


class FallingEdgeEvents(RegisterBase[Pins]):
    """Event Only. Returns a timestamped message with the Port state when any of the pins specified in the EnableFallingEdgeEvents register transitions from logic HIGH to logic LOW. For pins configured with PwmSettings or AnalogPwmSettings, events are produced from the scheduled output changes (for AnalogPwmSettings, the nominal output rather than every edge of the high-frequency output)."""

    address: ClassVar[int] = 39
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = FallingEdgeEventsPayload


class PwmState(RegisterBase[EnableFlag]):
    """Write a nonzero value to this register to start the PWM schedule. Write zero to stop the schedule. Receive an event with payload=0 when the pwm schedule has finished."""

    address: ClassVar[int] = 40
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmStatePayload


class PwmSettings0(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 0."""

    address: ClassVar[int] = 41
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings1(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 1."""

    address: ClassVar[int] = 42
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings2(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 2."""

    address: ClassVar[int] = 43
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings3(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 3."""

    address: ClassVar[int] = 44
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings4(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 4."""

    address: ClassVar[int] = 45
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings5(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 5."""

    address: ClassVar[int] = 46
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings6(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 6."""

    address: ClassVar[int] = 47
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class PwmSettings7(RegisterBase[PwmSettingsPayload]):
    """Configure the settings of the PWM output on pin 7."""

    address: ClassVar[int] = 48
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = PwmSettingsPayload


class AnalogPwmSettings0(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 0. The output follows the same nominal waveform as PwmSettings0, but, while the nominal output is HIGH, the pin produces a high-frequency PWM signal with the specified frequency and duty cycle. Writing this register replaces any settings previously written to PwmSettings0 (and vice versa). Pins 0-1, 2-3, 4-5, and 6-7 share a hardware PWM peripheral; the analog outputs on a shared pair must use the same frequency."""

    address: ClassVar[int] = 49
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings1(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 1. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 50
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings2(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 2. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 51
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings3(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 3. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 52
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings4(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 4. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 53
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings5(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 5. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 54
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings6(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 6. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 55
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class AnalogPwmSettings7(RegisterBase[AnalogPwmSettingsPayload]):
    """Configure the settings of an analog PWM output on pin 7. See AnalogPwmSettings0 for details."""

    address: ClassVar[int] = 56
    payload_type: ClassVar[PayloadType] = PayloadType.U8
    payload_class = AnalogPwmSettingsPayload


class ScheduleStatus(RegisterU8):
    """Event Only. Emitted when the PWM schedule encounters an error while running. A value of 1 indicates a missed deadline, after which the schedule is aborted."""

    address: ClassVar[int] = 57


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
    49: AnalogPwmSettings0,
    50: AnalogPwmSettings1,
    51: AnalogPwmSettings2,
    52: AnalogPwmSettings3,
    53: AnalogPwmSettings4,
    54: AnalogPwmSettings5,
    55: AnalogPwmSettings6,
    56: AnalogPwmSettings7,
    57: ScheduleStatus,
}
