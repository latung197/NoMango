using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;
using Wcs.OpcUa;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.EventTransformers;

/// <summary>
/// Transform OPC UA tag changes thành domain events
/// Phân tích tag name và value để tạo ra các events phù hợp
/// </summary>
public class OpcTagEventTransformer(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<OpcUaOptions> opcUaOptions,
    IOpcUaClient opcUaClient,
    ILogger<OpcTagEventTransformer> logger) : IEventTransformer<OpcTagChangedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<OpcTagEventTransformer> _logger = logger;
    private readonly OpcUaOptions _opcUaOptions = opcUaOptions.Value;
    private readonly IOpcUaClient _opcUaClient = opcUaClient;

    public bool CanTransform(OpcTagChangedEvent input)
    {
        if (string.IsNullOrEmpty(input.NodeId)) return false;
        return true;
    }

    public async Task<IEnumerable<object>> TransformAsync(OpcTagChangedEvent input, CancellationToken cancellationToken = default)
    {
        var events = new List<object>();
        _logger.LogDebug("Transforming OpcTagChangedEvent: NodeId={NodeId}, NewValue={NewValue}", input.NodeId, input.NewValue);
        try
        {
            if (input.NodeId.Equals(_opcUaOptions.TagClockPulseRead))
            {
                await _opcUaClient.WriteValueAsync(_opcUaOptions.TagClockPC, input.NewValue.ToInt32());
                return events;
            }
            if (input.NodeId.Equals(_opcUaOptions.TagSystemStatus))
            {
                if ((int)SystemStatus.Normal == input.NewValue.ToInt32())
                {
                    events.Add(new SystemStatusNormal());
                }
                else if ((int)SystemStatus.Error == input.NewValue.ToInt32())
                {
                    events.Add(new SystemStatusError());
                }
                return events;
            }

            var station = await FindStationByTagAsync(input.NodeId);
            if (station == null)
            {
                _logger.LogDebug("Không tìm thấy station cho tag {NodeId}", input.NodeId);
                return events;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var flowTaskRepository = scope.ServiceProvider.GetRequiredService<IFlowTaskRepository>();
            var tasks = await flowTaskRepository.GetTasksInProgressOnStationAsync(station.Code, cancellationToken);
            var task = tasks.FirstOrDefault();
            
            // Nếu là Tag CurtainState thì transform thành CurtainOpened hoặc CurtainClosed
            if (!string.IsNullOrEmpty(station.Tag_CurtainState) && input.NodeId.Equals(station.Tag_CurtainState))
            {
                var curtainEvents = TransformCurtainStateTag(task, station, input);
                events.AddRange(curtainEvents);
            }

            if (!string.IsNullOrEmpty(station.Tag_ConveyorState) && input.NodeId.Equals(station.Tag_ConveyorState))
            {
                var conveyorEvents = TransformConveyorStageTag(task, station, input);
                events.AddRange(conveyorEvents);
                events.Add(new StationChanged(station));
            }

            if (!string.IsNullOrEmpty(station.Tag_HasCassette) && input.NodeId.Equals(station.Tag_HasCassette))
            {
                var hasCassetteEvents = TransformHasCassetteTag(task, station, input);
                events.AddRange(hasCassetteEvents);
                events.Add(new StationChanged(station));
            }

            if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber) && input.NodeId.Equals(station.Tag_ConveyorNumber))
            {
                var conveyorNumberEvents = TransformConveyorNumberTag(task, station, input);
                events.AddRange(conveyorNumberEvents);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi transform OpcTagChangedEvent: NodeId={NodeId}", input.NodeId);
        }
        _logger.LogInformation("Transform OpcTagChangedEvent: NodeId={NodeId}, NewValue={NewValue} - events={Events}", input.NodeId, input.NewValue, events);
        return events;
    }

    /// <summary>
    /// Transform tag CurtainState - phát hiện curtain mở/đóng
    /// </summary>
    private List<object> TransformCurtainStateTag(FlowTask? task, Station station, OpcTagChangedEvent input)
    {
        var events = new List<object>();

        // IntrusionState = false => Curtain opened (không có vật cản)
        // IntrusionState = true => Curtain closed (có vật cản)
        if (input.NewValue == null)
        {
            _logger.LogWarning("{TaskId} NewValue là null cho tag {NodeId}", task?.Id, input.NodeId);
            return events;
        }
        _logger.LogInformation("{TaskId} TransformCurtainStateTag: StationCode: {StationCode}, NewValue: {NewValue}, NodeId: {NodeId}", task?.Id, station.Code, input.NewValue, input.NodeId);
        try
        {
            var curtainStateValue = input.NewValue.ToInt32();
            
            if (curtainStateValue == (int)CurtainState.IntrusionPossible)
            {
                if (task != null)
                {
                    events.Add(new CurtainOpened(task.Id.ToString(), station.Code));
                }
            }
            else if (curtainStateValue == (int)CurtainState.IntrusionProhibited)
            {
                if (task != null)
                {
                    events.Add(new CurtainClosed(task.Id.ToString(), station.Code));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{TaskId} Không thể convert giá trị curtain state {Value} cho tag {NodeId}", task?.Id, input.NewValue, input.NodeId);
        }

        return events;
    }

    private List<object> TransformConveyorStageTag(FlowTask? task, Station station, OpcTagChangedEvent input)
    {
        var events = new List<object>();

        if (input.NewValue == null)
        {
            _logger.LogWarning("NewValue là null cho tag {NodeId}", input.NodeId);
            return events;
        }
        _logger.LogInformation("{TaskId} TransformConveyorStageTag: StationCode: {StationCode}, NewValue: {NewValue}, NodeId: {NodeId}", task?.Id, station.Code, input.NewValue, input.NodeId);
        try
        {
            var conveyorStageValue = input.NewValue.ToInt32();
            if (conveyorStageValue == (int)ConveyorStage.HasCassette)
            {
                if (task != null)
                {
                    events.Add(new ConveyorBoxArrived(task.Id.ToString(), station.Code));
                }
                station.IsExistCassette = true;
            }
            else if (conveyorStageValue == (int)ConveyorStage.Empty)
            {
                if (task != null)
                {
                    events.Add(new ConveyorBoxRemoved(task.Id.ToString(), station.Code));
                }
                station.IsExistCassette = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{TaskId} Không thể convert giá trị conveyor stage {Value} cho tag {NodeId}", task?.Id, input.NewValue, input.NodeId);
        }

        return events;
    }

    private List<object> TransformHasCassetteTag(FlowTask? task, Station station, OpcTagChangedEvent input)
    {
        var events = new List<object>();

        if (input.NewValue == null)
        {
            _logger.LogWarning("NewValue là null cho tag {NodeId}", input.NodeId);
            return events;
        }
        _logger.LogInformation("{TaskId} TransformHasCassetteTag: StationCode: {StationCode}, NewValue: {NewValue}, NodeId: {NodeId}", task?.Id, station.Code, input.NewValue, input.NodeId);
        try
        {
            var hasCassetteValue = input.NewValue.ToInt16();
            if (hasCassetteValue == (int)StationHasCassette.Exist)
            {
                if (task != null)
                {
                    events.Add(new BoxArrived(task.Id.ToString(), station.Code));
                }
                station.IsExistCassette = true;
            }
            else if (hasCassetteValue == (int)StationHasCassette.NoExist)
            {
                if (task != null)
                {
                    events.Add(new BoxRemoved(task.Id.ToString(), station.Code));
                }
                station.IsExistCassette = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{TaskId} Không thể convert giá trị has cassette {Value} cho tag {NodeId}", task?.Id, input.NewValue, input.NodeId);
        }

        return events;
    }

    private List<object> TransformConveyorNumberTag(FlowTask? task, Station station, OpcTagChangedEvent input)
    {
        var events = new List<object>();

        if (input.NewValue == null)
        {
            _logger.LogWarning("NewValue là null cho tag {NodeId}", input.NodeId);
            return events;
        }
        _logger.LogInformation("{TaskId} TransformConveyorNumberTag: StationCode: {StationCode}, NewValue: {NewValue}, NodeId: {NodeId}", task?.Id, station.Code, input.NewValue, input.NodeId);
        try
        {
            var conveyorNumberValue = input.NewValue.ToInt32();
            if (conveyorNumberValue >= 5)
            {
                if (task != null)
                {
                    events.Add(new ConveyorFull(task.Id.ToString(), station.Code));
                }
            }
            else if (conveyorNumberValue < 5)
            {
                if (task != null)
                {
                    events.Add(new ConveyorRemainingSpace(task.Id.ToString(), station.Code));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{TaskId} Không thể convert giá trị conveyor number {Value} cho tag {NodeId}", task?.Id, input.NewValue, input.NodeId);
        }

        return events;
    }

    private async Task<Station?> FindStationByTagAsync(string tag)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var stationService = scope.ServiceProvider.GetRequiredService<StationService>();
        return await stationService.FindStationByTag(tag);
    }
}

