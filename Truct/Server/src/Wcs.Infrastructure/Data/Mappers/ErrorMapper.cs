using Wcs.Common.Entities;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Data.Mappers;

public static class ErrorMapper
{
    public static Error ToDomain(this ErrorDbModel dbModel)
    {
        TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        return new Error(dbModel.Id, dbModel.Code)
        {
            //Id = dbModel.Id,
            Code = dbModel.Code,
            Message = dbModel.Message,
            Status = dbModel.Status,
            ErrorType = dbModel.ErrorType,
            //StackTrace = dbModel.StackTrace,
            //ActionRequired = dbModel.ActionRequired,
            //ErrorDate = dbModel.ErrorDate,
            //ResolvedDate = dbModel.ResolvedDate,
            //IsResolved = dbModel.IsResolved,
            //Area = string.IsNullOrWhiteSpace(dbModel.Area) ? null : StageArea.FromString(dbModel.Area),
            Area = dbModel.Area,
            FlowTaskId = dbModel.FlowTaskId,
            FromStation = dbModel.FromStation,
            ToStation = dbModel.ToStation,
            //Stage = dbModel.Stage,
            //Station = dbModel.Station,
            CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(dbModel.CreatedAt, vietnamZone),
            UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(dbModel.UpdatedAt, vietnamZone)
        };
    }

    public static ErrorDbModel ToDbModel(this Error domain)
    {
        return new ErrorDbModel
        { 
            Id = domain.Id,
            Code = domain.Code,
            Message = domain.Message,
            Status = domain.Status,
            ErrorType = domain.ErrorType,
            Area = domain.Area?.ToString(),
            FlowTaskId = domain.FlowTaskId,
            FromStation = domain.FromStation,
            ToStation = domain.ToStation,
            //Stage = domain.Stage,
            //Station = domain.Station,
            //IsResolved = false,
            //IsResolved = domain.IsResolved,
            //IsResolved = domain != null ? domain.IsResolved : false,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}