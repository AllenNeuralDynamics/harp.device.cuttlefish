"""App registers for the cuttlefish."""
from enum import IntEnum


class AppRegs(IntEnum):
    PortDir = 32
    PortState = 33
    PortSet = 34
    PortClear = 35

    EnableRisingEdgeEvents = 36
    EnableFallingEdgeEvents = 37
    RisingEdgeEvents = 38
    FallingEdgeEvents = 39

    PWMStart = 38
    PWMStop = 39

    PWMSettings0 = 40
    PWMSettings1 = 41
    PWMSettings2 = 42
    PWMSettings3 = 43
    PWMSettings4 = 44
    PWMSettings5 = 45
    PWMSettings6 = 46
    PWMSettings7 = 47
