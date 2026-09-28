using Core.Application.CustomModels.Dtos;
using Core.Application.Mapping;
using Core.Domain.Entity;
using Xunit;

namespace Core.Security.Tests;

public class MappingTests
{
    private readonly CoreMapper _mapper = new();

    [Fact]
    public void ExportPlanDto_MapsFieldsAndUpdatesTrackedEntity()
    {
        var dto = new ExportListPlanDto
        {
            ExportPlanID = 42,
            OrderNumber = "ORD-42",
            ProductID = 7,
            CustomerCode = "C01",
            IndicatorQuantity = 12,
            BillState = 2
        };

        var created = _mapper.ToExportListPlan(dto);
        Assert.Equal(42, created.ExportPlanID);
        Assert.Equal(7, created.ProductID);
        Assert.Equal("ORD-42", created.OrderNumber);

        var tracked = new ExportListPlan { ExportPlanID = 42, ProductID = 3, OrderNumber = "OLD" };
        _mapper.UpdateExportListPlan(dto, tracked);
        Assert.Equal(42, tracked.ExportPlanID);
        Assert.Equal(7, tracked.ProductID);
        Assert.Equal("ORD-42", tracked.OrderNumber);
        Assert.Equal(12, tracked.IndicatorQuantity);
        Assert.Equal(2, tracked.BillState);
    }

    [Fact]
    public void ExportHistoryDto_MapsAndUpdatesTrackedEntity()
    {
        var dto = new ExportHistoryListDto { ExportHistoryID = 5, ExportPlanID = 42, HUSerial = "HU123" };
        var created = _mapper.ToExportHistoryList(dto);
        Assert.Equal("HU123", created.HUSerial);

        var tracked = new ExportHistoryList { ExportHistoryID = 5, HUSerial = "OLD" };
        _mapper.UpdateExportHistoryList(dto, tracked);
        Assert.Equal("HU123", tracked.HUSerial);
        Assert.Equal(42, tracked.ExportPlanID);
    }

    [Fact]
    public void EcuDto_MapsIntoBothStorageEntities()
    {
        var dto = new EcuDataDto { EcuDataID = 9, HUCode = "HU-9", PackState = 1 };
        Assert.Equal("HU-9", _mapper.ToEcuData(dto).HUCode);
        Assert.Equal(1, _mapper.ToEcuExported(dto).PackState);
    }
}
