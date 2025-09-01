using System;

[Flags]
public enum BeamColor {
    None = 0,
    R = 1,
    G = 2,
    B = 4,
    White = R|G|B
}
