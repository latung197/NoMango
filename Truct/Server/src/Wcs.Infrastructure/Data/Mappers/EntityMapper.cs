using System.Text.Json;
using Wcs.Infrastructure.Data.Models;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Mappers;

public static class EntityMapper
{
    // Robot and Station are loaded from config, not from DB

    // FlowTask mappings
    public static FlowTask ToDomain(this FlowTaskDbModel dbModel, Robot? robot = null, Station? fromStation = null, Station? toStation = null)
    {
        List<CranePosition>? cranePositions = null;
        if (!string.IsNullOrEmpty(dbModel.CranePositionsJson))
        {
            try
            {
                cranePositions = JsonSerializer.Deserialize<List<CranePosition>>(dbModel.CranePositionsJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch
            {
                // If deserialization fails, leave as null
                cranePositions = null;
            }
        }

        TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var domain = new FlowTask
        {
            Id = dbModel.Id,
            CurrentStep = dbModel.CurrentStep,
            RcsTaskId = dbModel.RcsTaskId,
            Status = dbModel.Status,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt,
            CompletedAt = dbModel.CompletedAt,
            CurrentStepStartedAt = dbModel.CurrentStepStartedAt,
            CranePositions = cranePositions,
            CassetteCode = dbModel.CassetteCode,
            StorageStageCode = dbModel.StorageStageCode,
            Quantity = dbModel.Quantity,
            Product = dbModel.Product,
            CraneTaskNo = dbModel.CraneTaskNo,
            Reroute = DeserializeReroute(dbModel.RerouteJson),
            RerouteCount = dbModel.RerouteCount,
            ConsumedDropFillers = dbModel.ConsumedDropFillers,
            TaskSize = (Size)dbModel.TaskSize
        };

        // Set related entities from parameters (loaded from config)
        if (robot != null)
            domain.Robot = robot;
        
        if (fromStation != null)
            domain.FromStation = fromStation;
        
        if (toStation != null)
            domain.ToStation = toStation;

        // Map waiting conditions
        if (dbModel.WaitingSets.Count != 0)
        {
            var needs = dbModel.WaitingSets.Select(ws => ws.NeedType).ToArray();
            var waitingSet = WaitingSet.For(needs);
            
            foreach (var ws in dbModel.WaitingSets.Where(w => w.IsMet))
            {
                waitingSet.Mark(ws.NeedType);
            }
            
            domain.WaitingFor = waitingSet;
        }

        return domain;
    }

    public static FlowTaskDbModel ToDbModel(this FlowTask domain)
    {
        string? cranePositionsJson = null;
        if (domain.CranePositions != null && domain.CranePositions.Count > 0)
        {
            cranePositionsJson = JsonSerializer.Serialize(domain.CranePositions, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
        }

        var dbModel = new FlowTaskDbModel
        {
            Id = domain.Id,
            FromStationCode = domain.FromStation?.Code ?? string.Empty,
            ToStationCode = domain.ToStation?.Code ?? string.Empty,
            RobotCode = domain.Robot?.Code ?? string.Empty,
            CurrentStep = domain.CurrentStep,
            RcsTaskId = domain.RcsTaskId,
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            CompletedAt = domain.CompletedAt,
            CurrentStepStartedAt = domain.CurrentStepStartedAt,
            CranePositionsJson = cranePositionsJson,
            CassetteCode = domain.CassetteCode,
            StorageStageCode = domain.StorageStageCode,
            Quantity = domain.Quantity,
            Product = domain.Product,
            CraneTaskNo = domain.CraneTaskNo,
            RerouteJson = domain.Reroute is null ? null : JsonSerializer.Serialize(domain.Reroute),
            RerouteCount = domain.RerouteCount,
            ConsumedDropFillers = domain.ConsumedDropFillers,
            TaskSize = (int)domain.TaskSize
        };

        return dbModel;
    }

    private static RerouteRequest? DeserializeReroute(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<RerouteRequest>(json);
        }
        catch
        {
            return null;
        }
    }

    // Mapping methods for WaitingSet to DB entities
    public static List<FlowTaskWaitingSetDbModel> ToWaitingSetDbModels(this WaitingSet? waitingSet, string flowTaskId)
    {
        if (waitingSet == null) return [];

        var needs = waitingSet.GetNeeds();
        var met = waitingSet.GetMet();

        return [.. needs.Select(need => new FlowTaskWaitingSetDbModel
        {
            Id = Guid.NewGuid(),
            FlowTaskId = flowTaskId,
            NeedType = need,
            IsMet = met.Contains(need),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        })];
    }

    // Helper method to map full FlowTask with waiting sets
    public static FlowTaskDbModel ToDbModelWithWaitingSets(this FlowTask domain)
    {
        var dbModel = domain.ToDbModel();
        
        if (domain.WaitingFor != null)
        {
            dbModel.WaitingSets = domain.WaitingFor.ToWaitingSetDbModels(domain.Id);
        }

        return dbModel;
    }
}

