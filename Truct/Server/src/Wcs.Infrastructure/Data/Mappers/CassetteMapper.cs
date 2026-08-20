using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;

namespace Wcs.Infrastructure.Data.Mappers;

public static class CassetteMapper
{
    public static Cassette ToDomain(this CassetteDbModel dbModel)
    {
        return new Cassette
        {
            Id = dbModel.Id,
            Name = dbModel.Name,
            Code = dbModel.Code,
            Product = dbModel.Product,
            Size = dbModel.Size,
            Capacity = dbModel.Capacity,
            Quantity = dbModel.Quantity,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt
        };
    }

    public static CassetteDbModel ToDbModel(this Cassette domain)
    {
        return new CassetteDbModel
        {
            Id = domain.Id,
            Name = domain.Name,
            Code = domain.Code,
            Product = domain.Product,
            Size = domain.Size,
            Capacity = domain.Capacity,
            Quantity = domain.Quantity,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
