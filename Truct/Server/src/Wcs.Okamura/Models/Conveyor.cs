namespace Wcs.Okamura.Models;

using Wcs.Okamura.Enums;

public record ConveyorCommand(
    ConveyorCommandType Type,
    ConveyorDirection Direction = ConveyorDirection.Forward,
    string? Reason = null,
    int? CorrelationId = null
);

public record ConveyorStatus(
    ConveyorRunState RunState,
    ConveyorDirection Direction,
    bool IsHealthy,
    bool HasPalletPresent,     // nếu bạn có sensor “pallet present”
    string? Message = null
);

public record ConveyorCommandResult(
    bool Success,
    ConveyorRunState FinalState,
    string? ErrorMessage = null
);