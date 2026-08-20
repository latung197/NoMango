using Wcs.Common.ValueObjects;

namespace Wcs.Api.Controllers.DTOs;

public record FlowTaskHistoryDto(
    Guid Id,
    string FlowTaskId,
    string? Event,
    FlowStep? Step,
    string? LogMessage,
    DateTime CreatedAt
);
