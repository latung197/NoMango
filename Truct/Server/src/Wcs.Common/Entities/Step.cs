using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Step(Guid id, Guid flow, int step, string stage)
{
    public Guid Id { get; private set; } = id;
    public Guid FlowId { get; set; } = flow;
    public int StepNo { get; set; } = step;
    public string Stage { get; set; } = stage;
    public Size? Size { get; set; }
}