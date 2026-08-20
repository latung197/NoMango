using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Rcs.DTOs;
using Wcs.Common.ValueObjects;
using Wcs.Api.Configs;
using Wcs.Api.RobotFlow;
using Microsoft.Extensions.Options;
using Wcs.Rcs;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class MoveToDropWaitingPointHandler(
  ILogger<MoveToDropWaitingPointHandler> logger, 
  IRcsClient rcsClient, 
  IEventPublisher publisher, 
  ITaskStateStore store,
  IOptions<HikConfig> options)
{
  private readonly ILogger<MoveToDropWaitingPointHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly HikConfig _hikConfig = options.Value;

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
    if (string.IsNullOrEmpty(flowTask.RcsTaskId))
    {
      flowTask.RcsTaskId = await _rcsClient.GenAgvSchedulingTaskOrThrowAsync(_store, flowTask, new CreateTaskRequest(
        TaskTyp: _hikConfig.FallbackTaskTyp,
        Priority: "1",
        PositionCodePath: [
          RcsPositions.Berth(flowTask.ToStation.WaitingPoint),
        ],
        AgvCode: flowTask.Robot!.Code
      ), ct);

      flowTask.UpdateStep(FlowStep.MoveToDropWaitingPoint);
      await _store.SaveAsync(flowTask);
    } else {
      await _rcsClient.ContinueTaskOrThrowAsync(_store, flowTask, new ContinueTaskRequest(
        flowTask.RcsTaskId,
        RcsPositions.Berth(flowTask.ToStation.WaitingPoint)
      ), ct);
      flowTask.UpdateStep(FlowStep.MoveToDropWaitingPoint);
      await _store.SaveAsync(flowTask);
    }
  }
}