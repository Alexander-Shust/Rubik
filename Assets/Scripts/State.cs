public static class State
{
    public static readonly int[] ReadyState =
    {
        (int) Edges.UF, (int) Edges.UR, (int) Edges.UB, (int) Edges.UL, 
        (int) Edges.DF, (int) Edges.DR, (int) Edges.DB, (int) Edges.DL, 
        (int) Edges.FR, (int) Edges.FL, (int) Edges.BR, (int) Edges.BL, 
        (int) Corners.UFR, (int) Corners.URB, (int) Corners.UBL, (int) Corners.ULF, 
        (int) Corners.DRF, (int) Corners.DFL, (int) Corners.DLB, (int) Corners.DBR,
        (int) EO.Good, (int) EO.Good, (int) EO.Good, (int) EO.Good, 
        (int) EO.Good, (int) EO.Good, (int) EO.Good, (int) EO.Good, 
        (int) EO.Good, (int) EO.Good, (int) EO.Good, (int) EO.Good, 
        (int) CO.Normal, (int) CO.Normal, (int) CO.Normal, (int) CO.Normal, 
        (int) CO.Normal, (int) CO.Normal, (int) CO.Normal, (int) CO.Normal
    };
}

public enum Phases
{
    Eo = 1,
    Co = 2,
    Htr = 3,
    Final = 4
}

public enum Faces
{
    U = 0,
    D = 1,
    F = 2,
    B = 3,
    L = 4,
    R = 5
}

public enum Edges
{
    UF = 0,
    UR = 1,
    UB = 2,
    UL = 3,
    DF = 4,
    DR = 5,
    DB = 6,
    DL = 7,
    FR = 8,
    FL = 9,
    BR = 10,
    BL = 11
}

public enum Corners
{
    UFR = 12,
    URB = 13,
    UBL = 14,
    ULF = 15,
    DRF = 16,
    DFL = 17,
    DLB = 18,
    DBR = 19
}

public enum EO
{
    Good = 0,
    Bad = 1
}

public enum CO
{
    Normal = 0,
    CW = 1,
    CCW = 2
}