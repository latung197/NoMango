using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Services;

public class CassetteService(ICassetteRepository cassetteRepository)
{
    private readonly ICassetteRepository _cassetteRepository = cassetteRepository;

    /// <summary>
    /// Lấy tất cả cassettes (chỉ lấy những cassette có IsActive = true)
    /// </summary>
    public async Task<IEnumerable<Cassette>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cassetteRepository.GetAllActiveAsync(cancellationToken);
    }

    /// <summary>
    /// Lấy chi tiết cassette theo id (chỉ lấy cassette có IsActive = true)
    /// </summary>
    public async Task<Cassette> ShowCassetteAsync(string id, CancellationToken cancellationToken = default)
    {
        var cassette = await _cassetteRepository.GetActiveByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Cassette với id {id} không tồn tại");
        return cassette;
    }

    /// <summary>
    /// Lấy chi tiết cassette theo code
    /// </summary>
    public async Task<Cassette?> GetStationByCode(string code, CancellationToken cancellationToken = default)
    {
        var cassette = await _cassetteRepository.GetByCodeAsync(code, cancellationToken)
            ?? throw new NotFoundException($"Cassette với mã {code} không tồn tại");
        return cassette;
    }

    /// <summary>
    /// Tạo cassette mới (IsActive mặc định là true)
    /// </summary>
    public async Task<Cassette> CreateCassetteAsync(CassetteCreateRequest request, CancellationToken cancellationToken = default)
    {
        var size = ParseSize(request.Size);

        // Kiểm tra code đã tồn tại chưa (trong các cassette active)
        var existingActive = await _cassetteRepository.GetActiveByCodeAsync(request.Code, cancellationToken);
        if (existingActive != null)
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Cassette với mã {request.Code} đã tồn tại");
        }

        // Kiểm tra xem có cassette cũ với IsActive = 0 và cùng code không
        // Nếu có, khôi phục nó thay vì tạo mới
        var existingInactive = await _cassetteRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingInactive != null && existingInactive.IsActive == false)
        {
            existingInactive.Name = request.Name;
            existingInactive.Code = request.Code;
            existingInactive.Product = request.Product;
            existingInactive.Size = size;
            existingInactive.Capacity = request.Capacity;
            existingInactive.Quantity = request.Quantity;
            existingInactive.IsActive = true;
            existingInactive.UpdatedAt = DateTime.UtcNow;
            
            await _cassetteRepository.UpdateAsync(existingInactive, cancellationToken);
            return existingInactive;
        }

        var cassette = new Cassette(
            name: request.Name,
            code: request.Code,
            product: request.Product,
            size: size,
            capacity: request.Capacity,
            quantity: request.Quantity,
            isActive: true
        );
        
        try
        {
            cassette = await _cassetteRepository.CreateAsync(cassette, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_Cassettes_Code_IsActive") == true || 
                ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Cassette với mã {request.Code} đã tồn tại");
            }
            throw;
        }
        
        return cassette;
    }

    /// <summary>
    /// Cập nhật cassette
    /// </summary>
    public async Task<Cassette> UpdateCassetteAsync(string id, CassetteUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var size = ParseSize(request.Size);

        var cassette = await _cassetteRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Cassette với id {id} không tồn tại");
        
        if (cassette.Code != request.Code || (cassette.IsActive == false && request.IsActive == true))
        {
            var existing = await _cassetteRepository.GetActiveByCodeAsync(request.Code, cancellationToken);
            if (existing != null && existing.Id != cassette.Id)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Cassette với mã {request.Code} đã tồn tại");
            }
        }

        cassette.Name = request.Name;
        cassette.Code = request.Code;
        cassette.Product = request.Product;
        cassette.Size = size;
        cassette.Capacity = request.Capacity;
        cassette.Quantity = request.Quantity;
        cassette.IsActive = request.IsActive;
        cassette.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _cassetteRepository.UpdateAsync(cassette, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_Cassettes_Code_IsActive") == true || 
                ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Cassette với mã {request.Code} đã tồn tại");
            }
            throw;
        }
        
        return cassette;
    }

    /// <summary>
    /// Xóa cassette update sản phẩn = 0
    /// </summary>
    public async Task DeleteCassetteAsync(string id, CancellationToken cancellationToken = default)
    {
        var cassette = await _cassetteRepository.GetActiveByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Cassette với id {id} không tồn tại");
        
        await _cassetteRepository.SoftDeleteAsync(cassette.Id, cancellationToken);
    }

    private static Size ParseSize(int sizeValue)
    {
        if (!Enum.IsDefined(typeof(Size), sizeValue))
        {
            throw new InvalidOperationException($"Size không hợp lệ: {sizeValue}");
        }

        return (Size)sizeValue;
    }
}
