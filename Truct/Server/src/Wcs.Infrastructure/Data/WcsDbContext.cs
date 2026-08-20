using Microsoft.EntityFrameworkCore;
using Wcs.Infrastructure.Data.Models;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data;

public class WcsDbContext(DbContextOptions<WcsDbContext> options) : DbContext(options)
{
    public DbSet<FlowTaskDbModel> FlowTasks { get; set; } = null!;
    public DbSet<FlowTaskWaitingSetDbModel> FlowTaskWaitingSets { get; set; } = null!;
    public DbSet<FlowTaskHistoryDbModel> FlowTaskHistory { get; set; } = null!;
    public DbSet<FlowDbModel> Flows { get; set; } = null!;
    public DbSet<StepDbModel> Steps { get; set; } = null!;
    public DbSet<StageDbModel> Stages { get; set; } = null!;
    public DbSet<StationDbModel> Stations { get; set; } = null!;
    public DbSet<StationTagDbModel> StationTags { get; set; } = null!;
    public DbSet<CassetteDbModel> Cassettes { get; set; } = null!;
    public DbSet<AmrDbModel> Amrs { get; set; } = null!;
    public DbSet<MaterialDbModel> Materials { get; set; } = null!;
    public DbSet<RequestDbModel> Requests { get; set; } = null!;
    public DbSet<MaterialRequestDbModel> MaterialRequest { get; set; } = null!;
    public DbSet<ErrorDbModel> Errors { get; set; } = null!;
    public DbSet<SettingDbModel> Settings { get; set; } = null!;
    public DbSet<TransferRequestDbModel> TransferRequests { get; set; } = null!;
    public DbSet<CraneTaskDispatchDbModel> CraneTaskDispatches { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Stage configuration
        modelBuilder.Entity<StageDbModel>(entity =>
        {
            entity.HasIndex(s => s.Code);
            entity.Property(s => s.Area)
                .HasConversion(
                    v => v.Value,                    // Convert StageArea to string (for DB)
                    v => StageArea.FromString(v));   // Convert string to StageArea (from DB)
            entity.Property(s => s.Distance).HasConversion<float>();
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Station configuration
        modelBuilder.Entity<StationDbModel>(entity =>
        {
            entity.HasIndex(s => s.Code);
            entity.Property(s => s.Type).HasConversion<int>();   // Convert int to InOut (from DB)
            entity.Property(s => s.Sizes).HasMaxLength(50);
            entity.Property(s => s.HasCurtain).HasConversion<bool>();   // Convert bool to bool (from DB)
            entity.Property(s => s.HasConveyor).HasConversion<bool>();   // Convert bool to bool (from DB)
            entity.Property(s => s.MainPoint).HasMaxLength(50);
            entity.Property(s => s.WaitingPoint).HasMaxLength(50);
            entity.Property(s => s.AutoSendToStage).HasMaxLength(50);
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.CompletedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Station configuration
        modelBuilder.Entity<StationTagDbModel>(entity =>
        {
            entity.HasIndex(s => s.StationId);
            entity.Property(s => s.TagType)
                .HasConversion(
                    v => v.Value,
                    v => StationTag.FromString(v));
            entity.Property(s => s.Tag).HasMaxLength(255);
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Flow configuration
        modelBuilder.Entity<FlowDbModel>(entity =>
        {
            entity.HasIndex(s => s.Name);
            entity.Property(s => s.Area)
                .HasConversion(
                    v => v.Value,                    // Convert StageArea to string (for DB)
                    v => StageArea.FromString(v));   // Convert string to StageArea (from DB)
            //entity.Property(s => s.Priority)
            //    .HasConversion(
            //        v => v.Value,                    // Convert Priority to string (for DB)
            //        v => Priority.FromString(v));    // Convert string to Priority (from DB)
            entity.HasIndex(s => s.Priority);
            entity.HasIndex(s => s.ReturnEmpty);
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Step configuration
        modelBuilder.Entity<StepDbModel>(entity =>
        {
            entity.HasIndex(s => s.FlowId);
            entity.HasIndex(s => s.StepNo);
            entity.HasIndex(s => s.Stage);
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // FlowTask configuration
        modelBuilder.Entity<FlowTaskDbModel>(entity =>
        {
            entity.HasIndex(ft => ft.Status);
            entity.HasIndex(ft => ft.CurrentStep);
            entity.HasIndex(ft => ft.CreatedAt);
            entity.HasIndex(ft => ft.RobotCode);
            entity.HasIndex(ft => ft.FromStationCode);
            entity.HasIndex(ft => ft.ToStationCode);
            entity.HasIndex(ft => ft.CraneTaskNo);

            entity.Property(ft => ft.CurrentStep).HasConversion<int>();
            entity.Property(ft => ft.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(ft => ft.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(ft => ft.CompletedAt);
            entity.Property(ft => ft.CurrentStepStartedAt);
            entity.Property(ft => ft.CassetteCode).HasMaxLength(50);
            // No foreign key relationships - Robot/Station loaded from config
        });

        // CraneTaskDispatch configuration
        modelBuilder.Entity<CraneTaskDispatchDbModel>(entity =>
        {
            entity.HasIndex(d => d.TaskNo)
                .HasDatabaseName("IX_CraneTaskDispatches_TaskNo");
            entity.HasIndex(d => d.FlowTaskId);
            entity.HasIndex(d => d.Status);
            entity.HasIndex(d => d.CreatedAt);
            entity.HasIndex(d => d.IdempotencyKey)
                .HasDatabaseName("IX_CraneTaskDispatches_IdempotencyKey");
            entity.HasIndex(d => d.TaskNo)
                .IsUnique()
                .HasFilter("[Status] IN (0, 1, 2)")
                .HasDatabaseName("IX_CraneTaskDispatches_TaskNo_Active");
            entity.HasIndex(d => d.IdempotencyKey)
                .IsUnique()
                .HasFilter("[Status] IN (0, 1, 2)")
                .HasDatabaseName("IX_CraneTaskDispatches_IdempotencyKey_Active");

            entity.Property(d => d.Status).HasConversion<int>();
            entity.Property(d => d.Id).HasMaxLength(50);
            entity.Property(d => d.FlowTaskId).HasMaxLength(50);
            entity.Property(d => d.IdempotencyKey).HasMaxLength(200);
            entity.Property(d => d.ErrorNote).HasMaxLength(500);
            entity.Property(d => d.LastMessage).HasMaxLength(500);
            entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(d => d.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // FlowTaskWaitingSet configuration
        modelBuilder.Entity<FlowTaskWaitingSetDbModel>(entity =>
        {
            entity.HasIndex(ws => new { ws.FlowTaskId, ws.NeedType }).IsUnique();
            entity.Property(ws => ws.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(ws => ws.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(ws => ws.FlowTask)
                .WithMany(ft => ft.WaitingSets)
                .HasForeignKey(ws => ws.FlowTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FlowTaskHistory configuration
        modelBuilder.Entity<FlowTaskHistoryDbModel>(entity =>
        {
            entity.HasIndex(h => h.FlowTaskId);
            entity.HasIndex(h => h.CreatedAt);
            entity.Property(h => h.Step).HasConversion<int>();
            entity.Property(h => h.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(h => h.LogMessage).HasMaxLength(500);
            entity.Property(h => h.Event).HasMaxLength(100);

            entity.HasOne(h => h.FlowTask)
                .WithMany(ft => ft.History)
                .HasForeignKey(h => h.FlowTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cassette configuration
        modelBuilder.Entity<CassetteDbModel>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(255);
            entity.Property(c => c.Code).HasMaxLength(50);
            entity.Property(c => c.Size).HasConversion<int>();
            entity.Property(c => c.Capacity).HasDefaultValue(0);
            entity.Property(c => c.IsActive).HasDefaultValue(true);
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // AMR configuration
        modelBuilder.Entity<AmrDbModel>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.Code).IsUnique();
            entity.Property(a => a.Code).IsRequired().HasMaxLength(16);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(16);
            entity.Property(a => a.Area)
                .HasConversion(
                    v => v.Value,                    // Convert StageArea to string (for DB)
                    v => StageArea.FromString(v));   // Convert string to StageArea (from DB)
            entity.Property(a => a.MapCode).HasMaxLength(2);
            entity.Property(a => a.MapName).HasMaxLength(32);
            entity.Property(a => a.RobotStatus).HasMaxLength(10);
            entity.Property(a => a.TypeCode).HasMaxLength(30);
            entity.Property(a => a.Battery).HasMaxLength(3);
            entity.Property(a => a.Direction).HasMaxLength(4);
            entity.Property(a => a.Exclude).HasMaxLength(2);
            entity.Property(a => a.ExcludeStr).HasMaxLength(10);
            entity.Property(a => a.OnLine).HasMaxLength(2);
            entity.Property(a => a.PodCode).HasMaxLength(2);
            entity.Property(a => a.PodDir).HasMaxLength(10);
            entity.Property(a => a.PosX).HasMaxLength(10);
            entity.Property(a => a.PosY).HasMaxLength(10);
            entity.Property(a => a.Ip).HasMaxLength(20);
            entity.Property(a => a.Status).HasMaxLength(10);
            entity.Property(a => a.StatusStr).HasMaxLength(255);
            entity.Property(a => a.Stop).HasMaxLength(2);
            entity.Property(a => a.StopStr).HasMaxLength(255);
            entity.Property(a => a.IsActive).HasDefaultValue(true);
            entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Material configuration
        modelBuilder.Entity<MaterialDbModel>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.Code).IsUnique();
            entity.Property(m => m.Name).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Code).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Unit).HasMaxLength(15);
            entity.Property(m => m.Note).HasMaxLength(255);
            entity.Property(m => m.IsActive).HasDefaultValue(true);
            entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(m => m.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Request configuration
        modelBuilder.Entity<RequestDbModel>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.FlowTaskId).HasMaxLength(50);
            entity.HasIndex(r => r.Code).IsUnique();
            entity.Property(r => r.Code).HasMaxLength(50);
            entity.Property(r => r.StageCode).HasMaxLength(50);
            entity.Property(r => r.DeliveryCode).HasMaxLength(50);
            entity.Property(r => r.DeliveryOrder).HasConversion<int>();
            entity.Property(r => r.CurrentStep).HasConversion<int>();
            entity.Property(r => r.Note).HasMaxLength(255);
            entity.Property(r => r.IsActive).HasDefaultValue(true);
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // MaterialRequest configuration
        modelBuilder.Entity<MaterialRequestDbModel>(entity =>
        {
            entity.HasKey(mr => mr.Id);
            //entity.HasKey(mr => new { mr.RequestId, mr.MaterialId });
            entity.HasIndex(mr => mr.RequestId);
            entity.HasIndex(mr => mr.MaterialId);
            entity.Property(mr => mr.Quantity).IsRequired().HasDefaultValue(1);
            entity.Property(mr => mr.Actual).IsRequired().HasDefaultValue(0);
            entity.Property(mr => mr.Note).HasMaxLength(255);
            entity.Property(mr => mr.IsActive).HasDefaultValue(true);
            entity.Property(mr => mr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(mr => mr.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(mr => mr.Request)
                .WithMany(r => r.MaterialRequests)
                .HasForeignKey(mr => mr.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mr => mr.Material)
                .WithMany(m => m.MaterialRequests)
                .HasForeignKey(mr => mr.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // No seed data - Robot/Station loaded from config

        modelBuilder.Entity<SettingDbModel>(entity =>
        {
            entity.HasIndex(s => s.Key).IsUnique();
            entity.Property(s => s.Key).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Value).IsRequired().HasMaxLength(500);
            entity.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<TransferRequestDbModel>(entity =>
        {
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => new { r.FromStageCode, r.ToStageCode, r.Size, r.Status });
            entity.HasIndex(r => r.ExpiresAt);
            entity.HasIndex(r => new { r.Status, r.WarehousePendingKind, r.PendingWarehouseStationCode, r.CreatedAt });
            entity.Property(r => r.Type).HasConversion<int>();
            entity.Property(r => r.Size).HasConversion<int>();
            entity.Property(r => r.Source).HasConversion<int>();
            entity.Property(r => r.Status).HasConversion<int>();
            entity.Property(r => r.WarehousePendingKind).HasConversion<int>();
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }


    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var entity in entries)
        {
            if (entity is FlowTaskDbModel flowTask)
                flowTask.UpdatedAt = DateTime.UtcNow;
            else if (entity is FlowTaskWaitingSetDbModel waitingSet)
                waitingSet.UpdatedAt = DateTime.UtcNow;
            else if (entity is StationDbModel station)
                station.UpdatedAt = DateTime.UtcNow;
            else if (entity is StationTagDbModel stationTag)
                stationTag.UpdatedAt = DateTime.UtcNow;
            else if (entity is CassetteDbModel cassette)
                cassette.UpdatedAt = DateTime.UtcNow;
            else if (entity is AmrDbModel amr)
                amr.UpdatedAt = DateTime.UtcNow;
            else if (entity is MaterialDbModel material)
                material.UpdatedAt = DateTime.UtcNow;
            else if (entity is SettingDbModel setting)
                setting.UpdatedAt = DateTime.UtcNow;
            else if (entity is TransferRequestDbModel transferRequest)
                transferRequest.UpdatedAt = DateTime.UtcNow;
            else if (entity is CraneTaskDispatchDbModel craneTaskDispatch)
                craneTaskDispatch.UpdatedAt = DateTime.UtcNow;
        }
    }
}
