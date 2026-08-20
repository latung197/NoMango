namespace Wcs.Okamura.Models;

using Wcs.Okamura.Enums;

public record CurtainCommand(
    CurtainCommandType Type,
    string? Reason = null,
    int? CorrelationId = null
);

public record CurtainStatus(
    CurtainState State,
    bool IsHealthy,
    string? Message = null
);

public record CurtainCommandResult(
    bool Success,
    CurtainState FinalState,
    string? ErrorMessage = null
);