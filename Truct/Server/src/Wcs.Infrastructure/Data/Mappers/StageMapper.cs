using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;

namespace Wcs.Infrastructure.Data.Mappers;

public static class StageMapper
{
    public static Stage ToDomain(this StageDbModel dbModel)
    {
        return new Stage(dbModel.Id, dbModel.Name, dbModel.Code, dbModel.Area, dbModel.Distance)
        {
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt
        };
    }

    public static StageDbModel ToDbModel(this Stage domain)
    {
        return new StageDbModel 
        { 
            Id = domain.Id, 
            Name = domain.Name,
            Code = domain.Code, 
            Area = domain.Area,
            Distance = domain.Distance,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}