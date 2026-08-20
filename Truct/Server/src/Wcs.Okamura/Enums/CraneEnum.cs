namespace Wcs.Okamura.Enums;

public enum CraneTaskType
{
    None = 0,
    Inbound = 1,
    Outbound = 2,
    InternalMove = 3,
    StationMove = 4
}

// D2054
public enum CraneSpecialErrorType
{
    None = 0,
    NoItemAtPick = 1,
    PutPositionOccupied = 2,
    BlockedBehindPick = 3,
    BlockedBehindPut = 4
}

// D2050
public enum TaskExecuteFeedback
{
    None = 0,
    Accepted = 1,
    InstructionError = 2
}

// D2053
public enum TaskCompleteStatus
{
    None = 0,
    AutoCompleted = 1,
    ManualCompleted = 2,
    Canceled = 3
}

// D2059
public enum CraneStatus
{
    Auto = 1,
    Manual = 2,
    Online = 3,
    Maintenance = 4
}