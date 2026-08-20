using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;

namespace Wcs.Infrastructure.Data.Mappers;

public static class MaterialRequestMapper
{
    public static MaterialRequest ToDomain(this MaterialRequestDbModel dbModel)
    {
        return new MaterialRequest
        {
            Id = dbModel.Id,
            RequestId = dbModel.RequestId,
            MaterialId = dbModel.MaterialId,
            MaterialName = dbModel.Material != null ? dbModel.Material.Name : string.Empty,
            MaterialCode = dbModel.Material != null ? dbModel.Material.Code : string.Empty,
            MaterialUnit = dbModel.Material != null ? dbModel.Material.Unit : string.Empty,
            Quantity = dbModel.Quantity,
            Actual = dbModel.Actual,
            Note = dbModel.Note,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt.AddHours(7),
            UpdatedAt = dbModel.UpdatedAt.AddHours(7)
        };
    }

    public static MaterialRequestDbModel ToDbModel(this MaterialRequest domain)
    {
        return new MaterialRequestDbModel
        {
            Id = domain.Id,
            RequestId = domain.RequestId,
            MaterialId = domain.MaterialId,
            Quantity = domain.Quantity,
            Actual = domain.Actual,
            Note = domain.Note,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
