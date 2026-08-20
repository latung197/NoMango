using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;

namespace Wcs.Infrastructure.Data.Mappers;

public static class StepMapper
{
    public static Step ToDomain(this StepDbModel dbModel)
    {
        var step = new Step(dbModel.Id, dbModel.FlowId, dbModel.StepNo, dbModel.Stage)
        {
            Size = dbModel.Size
        };
        return step;
    }

    public static StepDbModel ToDbModel(this Step domain)
    {
        return new StepDbModel
        {
            Id = domain.Id,
            FlowId = domain.FlowId,
            StepNo = domain.StepNo,
            Stage = domain.Stage,
            Size = domain.Size
        };
    }
}