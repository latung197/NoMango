namespace Wcs.Common.ValueObjects;

public enum TransferRequestType
{
    Send = 1,
    Receive = 2,
}

public enum TransferRequestStatus
{
    Waiting = 0,
    Matched = 1,
    Cancelled = 2,
    TimedOut = 3,
    Completed = 4,
    Dispatching = 5,
}

public enum TransferRequestSource
{
    Manual = 0,
    AutoOpc = 1,
}

public enum WarehousePendingKind
{
    None = 0,
    Inbound = 1,
    Outbound = 2,
}
