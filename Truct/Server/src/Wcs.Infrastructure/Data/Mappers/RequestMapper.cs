using Wcs.Common.Entities;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Data.Mappers;

public static class RequestMapper
{
    public static Request ToDomain(this RequestDbModel dbModel, Stage? stage = null, Station[]? station = null, FlowTask? flowTasks = null)
    {
        var domain = new Request
        {
            Id = dbModel.Id,
            FlowTaskId = dbModel.FlowTaskId,
            Code = dbModel.Code,
            //Stage = stage,
            StageCode = dbModel.StageCode,
            DeliveryCode = dbModel.DeliveryCode,
            DeliveryOrder = dbModel.DeliveryOrder,
            CurrentStep = dbModel.CurrentStep,
            Note = dbModel.Note,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt.AddHours(7),
            UpdatedAt = dbModel.UpdatedAt.AddHours(7),
            Materials = dbModel.MaterialRequests
                        .Select(s => s.ToDomain())
                        .OrderBy(s => s.Quantity)
                        .ThenBy(s => s.Actual)
                        .ToList()
        };

        if (stage != null)
            domain.Stage = stage;
        if (station != null)
            domain.Station = station;
        if (flowTasks != null)
            domain.FlowTask = flowTasks;

        return domain;
    }

    public static Request ToDomain(this RequestDbModel dbModel)
    {
        return new Request
        {
            Id = dbModel.Id,
            FlowTaskId = dbModel.FlowTaskId,
            Code = dbModel.Code,
            //Stage = dbModel.StageCode,
            StageCode = dbModel.StageCode,
            DeliveryCode = dbModel.DeliveryCode,
            DeliveryOrder = dbModel.DeliveryOrder,
            CurrentStep = dbModel.CurrentStep,
            Note = dbModel.Note,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt.AddHours(7),
            UpdatedAt = dbModel.UpdatedAt.AddHours(7),
            Materials = dbModel.MaterialRequests
                        .Select(s => s.ToDomain())
                        .OrderBy(s => s.Quantity)
                        .ThenBy(s => s.Actual)
                        .ToList()
        };
    }

    public static RequestDbModel ToDbModel(this Request domain)
    {
        var dbModel = new RequestDbModel
        {
            Id = domain.Id,
            Code = domain.Code,
            StageCode = domain.StageCode,
            DeliveryCode = domain.DeliveryCode,
            DeliveryOrder = domain.DeliveryOrder,
            CurrentStep = domain.CurrentStep,
            Note = domain.Note,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            MaterialRequests = domain.Materials.Select(s => s.ToDbModel()).ToList()
        };
        if (domain.FlowTaskId != null)
        {
            dbModel.FlowTaskId = domain.FlowTaskId;
        }
        return dbModel;
    }
}
