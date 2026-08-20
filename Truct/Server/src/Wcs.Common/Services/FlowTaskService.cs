using Wcs.Common.Abstractions;
using Wcs.Common.Constants;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Services;

public class FlowTaskService : IFlowTaskService
{

    /// <summary>
    /// Lấy bước tiếp theo trong flow sử dụng FlowConstant
    /// </summary>
    public FlowStep? GetNextStep(FlowStep currentStep)
    {
        var index = Array.IndexOf(FlowConstant.FlowStepList, currentStep);
        if (index == -1 || index == FlowConstant.FlowStepList.Length - 1)
            return null;
        
        return FlowConstant.FlowStepList[index + 1];
    }

    public bool ValidateCurtainEvent(FlowTask task, CurtainOpened eventData)
    {
        if (task.CurrentStep == FlowStep.BeforePick) {
            return task.FromStation.HasCurtain && eventData.StationCode == task.FromStation.Code;
        }
        if (task.CurrentStep == FlowStep.BeforeDrop) {
            return task.ToStation.HasCurtain && eventData.StationCode == task.ToStation.Code;
        }
        return false;
    }

    public bool ValidateRobotTaskCompleteEvent(FlowTask task, RobotTaskComplete eventData)
    {
        if (!HikCallbackStep.TryGetFlowStep(eventData.Step, out var eventStep)) {
            return false;
        }

        return task.CurrentStep == eventStep;
    }

    public bool ValidateConveyorBoxArrivedEvent(FlowTask task, ConveyorBoxArrived eventData)
    {
        if (task.CurrentStep == FlowStep.BeforePick) {
            return task.FromStation.HasConveyor && eventData.StationCode == task.FromStation.Code;
        }
        return false;
    }

    public bool ValidateConveyorBoxRemovedEvent(FlowTask task, ConveyorBoxRemoved eventData)
    {
        if (task.CurrentStep == FlowStep.BeforeDrop) {
            return task.ToStation.HasConveyor && eventData.StationCode == task.ToStation.Code;
        }
        return false;
    }

    public bool ValidateBoxArrivedEvent(FlowTask task, BoxArrived eventData)
    {
        if (task.CurrentStep == FlowStep.BeforePick) {
            return !string.IsNullOrEmpty(task.FromStation.Tag_HasCassette) && eventData.StationCode == task.FromStation.Code;
        }
        return false;
    }

    public bool ValidateBoxRemovedEvent(FlowTask task, BoxRemoved eventData)
    {
        if (task.CurrentStep == FlowStep.BeforeDrop) {
            return !string.IsNullOrEmpty(task.ToStation.Tag_HasCassette) && eventData.StationCode == task.ToStation.Code;
        }
        return false;
    }

    public bool ValidateConveyorRemainingSpaceEvent(FlowTask task, ConveyorRemainingSpace eventData)
    {
        if (task.CurrentStep == FlowStep.BeforeDrop) {
            return !string.IsNullOrEmpty(task.ToStation.Tag_ConveyorNumber) && eventData.StationCode == task.ToStation.Code;
        }
        return false;
    }

    public bool ValidateFlowStartedEvent(FlowTask task, FlowStarted eventData)
    {
        if (task.RcsTaskId == null) {
            return true;
        }
        return false;
    }
}
