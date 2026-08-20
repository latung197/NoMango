using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Mappers;

public static class StationMapper
{
    public static Station ToDomain(this StationDbModel dbModel)
    {
        var station = new Station
        {
            Id = dbModel.Id,
            Code = dbModel.Code,
            Sizes = SizeCsvExtensions.ParseSizes(dbModel.Sizes),
            StageCode = dbModel.StageCode,
            Type = dbModel.Type,
            Tag_HasCassette = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.HasCassette.Value)?.Tag,
            Tag_Status = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.Status.Value)?.Tag,
            Tag_Control = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.Control.Value)?.Tag,
            Tag_QRCode = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.QRCode.Value)?.Tag,
            HasCurtain = dbModel.HasCurtain,
            Tag_CurtainState = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.CurtainState.Value)?.Tag,
            HasConveyor = dbModel.HasConveyor,
            Tag_ConveyorState = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.ConveyorState.Value)?.Tag,
            Tag_ConveyorNumber = dbModel.Tags.FirstOrDefault(t => t.TagType.Value == StationTag.ConveyorNumber.Value)?.Tag,
            HasWarehouseDoor = dbModel.HasWarehouseDoor,
            IsAuxiliaryMaterial = dbModel.IsAuxiliaryMaterial,
            Capacity = dbModel.Capacity,
            MainPoint = dbModel.MainPoint,
            WaitingPoint = dbModel.WaitingPoint,
            AutoReceiveEnabled = dbModel.AutoReceiveEnabled,
            AutoSendEnabled = dbModel.AutoSendEnabled,
            AutoSendIsEmptyTray = dbModel.AutoSendIsEmptyTray,
            AutoSendToStage = dbModel.AutoSendToStage,
            IsActive = dbModel.IsActive,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt
        };
        return station;
    }

    public static StationDbModel ToDbModel(this Station domain)
    {
        var stationDbModel = new StationDbModel
        {
            Id = domain.Id,
            Code = domain.Code,
            Sizes = SizeCsvExtensions.ToSizesCsv(domain.Sizes),
            StageCode = domain.StageCode,
            Type = domain.Type,
            HasCurtain = domain.HasCurtain,
            HasConveyor = domain.HasConveyor,
            HasWarehouseDoor = domain.HasWarehouseDoor,
            IsAuxiliaryMaterial = domain.IsAuxiliaryMaterial,
            Capacity = domain.Capacity,
            MainPoint = domain.MainPoint,
            WaitingPoint = domain.WaitingPoint,
            AutoReceiveEnabled = domain.AutoReceiveEnabled,
            AutoSendEnabled = domain.AutoSendEnabled,
            AutoSendIsEmptyTray = domain.AutoSendIsEmptyTray,
            AutoSendToStage = domain.AutoSendToStage,
            IsActive = domain.IsActive,
            UpdatedAt = domain.UpdatedAt,
        };

        if (!string.IsNullOrEmpty(domain.Tag_HasCassette))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.HasCassette, Tag = domain.Tag_HasCassette });
        }
        if (!string.IsNullOrEmpty(domain.Tag_Status))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.Status, Tag = domain.Tag_Status });
        }
        if (!string.IsNullOrEmpty(domain.Tag_Control))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.Control, Tag = domain.Tag_Control });
        }
        if (!string.IsNullOrEmpty(domain.Tag_QRCode))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.QRCode, Tag = domain.Tag_QRCode });
        }
        if (!string.IsNullOrEmpty(domain.Tag_CurtainState))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.CurtainState, Tag = domain.Tag_CurtainState });
        }
        if (!string.IsNullOrEmpty(domain.Tag_ConveyorState))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.ConveyorState, Tag = domain.Tag_ConveyorState });
        }
        if (!string.IsNullOrEmpty(domain.Tag_ConveyorNumber))
        {
            stationDbModel.Tags.Add(new StationTagDbModel { TagType = StationTag.ConveyorNumber, Tag = domain.Tag_ConveyorNumber });
        }
        return stationDbModel;
    }
}
