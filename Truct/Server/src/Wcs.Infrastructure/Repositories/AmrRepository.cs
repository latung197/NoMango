using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Repositories;

public class AmrRepository(WcsDbContext context) : IAmrRepository
{
    private readonly WcsDbContext _context = context;

    /// <summary>
    /// Lấy tất cả AMR (bao gồm cả inactive)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Amr>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Amrs.AsNoTracking()
            .OrderBy(s => s.Area)
            .ThenBy(s => s.Code)
            .ThenBy(s => s.Name)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lấy danh sách AMR không phân trang (chỉ lấy active)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Amr>> GetListAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<AmrDbModel> query = _context.Amrs.AsNoTracking()
            .Where(s => s.IsActive); // Chỉ lấy amr đang active
        return await query
            .OrderBy(s => s.Area)
            .ThenBy(s => s.Code)
            .ThenBy(s => s.Name)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lấy AMR active theo area (chỉ lấy active)
    /// </summary>
    /// <param name="area"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Amr>> AmrByAreaListRequest(string? area, CancellationToken cancellationToken = default)
    {
        var amrModels = await _context.Amrs.AsNoTracking()
                                .Where(s => s.IsActive && (string.IsNullOrWhiteSpace(area) || s.Area == StageArea.FromString(area)))
                                .OrderBy(s => s.Area)
                                .ThenBy(s => s.Code)
                                .ThenBy(s => s.Name)
                                .Select(dbModel => dbModel.ToDomain())
                                .ToListAsync(cancellationToken);

        return amrModels;
    }

    /// <summary>
    /// Đếm số AMR active
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Amrs.Where(s => s.IsActive).CountAsync(cancellationToken);
    }

    /// <summary>
    /// Lấy AMR theo code (bao gồm cả inactive)
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Amr?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Amrs.AsNoTracking()
                        .Where(s => s.Code == code)
                        //.Where(s => s.Code == code && s.IsActive)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    /// <summary>
    /// Lấy AMR theo code và area (bao gồm cả inactive)
    /// </summary>
    /// <param name="code"></param>
    /// <param name="area"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Amr?> GetByCodeAndAreaAsync(string code, StageArea area, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Amrs.AsNoTracking()
                        .Where(s => s.Code == code && s.Area == area)
                        //.Where(s => s.Code == code && s.Area == area && s.IsActive)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    /// <summary>
    /// Lấy AMR theo Id (bao gồm cả inactive)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Amr?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Amrs.AsNoTracking()
                        .Where(s => s.Id == id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    /// <summary>
    /// Tạo mới AMR
    /// </summary>
    /// <param name="amr"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Amr> CreateAsync(Amr amr, CancellationToken cancellationToken = default)
    {
        _context.Amrs.Add(amr.ToDbModel());
        await _context.SaveChangesAsync(cancellationToken);
        return amr;
    }

    /// <summary>
    /// Tạo mới AMRs
    /// </summary>
    /// <param name="amrs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Amr>> CreateBulkAsync(IEnumerable<Amr> amrs, CancellationToken cancellationToken = default)
    {
        var dbModels = amrs.Select(r => r.ToDbModel()).ToList();
        _context.Amrs.AddRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModels.Select(db => db.ToDomain());
    }

    /// <summary>
    /// Cập nhật AMR
    /// </summary>
    /// <param name="amr"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task UpdateAsync(Amr amr, CancellationToken cancellationToken = default)
    {
        var existingAmr = await _context.Amrs.FirstOrDefaultAsync(s => s.Id == amr.Id, cancellationToken);
        if (existingAmr is null)
        {
            throw new InvalidOperationException($"Amr với id {amr.Id} không tồn tại");
        }

        existingAmr.Name = amr.Name;
        existingAmr.Code = amr.Code;
        existingAmr.Area = amr.Area;
        existingAmr.IsActive = amr.IsActive;
        existingAmr.UpdatedAt = amr.UpdatedAt;
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Cập nhật AMR
    /// </summary>
    /// <param name="amrs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateBulkAsync(IEnumerable<Amr> amrs, CancellationToken cancellationToken = default)
    {
        var dbModels = amrs.Select(db => db.ToDbModel());
        _context.Amrs.UpdateRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deactivate AMR
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Amr>> DeactivateAsync(IEnumerable<Amr> amrs, CancellationToken cancellationToken = default)
    {
        foreach (var robot in amrs)
        {
            robot.IsActive = false;
        }
        var dbModels = amrs.Select(db => db.ToDbModel());
        _context.Amrs.UpdateRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
        //return [];
        return dbModels.Select(db => db.ToDomain());
    }

    /// <summary>
    /// Deactivate AMRs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Amr>> DeactivateAsync(CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.Amrs
            .ToListAsync(cancellationToken);
        foreach (var robot in dbModels)
        {
            robot.IsActive = false;
        }
        _context.Amrs.UpdateRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModels.Select(db => db.ToDomain());
    }
}