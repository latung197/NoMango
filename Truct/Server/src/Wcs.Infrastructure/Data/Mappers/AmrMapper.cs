using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Data.Mappers;

public static class AmrMapper
{
    public static Amr ToDomain(this AmrDbModel dbModel)
    {
        return new Amr(dbModel.Id, dbModel.Name, dbModel.Code, dbModel.Area)
        {
            MapCode = dbModel.MapCode,
            RobotStatus = dbModel.RobotStatus,
            TypeCode = dbModel.TypeCode,
            Battery = dbModel.Battery,
            Direction = dbModel.Direction,
            Exclude = dbModel.Exclude,
            ExcludeStr = dbModel.ExcludeStr,
            OnLine = dbModel.OnLine,
            PodCode = dbModel.PodCode,
            PodDir = dbModel.PodDir,
            PosX = dbModel.PosX,
            PosY = dbModel.PosY,
            Ip = dbModel.Ip,
            Status = dbModel.Status,
            StatusStr = dbModel.StatusStr,
            Stop = dbModel.Stop,
            StopStr = dbModel.StopStr,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt
        };
    }

    public static AmrDbModel ToDbModel(this Amr domain)
    {
        return new AmrDbModel
        { 
            Id = domain.Id,
            Name = domain.Name,
            Code = domain.Code,
            Area = domain.Area ?? StageArea.Chip,
            MapCode = domain.MapCode,
            MapName = domain.MapName,
            RobotStatus = domain.RobotStatus,
            TypeCode = domain.TypeCode,
            Battery = domain.Battery,
            Direction = domain.Direction,
            Exclude = domain.Exclude,
            ExcludeStr = domain.ExcludeStr,
            OnLine = domain.OnLine,
            PodCode = domain.PodCode,
            PodDir = domain.PodDir,
            PosX = domain.PosX,
            PosY = domain.PosY,
            Ip = domain.Ip,
            Status = domain.Status,
            StatusStr = domain.StatusStr,
            Stop = domain.Stop,
            StopStr = domain.StopStr,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}