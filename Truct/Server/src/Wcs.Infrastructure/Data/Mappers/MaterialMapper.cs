using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;

namespace Wcs.Infrastructure.Data.Mappers;

public static class MaterialMapper
{
    public static Material ToDomain(this MaterialDbModel dbModel)
    {
        return new Material
        {
            Id = dbModel.Id,
            Name = dbModel.Name,
            Code = dbModel.Code,
            Unit = dbModel.Unit,
            Note = dbModel.Note,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt.AddHours(7),
            UpdatedAt = dbModel.UpdatedAt.AddHours(7)
        };
    }

    public static MaterialDbModel ToDbModel(this Material domain)
    {
        return new MaterialDbModel
        {
            Id = domain.Id,
            Name = domain.Name,
            Code = domain.Code,
            Unit = domain.Unit,
            Note = domain.Note,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
