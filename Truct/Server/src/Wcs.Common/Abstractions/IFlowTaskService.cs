using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions;

/// <summary>
/// Service để xử lý logic liên quan đến FlowTask
/// </summary>
public interface IFlowTaskService
{
    /// <summary>
    /// Lấy bước tiếp theo trong flow sử dụng FlowConstant
    /// </summary>
    FlowStep? GetNextStep(FlowStep currentStep);

    /// <summary>
    /// Validate CurtainOpened event có phù hợp với task không
    /// </summary>
    bool ValidateCurtainEvent(FlowTask task, CurtainOpened eventData);

    /// <summary>
    /// Validate RobotTaskComplete event có phù hợp với task không
    /// </summary>
    bool ValidateRobotTaskCompleteEvent(FlowTask task, RobotTaskComplete eventData);

    /// <summary>
    /// Validate ConveyorBoxArrived event có phù hợp với task không
    /// </summary>
    bool ValidateConveyorBoxArrivedEvent(FlowTask task, ConveyorBoxArrived eventData);

    /// <summary>
    /// Validate ConveyorBoxRemoved event có phù hợp với task không
    /// </summary>
    bool ValidateConveyorBoxRemovedEvent(FlowTask task, ConveyorBoxRemoved eventData);

    /// <summary>
    /// Validate BoxArrived event có phù hợp với task không
    /// </summary>
    bool ValidateBoxArrivedEvent(FlowTask task, BoxArrived eventData);

    /// <summary>
    /// Validate BoxRemoved event có phù hợp với task không
    /// </summary>
    bool ValidateBoxRemovedEvent(FlowTask task, BoxRemoved eventData);

    /// <summary>
    /// Validate ConveyorRemainingSpace event có phù hợp với task không
    /// </summary>
    bool ValidateConveyorRemainingSpaceEvent(FlowTask task, ConveyorRemainingSpace eventData);

    /// <summary>
    /// Validate FlowStarted event có phù hợp với task không
    /// </summary>
    bool ValidateFlowStartedEvent(FlowTask task, FlowStarted eventData);
}
