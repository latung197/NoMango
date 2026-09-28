namespace Core.Application.Features.Erp.Organization;

public sealed record UnitDto(string Code, string Name, bool IsActive, int SortOrder);
public sealed record PlantDto(string Code, string UnitCode, string Name, bool IsActive, int SortOrder);
public sealed record SaveUnitRequest(string Code, string Name, bool IsActive, int SortOrder);
public sealed record SavePlantRequest(string Code, string UnitCode, string Name, bool IsActive, int SortOrder);
public sealed record SetUserPlantsRequest(IReadOnlyList<string> PlantCodes);

public interface IOrganizationService
{
    Task<IReadOnlyList<UnitDto>> GetUnitsAsync(CancellationToken ct);
    Task<IReadOnlyList<PlantDto>> GetPlantsAsync(CancellationToken ct);
    Task SaveUnitAsync(SaveUnitRequest request, CancellationToken ct);
    Task SavePlantAsync(SavePlantRequest request, CancellationToken ct);
    Task<IReadOnlyList<PlantDto>> GetUserPlantsAsync(int userId, CancellationToken ct);
    Task SetUserPlantsAsync(int userId, IReadOnlyList<string> plantCodes, CancellationToken ct);
}
