using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;

namespace Wcs.Infrastructure.Data.Mappers;

public static class FlowMapper
{
    public static Flow ToDomain(this FlowDbModel dbModel)
    {
        var domain = new Flow
        {
            Id = dbModel.Id,
            Name = dbModel.Name,
            Area = dbModel.Area,
            Priority = dbModel.Priority,
            ReturnEmpty = dbModel.ReturnEmpty,
            Status = dbModel.Status,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt,
            Steps = dbModel.Steps
                        .Select(s => s.ToDomain())
                        .OrderBy(s => s.StepNo)
                        .ThenBy(s => s.Stage)
                        .ToList()
        };
        return domain;
    }

    public static FlowDbModel ToDbModel(this Flow domain)
    {
        var dbModel = new FlowDbModel
        {
            Id = domain.Id,
            Name = domain.Name,
            Area = domain.Area,
            Priority = domain.Priority,
            ReturnEmpty = domain.ReturnEmpty,
            Status = domain.Status,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            Steps = domain.Steps.Select(s => s.ToDbModel()).ToList()
        };
        return dbModel;
    }
}
