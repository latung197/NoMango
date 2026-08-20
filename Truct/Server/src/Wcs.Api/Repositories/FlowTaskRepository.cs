using Microsoft.EntityFrameworkCore;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;
using Wcs.Infrastructure.Data;
using Wcs.Api.Services;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Constants;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Repositories;

public class FlowTaskRepository(WcsDbContext context, RobotService robotService, StationService stationService) : IFlowTaskRepository
{
    private readonly WcsDbContext _context = context;
    private readonly RobotService _robotService = robotService;
    private readonly StationService _stationService = stationService;

    public async Task<FlowTask> CreateAsync(FlowTask flowTask, CancellationToken cancellationToken = default)
    {
        var dbModel = flowTask.ToDbModel();
        _context.FlowTasks.Add(dbModel);
        
        // Thêm waiting sets nếu có
        if (flowTask.WaitingFor != null)
        {
            var waitingSetDbModels = flowTask.WaitingFor.ToWaitingSetDbModels(flowTask.Id);
            _context.FlowTaskWaitingSets.AddRange(waitingSetDbModels);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        // Load related entities from services
        var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
        var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
        var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);
        
        return dbModel.ToDomain(robot, fromStation, toStation);
    }

    public async Task UpdateAsync(FlowTask flowTask, CancellationToken cancellationToken = default)
    {
        // Kiểm tra xem entity có đang được track không
        var trackedEntity = _context.ChangeTracker.Entries<FlowTaskDbModel>()
            .FirstOrDefault(e => e.Entity.Id == flowTask.Id);
        
        if (trackedEntity != null)
        {
            // Nếu đã được track, detach nó
            trackedEntity.State = EntityState.Detached;
        }
        
        // Tạo entity mới từ domain object
        var dbModel = flowTask.ToDbModel();
        
        // Xóa waiting sets cũ
        var oldWaitingSets = await _context.FlowTaskWaitingSets
            .Where(ws => ws.FlowTaskId == flowTask.Id)
            .ToListAsync(cancellationToken);
        _context.FlowTaskWaitingSets.RemoveRange(oldWaitingSets);
        
        // Thêm waiting sets mới nếu có
        if (flowTask.WaitingFor != null)
        {
            var waitingSetDbModels = flowTask.WaitingFor.ToWaitingSetDbModels(flowTask.Id);
            _context.FlowTaskWaitingSets.AddRange(waitingSetDbModels);
        }
        
        // Update entity (CreatedAt must stay DB value — Update() marks every property modified)
        _context.FlowTasks.Update(dbModel);
        _context.Entry(dbModel).Property(e => e.CreatedAt).IsModified = false;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task LogAsync(FlowTask flowTask, string logMessage, string? eventName = null, CancellationToken cancellationToken = default)
    {
        _context.FlowTaskHistory.Add(new FlowTaskHistoryDbModel
        {
            FlowTaskId = flowTask.Id,
            Step = flowTask.CurrentStep,
            LogMessage = logMessage,
            Event = eventName,
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlowTask>> GetTasksInProgressOnStationAsync(string stationCode, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Where(ft => ft.Status == FlowStatus.Active)
            .Where(ft => ft.CurrentStep != FlowStep.Completed)
            .Where(ft =>
                (ft.FromStationCode == stationCode && ft.CurrentStep < FlowConstant.FromStationReleaseStep)
                || ft.ToStationCode == stationCode)
            .ToListAsync(cancellationToken);

        return await MapToFlowTasksAsync(dbModels, cancellationToken);
    }

    public async Task<IEnumerable<FlowTask>> GetActiveTasksByRouteAsync(string fromStationCode, string toStationCode, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Where(ft => ft.FromStationCode == fromStationCode && ft.ToStationCode == toStationCode)
            .Where(ft => ft.Status == FlowStatus.Active)
            .Where(ft => ft.CurrentStep != FlowStep.Completed)
            .ToListAsync(cancellationToken);

        return await MapToFlowTasksAsync(dbModels, cancellationToken);
    }

    public async Task<IEnumerable<FlowTask>> GetTasksInProgressOnRobotAsync(string robotCode, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Where(ft => ft.RobotCode == robotCode)
            .Where(ft => ft.Status == FlowStatus.Active)
            .Where(ft => ft.CurrentStep != FlowStep.Completed)
            .ToListAsync(cancellationToken);

        return await MapToFlowTasksAsync(dbModels, cancellationToken);
    }

    private async Task<List<FlowTask>> MapToFlowTasksAsync(IReadOnlyList<FlowTaskDbModel> dbModels, CancellationToken cancellationToken)
    {
        var flowTasks = new List<FlowTask>();
        foreach (var dbModel in dbModels)
        {
            var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
            var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
            var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);
            flowTasks.Add(dbModel.ToDomain(robot, fromStation, toStation));
        }
        return flowTasks;
    }

    public async Task<IEnumerable<FlowTask>> GetActiveTasksAsync(CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Where(ft => ft.Status == FlowStatus.Active)
            .Where(ft => ft.CurrentStep != FlowStep.Completed)
            .OrderByDescending(ft => ft.CreatedAt)
            .ToListAsync(cancellationToken);

        // Sequential (not WhenAll) để tránh concurrent DbContext access
        var flowTasks = new List<FlowTask>();
        foreach (var dbModel in dbModels)
        {
            var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
            var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
            var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);
            flowTasks.Add(dbModel.ToDomain(robot, fromStation, toStation));
        }

        return flowTasks;
    }

    public async Task<IEnumerable<FlowTask>> GetCancelledTasksAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Where(ft => ft.Status == FlowStatus.Cancelled)
            .OrderByDescending(ft => ft.UpdatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var flowTasks = new List<FlowTask>();
        foreach (var dbModel in dbModels)
        {
            var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
            var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
            var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);
            flowTasks.Add(dbModel.ToDomain(robot, fromStation, toStation));
        }

        return flowTasks;
    }

    public async Task<IEnumerable<FlowTask>> GetPendingStartTasksAsync(CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Where(ft => ft.Status == FlowStatus.Pending)
            .Where(ft => ft.RcsTaskId == null)
            .Where(ft => ft.CurrentStep == FlowStep.Initial)
            .OrderBy(ft => ft.CreatedAt)
            .ToListAsync(cancellationToken);

        var flowTasks = new List<FlowTask>();
        foreach (var dbModel in dbModels)
        {
            var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
            var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
            var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);
            flowTasks.Add(dbModel.ToDomain(robot, fromStation, toStation));
        }

        return flowTasks;
    }

    public async Task<IEnumerable<FlowTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Include(ft => ft.History)
            .OrderByDescending(ft => ft.CreatedAt)
            .ToListAsync(cancellationToken);

        var flowTasks = new List<FlowTask>();
        foreach (var dbModel in dbModels)
        {
            var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
            var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
            var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);
            flowTasks.Add(dbModel.ToDomain(robot, fromStation, toStation));
        }

        return flowTasks;
    }

    public async Task<FlowTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Include(ft => ft.History)
            .FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken);

        if (dbModel == null) return null;

        // Load related entities from services
        var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
        var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
        var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);

        return dbModel.ToDomain(robot, fromStation, toStation);
    }

    public async Task<FlowTask?> GetByRcsTaskIdAsync(string rcsTaskId, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Include(ft => ft.History)
            .FirstOrDefaultAsync(ft => ft.RcsTaskId == rcsTaskId, cancellationToken);

        if (dbModel == null) return null;
        
        // Load related entities from services
        var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
        var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
        var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);

        return dbModel.ToDomain(robot, fromStation, toStation);
    }

    public async Task<FlowTask?> GetActiveByCraneTaskNoAsync(int craneTaskNo, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .FirstOrDefaultAsync(
                ft => ft.CraneTaskNo == craneTaskNo
                    && ft.Status == FlowStatus.Active
                    && ft.CurrentStep != FlowStep.Completed,
                cancellationToken);

        if (dbModel == null) return null;

        var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
        var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
        var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);

        return dbModel.ToDomain(robot, fromStation, toStation);
    }

    public async Task<FlowTask?> GetSingleActiveAfterDropWithCraneAsync(CancellationToken cancellationToken = default)
    {
        var matches = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Where(ft => ft.Status == FlowStatus.Active
                && ft.CurrentStep == FlowStep.AfterDrop
                && ft.CraneTaskNo != null)
            .ToListAsync(cancellationToken);

        if (matches.Count != 1)
        {
            return null;
        }

        var dbModel = matches[0];
        var robot = await _robotService.GetRobotByCode(dbModel.RobotCode, cancellationToken);
        var fromStation = await _stationService.GetStationByCode(dbModel.FromStationCode);
        var toStation = await _stationService.GetStationByCode(dbModel.ToStationCode);

        return dbModel.ToDomain(robot, fromStation, toStation);
    }

    public async Task<IReadOnlyList<FlowTaskHistoryEntry>> GetHistoryAsync(
        string? taskId = null,
        bool errorsOnly = true,
        string? eventName = null,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _context.FlowTaskHistory.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(taskId))
        {
            query = query.Where(h => h.FlowTaskId == taskId);
        }

        if (!string.IsNullOrWhiteSpace(eventName))
        {
            query = query.Where(h => h.Event == eventName);
        }
        else if (errorsOnly)
        {
            query = query.Where(h =>
                h.Event == "ExternalApiError"
                || h.Event == "FlowErrorOccurred"
                || h.Event == "TimeoutFired"
                || h.Event == "StopEmergency"
                || (h.LogMessage != null && (
                    h.LogMessage.StartsWith("[RCS]")
                    || h.LogMessage.StartsWith("[WMS]")
                    || h.LogMessage.StartsWith("[Crane]")
                    || h.LogMessage.StartsWith("[OPC]"))));
        }

        var rows = await query
            .OrderByDescending(h => h.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return rows.Select(h => new FlowTaskHistoryEntry
        {
            Id = h.Id,
            FlowTaskId = h.FlowTaskId,
            Event = h.Event,
            Step = h.Step,
            LogMessage = h.LogMessage,
            CreatedAt = h.CreatedAt,
        }).ToList();
    }
}