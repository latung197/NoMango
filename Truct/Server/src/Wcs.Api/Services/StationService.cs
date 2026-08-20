using Wcs.Common.Abstractions;
using Wcs.Api.Configs;
using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;
using Wcs.OpcUa.Contracts;
using Wcs.Common.Abstractions.Repositories;
using Microsoft.IdentityModel.Tokens;
using Wcs.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Wcs.Okamura;

namespace Wcs.Api.Services;

public class StationService(IConfigService configService, ILogger<StationService> logger, IOpcUaClient opcUaClient, IStationRepository stationRepository, IServiceProvider serviceProvider)
{
    private readonly IConfigService _configService = configService;
    private readonly IOpcUaClient _opcUaClient = opcUaClient;
    private readonly ILogger<StationService> _logger = logger;
    private readonly IStationRepository _stationRepository = stationRepository;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<Station?> GetStationByCode(string code)
    {
        return await _stationRepository.GetByCodeAsync(code);
    }

    /// <summary>
    /// Load Station từ config theo code và size có cassette
    /// </summary>
    public async Task<Station?> GetHasCassetteStationByStageAndSize(string stageCode, string size)
    {
        var stations = await _stationRepository.GetByStageAndSizeAsync(stageCode, size);
        foreach (var station in stations)
        {
            if (await HasActiveTaskOnStationAsync(station.Code))
            {
                _logger.LogDebug(
                    "Bỏ qua station {StationCode}: đang có task active",
                    station.Code);
                continue;
            }

            if (!await IsStationPlcRunningAsync(station))
            {
                _logger.LogDebug(
                    "Bỏ qua station {StationCode}: Tag_Status != Running",
                    station.Code);
                continue;
            }

            if (station.HasConveyor && !string.IsNullOrEmpty(station.Tag_ConveyorNumber))
            {
                var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorNumber);
                var count = dataValue.Value.ToInt16OrDefault();
                if (count <= 0)
                {
                    _logger.LogDebug(
                        "Bỏ qua station {StationCode}: ConveyorNumber={Count} (cần > 0 để lấy hàng)",
                        station.Code,
                        count);
                    continue;
                }
            }

            if (!string.IsNullOrEmpty(station.Tag_HasCassette)) {
                var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_HasCassette);
                var hasCassette = dataValue.Value.ToInt16OrDefault();
                if (hasCassette == 0)
                {
                    _logger.LogDebug(
                        "Bỏ qua station {StationCode}: HasCassette={HasCassette} (cần > 0 để lấy hàng)",
                        station.Code,
                        hasCassette);
                    continue;
                }
            }

            return station;
        }
        return null;
    }

    /// <summary>
    /// Load Station từ config theo code và size không có cassette
    /// </summary>
    public async Task<Station?> GetEmptyStationByStageAndSize(string stageCode, string size)
    {
        var stations = await _stationRepository.GetByStageAndSizeAsync(stageCode, size);
        foreach (var station in stations)
        {
            if (await HasActiveTaskOnStationAsync(station.Code))
            {
                _logger.LogDebug(
                    "Bỏ qua station {StationCode}: đang có task active",
                    station.Code);
                continue;
            }

            if (!await IsStationPlcRunningAsync(station))
            {
                _logger.LogDebug(
                    "Bỏ qua station {StationCode}: Tag_Status != Running",
                    station.Code);
                continue;
            }

            if (station.HasConveyor && !string.IsNullOrEmpty(station.Tag_ConveyorNumber))
            {
                var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorNumber);
                var count = dataValue.Value.ToInt16OrDefault();
                if (count >= 5)
                {
                    _logger.LogDebug(
                        "Bỏ qua station {StationCode}: ConveyorNumber={Count} (cần < 5 để thả hàng)",
                        station.Code,
                        count);
                    continue;
                }
            }
            else if (!string.IsNullOrEmpty(station.Tag_HasCassette))
            {
                var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_HasCassette);
                var hasCassette = dataValue.Value.ToInt16OrDefault();
                if (hasCassette != (short)StationHasCassette.NoExist)
                {
                    _logger.LogDebug(
                        "Bỏ qua station {StationCode}: HasCassette={HasCassette} (kệ thường cần trống để thả hàng)",
                        station.Code,
                        hasCassette);
                    continue;
                }
            }

            return station;
        }
        return null;
    }

    public async Task<Station?> FindStationByTag(string tag)
    {
        return await _stationRepository.FindByTagAsync(tag);
    }

    /// <summary>
    /// Kiểm tra Tag_Status PLC của trạm. Trạm không cấu hình Tag_Status được coi là sẵn sàng.
    /// Chỉ Running (1) mới được phép tạo task.
    /// </summary>
    public async Task<bool> IsStationPlcRunningAsync(Station station)
    {
        if (string.IsNullOrEmpty(station.Tag_Status))
            return true;

        var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_Status);
        if (!Opc.Ua.StatusCode.IsGood(dataValue.StatusCode))
            return false;

        return dataValue.Value.ToInt16OrDefault() == (short)StationStatus.Running;
    }

    /// <summary>
    /// Trả về lý do trạm chưa sẵn sàng theo Tag_Status, hoặc null nếu OK.
    /// </summary>
    public async Task<string?> GetStationNotRunningReasonAsync(Station station)
    {
        if (string.IsNullOrEmpty(station.Tag_Status))
            return await GetWarehouseDoorNotReadyReasonAsync(station);

        var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_Status);
        if (!Opc.Ua.StatusCode.IsGood(dataValue.StatusCode))
        {
            return $"Trạm {station.Code} mất kết nối OPC (Tag_Status)";
        }

        var status = (StationStatus)dataValue.Value.ToInt16OrDefault();
        if (status != StationStatus.Running)
        {
            return status == StationStatus.Error
                ? $"Trạm {station.Code} đang báo lỗi (Tag_Status=Error)"
                : $"Trạm {station.Code} chưa sẵn sàng (Tag_Status={(int)status})";
        }

        return await GetWarehouseDoorNotReadyReasonAsync(station);
    }

    public async Task<string?> GetWarehouseDoorNotReadyReasonAsync(Station station)
    {
        if (!station.HasWarehouseDoor)
            return null;

        var tag = station.Type switch
        {
            InOut.IN => OpcTagsCvAgv.D2001,
            InOut.OUT => OpcTagsCvAgv.D2011,
            _ => null
        };

        if (tag is null)
            return null;

        var dataValue = await _opcUaClient.ReadValueAsync(tag);
        if (!Opc.Ua.StatusCode.IsGood(dataValue.StatusCode))
        {
            return $"Cua kho {station.Code} mat ket noi OPC ({tag})";
        }

        if (dataValue.Value.ToInt16OrDefault() != 1)
        {
            return $"Cua kho {station.Code} chua san sang ({tag}=0)";
        }

        if (station.Type == InOut.IN)
        {
            var hasBoxValue = await _opcUaClient.ReadValueAsync(OpcTagsCvAgv.D2028);
            if (!Opc.Ua.StatusCode.IsGood(hasBoxValue.StatusCode))
            {
                return $"Cua kho {station.Code} mat ket noi OPC ({OpcTagsCvAgv.D2028})";
            }

            if (hasBoxValue.Value.ToInt16OrDefault() != 0)
            {
                return $"Cua kho {station.Code} con hang tai cua nhap (D2028=1)";
            }
        }

        return null;
    }

    public async Task ValidateStationsReadyForTaskAsync(params Station[] stations)
    {
        foreach (var station in stations)
        {
            var reason = await GetStationNotRunningReasonAsync(station);
            if (reason != null)
                throw new StationNotAvailableException(reason);
        }
    }

    /// <summary>
    /// Đọc toàn bộ giá trị các tag OPC của một station theo code
    /// </summary>
    public async Task<StationOpcTagsResponse> GetStationOpcTagsAsync(Station station)
    {
        var tagMap = new Dictionary<string, string>(StringComparer.Ordinal);

        if (!string.IsNullOrEmpty(station.Tag_HasCassette))    tagMap["HasCassette"]    = station.Tag_HasCassette;
        if (!string.IsNullOrEmpty(station.Tag_Status))         tagMap["Status"]          = station.Tag_Status;
        if (!string.IsNullOrEmpty(station.Tag_Control))        tagMap["Control"]         = station.Tag_Control;
        if (!string.IsNullOrEmpty(station.Tag_QRCode))         tagMap["QRCode"]          = station.Tag_QRCode;
        if (!string.IsNullOrEmpty(station.Tag_CurtainState))   tagMap["CurtainState"]    = station.Tag_CurtainState;
        if (!string.IsNullOrEmpty(station.Tag_ConveyorState))  tagMap["ConveyorState"]   = station.Tag_ConveyorState;
        if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber)) tagMap["ConveyorNumber"]  = station.Tag_ConveyorNumber;

        if (tagMap.Count == 0)
            return new StationOpcTagsResponse { StationCode = station.Code, Tags = [] };

        var tagTypes = tagMap.Keys.ToList();
        var tagNames = tagMap.Values.ToList();

        var dataValues = await _opcUaClient.ReadMultipleValuesAsync(tagNames);

        var items = new List<StationOpcTagItem>(tagTypes.Count);
        for (int i = 0; i < tagTypes.Count && i < dataValues.Count; i++)
        {
            items.Add(new StationOpcTagItem
            {
                TagType   = tagTypes[i],
                TagName   = tagNames[i],
                Value     = dataValues[i].Value,
                Timestamp = dataValues[i].SourceTimestamp,
                IsGood    = Opc.Ua.StatusCode.IsGood(dataValues[i].StatusCode)
            });
        }

        return new StationOpcTagsResponse { StationCode = station.Code, Tags = items };
    }

    /// <summary>
    /// Load tất cả Stations từ config
    /// </summary>
    public async Task<IEnumerable<Station>> GetAllStations()
    {
        return await _stationRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Station>> GetFallbackStationsAsync(Station station, Size taskSize)
    {
        var warehouseInboundStations = await _stationRepository.GetWarehouseInboundStationsAsync();

        return warehouseInboundStations
            .Where(s => s.Id != station.Id
                && (s.Type == InOut.ALL || s.Type == InOut.IN)
                && s.SupportsSize(taskSize))
            .OrderBy(s => s.Code);
    }

    /// <summary>
    /// Trả về lý do trạm không thể nhận hàng (băng tải đầy hoặc kệ đã có khay), hoặc null nếu OK.
    /// </summary>
    public async Task<string?> GetStationCannotAcceptDropReasonAsync(Station station)
    {
        var warehouseDoorReason = await GetWarehouseDoorNotReadyReasonAsync(station);
        if (warehouseDoorReason != null)
            return warehouseDoorReason;

        if (station.HasWarehouseDoor && station.Type == InOut.IN)
            return null;

        if (station.HasConveyor)
        {
            if (!string.IsNullOrEmpty(station.Tag_ConveyorState))
            {
                var stateValue = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorState);
                if (!Opc.Ua.StatusCode.IsGood(stateValue.StatusCode))
                    return $"Trạm {station.Code} mất kết nối OPC (ConveyorState)";

                var stage = (ConveyorStage)stateValue.Value.ToInt16OrDefault();
                if (stage != ConveyorStage.Empty)
                {
                    return $"Trạm {station.Code} băng tải chưa trống (ConveyorState={(int)stage}, cần Empty=1)";
                }
            }

            if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber))
            {
                var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorNumber);
                if (!Opc.Ua.StatusCode.IsGood(dataValue.StatusCode))
                    return $"Trạm {station.Code} mất kết nối OPC (ConveyorNumber)";

                var count = dataValue.Value.ToInt16OrDefault();
                if (count >= ConveyorConstants.CapacityMax)
                {
                    return $"Trạm {station.Code} đã đầy hàng trên băng tải ({count}/{ConveyorConstants.CapacityMax})";
                }
            }

            return null;
        }

        if (!string.IsNullOrEmpty(station.Tag_HasCassette))
        {
            var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_HasCassette);
            if (!Opc.Ua.StatusCode.IsGood(dataValue.StatusCode))
                return $"Trạm {station.Code} mất kết nối OPC (HasCassette)";

            var hasCassette = dataValue.Value.ToInt16OrDefault();
            if (hasCassette != (short)StationHasCassette.NoExist)
            {
                return $"Trạm {station.Code} đã có khay, không còn chỗ trống";
            }
        }

        return null;
    }

    /// <summary>
    /// Ném lỗi nếu trạm không thể nhận thêm hàng (băng tải đầy 5/5 hoặc kệ đã có khay).
    /// </summary>
    public async Task ValidateStationCanAcceptDropAsync(Station station)
    {
        var reason = await GetStationCannotAcceptDropReasonAsync(station);
        if (reason != null)
            throw new InvalidOperationException(reason);
    }

    /// <summary>
    /// Kiểm tra trạm còn chỗ trống để hạ hàng.
    /// Trạm nhập kho (HasWarehouseDoor + Type=IN) luôn được coi là sẵn sàng vì tự động đưa hàng vào kho.
    /// </summary>
    public async Task<bool> IsStationAvailableForDropAsync(Station station)
    {
        if (!await IsStationPlcRunningAsync(station))
        {
            return false;
        }

        if (await GetWarehouseDoorNotReadyReasonAsync(station) != null)
        {
            return false;
        }

        if (station.HasWarehouseDoor && station.Type == InOut.IN)
        {
            return true;
        }

        if (station.HasConveyor)
        {
            if (!string.IsNullOrEmpty(station.Tag_ConveyorState))
            {
                var stateValue = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorState);
                var stage = (ConveyorStage)stateValue.Value.ToInt16OrDefault();
                if (stage != ConveyorStage.Empty)
                {
                    _logger.LogDebug(
                        "Trạm fallback {StationCode}: ConveyorState={Stage} (cần Empty=1 để thả hàng)",
                        station.Code, (int)stage);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber))
            {
                var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorNumber);
                var count = dataValue.Value.ToInt16OrDefault();
                if (count >= 5)
                {
                    _logger.LogDebug(
                        "Trạm fallback {StationCode}: ConveyorNumber={Count} (cần < 5 để thả hàng)",
                        station.Code, count);
                    return false;
                }
            }
        }
        else if (!string.IsNullOrEmpty(station.Tag_HasCassette))
        {
            var dataValue = await _opcUaClient.ReadValueAsync(station.Tag_HasCassette);
            var hasCassette = dataValue.Value.ToInt16OrDefault();
            if (hasCassette != (short)StationHasCassette.NoExist)
            {
                _logger.LogDebug(
                    "Trạm fallback {StationCode}: HasCassette={HasCassette} (cần trống để thả hàng)",
                    station.Code, hasCassette);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Xác định các requirements cần chờ dựa trên features của station
    /// </summary>
    public async Task<WaitingSet> GetWaitingRequirements(FlowStep step, Station station)
    {
        var requirements = new List<Type>();

        if (station.HasCurtain && !string.IsNullOrEmpty(station.Tag_CurtainState))
        {
            if (step == FlowStep.BeforePick || step == FlowStep.BeforeDrop)
            {
                // nếu rèm chưa mở thì cần chờ rèm mở
                var isIntrusion = await _opcUaClient.ReadValueAsync(station.Tag_CurtainState);
                if ((int)CurtainState.IntrusionProhibited == (Int16)isIntrusion.Value)
                {
                    requirements.Add(typeof(CurtainOpened));
                }
            }
        }

        if (station.HasConveyor)
        {
            if (!string.IsNullOrEmpty(station.Tag_ConveyorState)) {
                if (step == FlowStep.BeforePick)
                {
                    // nếu băng tải chưa có vật thì cần chờ vật
                    var isCassette = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorState);
                    if ((int)ConveyorStage.HasCassette != (Int16)isCassette.Value)
                    {
                        requirements.Add(typeof(ConveyorBoxArrived));
                    }
                }
                else if (step == FlowStep.BeforeDrop)   
                {
                    // nếu băng tải có vật thì cần chờ băng tải trống
                    var isCassette = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorState);
                    if ((int)ConveyorStage.Empty != (Int16)isCassette.Value)
                    {
                        requirements.Add(typeof(ConveyorBoxRemoved));
                    }
                }
            }

            if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber)) {
                if (step == FlowStep.BeforeDrop) {
                    // nếu băng tải chưa có vật thì cần chờ vật
                    var count = await _opcUaClient.ReadValueAsync(station.Tag_ConveyorNumber);
                    if ((Int16)count.Value >= 5)
                    {
                        requirements.Add(typeof(ConveyorRemainingSpace));
                    }
                }
            }
        } else if (!string.IsNullOrEmpty(station.Tag_HasCassette)) {
            if (step == FlowStep.BeforePick) {
                // nếu station không có cassette thì cần chờ cassette
                var isCassette = await _opcUaClient.ReadValueAsync(station.Tag_HasCassette);
                if ((int)StationHasCassette.Exist != isCassette.Value.ToInt16())
                {
                    requirements.Add(typeof(BoxArrived));
                }
            } else if (step == FlowStep.BeforeDrop) {
                // nếu station có cassette thì cần chờ cassette
                var isCassette = await _opcUaClient.ReadValueAsync(station.Tag_HasCassette);
                if ((int)StationHasCassette.NoExist != isCassette.Value.ToInt16())
                {
                    requirements.Add(typeof(BoxRemoved));
                }
            }
        }
        _logger.LogInformation("requirements: {Requirements}", requirements);
        // Nếu không có gì cần chờ, return empty WaitingSet
        if (requirements.Count == 0)
        {
            return WaitingSet.For(); // Empty set
        }

        // Tạo WaitingSet với tất cả requirements
        return requirements.Count switch
        {
            1 => CreateWaitingSetForSingleType(requirements[0]),
            2 => CreateWaitingSetForTwoTypes(requirements[0], requirements[1]),
            _ => WaitingSet.ForTypes([.. requirements])
        };
    }

    private static WaitingSet CreateWaitingSetForSingleType(Type type)
    {
        var method = typeof(WaitingSet).GetMethods()
            .First(m => m.Name == "For"
                        && m.IsGenericMethodDefinition
                        && m.GetGenericArguments().Length == 1
                        && m.GetParameters().Length == 0);
        var genericMethod = method!.MakeGenericMethod(type);
        return (WaitingSet)genericMethod.Invoke(null, null)!;
    }

    private static WaitingSet CreateWaitingSetForTwoTypes(Type type1, Type type2)
    {
        var method = typeof(WaitingSet).GetMethods()
            .First(m => m.Name == "For" && m.GetGenericArguments().Length == 2);
        var genericMethod = method.MakeGenericMethod(type1, type2);
        return (WaitingSet)genericMethod.Invoke(null, null)!;
    }

    private async Task<bool> HasActiveTaskOnStationAsync(string stationCode)
    {
        var flowTaskRepository = _serviceProvider.GetRequiredService<IFlowTaskRepository>();
        var tasks = await flowTaskRepository.GetTasksInProgressOnStationAsync(stationCode);
        return tasks.Any();
    }

    /// <summary>
    /// Kiểm tra trạm không có FlowTask đang chạy (Active, chưa Completed).
    /// </summary>
    public async Task<bool> IsStationFreeForNewTaskAsync(string stationCode) =>
        !await HasActiveTaskOnStationAsync(stationCode);
}
