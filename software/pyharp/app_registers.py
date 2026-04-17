"""App registers for the cuttlefish."""
from enum import IntEnum


class AppRegs(IntEnum):
    PortDir = 32
    PortState = 33
    PortSet = 34
    PortClear = 35

    EnableRisingEdgeEvents = 36
    RisingEdgeEvents = 37
    EnableFallingEdgeEvents = 38
    FallingEdgeEvents = 39

    PWMState = 40

    PWMSettings0 = 41
    PWMSettings1 = 42
    PWMSettings2 = 43
    PWMSettings3 = 44
    PWMSettings4 = 45
    PWMSettings5 = 46
    PWMSettings6 = 47
    PWMSettings7 = 48
