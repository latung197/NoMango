using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Extensions;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.ValueObjects;
using Wcs.Common.Exceptions;

namespace Wcs.Cms.Services;

public class StationService(IStationRepository stationRepository)
{
    private readonly IStationRepository _stationRepository = stationRepository;

    public async Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stations = await _stationRepository.GetAllAsync(cancellationToken);
        return stations;
    }

    public async Task<IEnumerable<Station>> GetListAsync(StationListRequest request, CancellationToken cancellationToken = default)
    {
        var stations = await _stationRepository.GetListAsync(request.PageNumber, request.PageSize, request.Code, request.StageCode, request.Size, cancellationToken);
        return stations;
    }

    public async Task<int> CountAsync(StationListRequest request, CancellationToken cancellationToken = default)
    {
        return await _stationRepository.CountAsync(request.Code, request.StageCode, request.Size, cancellationToken);
    }

    public async Task<Station> ShowStationAsync(string id, CancellationToken cancellationToken = default)
    {
        var station = await _stationRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Station với id {id} không tồn tại");
        return station;
    }

    public async Task<Station> CreateStationAsync(StationCreateRequest request, CancellationToken cancellationToken = default)
    {
        var sizes = ParseAndValidateSizes(request.Sizes);
        var station = new Station(
            code: request.Code.Trim(),
            stageCode: request.StageCode,
            type: (InOut)request.Type,
            sizes: sizes,
            tag_HasCassette: request.TagHasCassette?.Trim(),
            tag_Status: request.TagStatus?.Trim(),
            tag_Control: request.TagControl?.Trim(),
            tag_QRCode: request.TagQRCode?.Trim(),
            hasCurtain: request.HasCurtain,
            tag_CurtainState: request.TagCurtainState?.Trim(),
            hasConveyor: request.HasConveyor,
            tag_ConveyorState: request.TagConveyorState?.Trim(),
            mainPoint: request.MainPoint.Trim(),
            waitingPoint: request.WaitingPoint.Trim()
        );
        
        station.HasWarehouseDoor = request.HasWarehouseDoor;
        station.IsAuxiliaryMaterial = request.IsAuxiliaryMaterial;
        station.Capacity = request.Capacity;
        station.Tag_ConveyorNumber = request.TagConveyorNumber;
        ApplyAutoOptions(
            station,
            request.AutoReceiveEnabled,
            request.AutoSendEnabled,
            request.AutoSendIsEmptyTray,
            request.AutoSendToStage);
        
        station = await _stationRepository.CreateAsync(station, cancellationToken);
        return station;
    }

    public async Task<Station> UpdateStationAsync(string id, StationUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var station = await _stationRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Station với id {id} không tồn tại");
        
        station.Code = request.Code.Trim();
        station.StageCode = request.StageCode;
        station.Type = (InOut)request.Type;
        station.Sizes = ParseAndValidateSizes(request.Sizes);
        station.Tag_HasCassette = request.TagHasCassette?.Trim();
        station.Tag_Status = request.TagStatus?.Trim();
        station.Tag_Control = request.TagControl?.Trim();
        station.Tag_QRCode = request.TagQRCode?.Trim();
        station.HasCurtain = request.HasCurtain;
        station.Tag_CurtainState = request.TagCurtainState?.Trim();
        station.HasConveyor = request.HasConveyor;
        station.Tag_ConveyorState = request.TagConveyorState?.Trim();
        station.Tag_ConveyorNumber = request.TagConveyorNumber?.Trim();
        station.HasWarehouseDoor = request.HasWarehouseDoor;
        station.IsAuxiliaryMaterial = request.IsAuxiliaryMaterial;
        station.Capacity = request.Capacity;
        station.MainPoint = request.MainPoint.Trim();
        station.WaitingPoint = request.WaitingPoint.Trim();
        ApplyAutoOptions(
            station,
            request.AutoReceiveEnabled,
            request.AutoSendEnabled,
            request.AutoSendIsEmptyTray,
            request.AutoSendToStage);
        
        await _stationRepository.UpdateAsync(station, cancellationToken);
        return station;
    }

    public async Task DeleteStationAsync(string id, CancellationToken cancellationToken = default)
    {
        var station = await _stationRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Station với id {id} không tồn tại");
        
        station.Deactivate();
        station.UpdatedAt = DateTime.UtcNow;
        await _stationRepository.UpdateAsync(station, cancellationToken);
    }

    private static List<Size> ParseAndValidateSizes(int[] sizes)
    {
        if (sizes is null || sizes.Length == 0)
        {
            throw new InvalidOperationException("Phải chọn ít nhất một size");
        }

        var parsed = sizes
            .Distinct()
            .Select(s =>
            {
                if (!Enum.IsDefined(typeof(Size), s))
                {
                    throw new InvalidOperationException($"Size không hợp lệ: {s}");
                }
                return (Size)s;
            })
            .OrderBy(s => (int)s)
            .ToList();

        return parsed;
    }

    private static void ApplyAutoOptions(
        Station station,
        bool autoReceiveEnabled,
        bool autoSendEnabled,
        bool autoSendIsEmptyTray,
        string? autoSendToStage)
    {
        station.AutoReceiveEnabled = false;
        station.AutoSendEnabled = false;
        station.AutoSendIsEmptyTray = false;
        station.AutoSendToStage = null;

        if (station.Type == InOut.IN)
        {
            station.AutoReceiveEnabled = autoReceiveEnabled;
            return;
        }

        if (station.Type == InOut.OUT)
        {
            station.AutoSendEnabled = autoSendEnabled;
            station.AutoSendIsEmptyTray = autoSendIsEmptyTray;
            station.AutoSendToStage = autoSendEnabled ? autoSendToStage?.Trim() : null;
        }
    }
}
