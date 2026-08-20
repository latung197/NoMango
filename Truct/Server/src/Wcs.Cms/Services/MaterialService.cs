using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Exceptions;

namespace Wcs.Cms.Services;

public class MaterialService(IMaterialRepository materialRepository)
{
    private readonly IMaterialRepository _materialRepository = materialRepository;

    /// <summary>
    /// Lấy tất cả materials (chỉ lấy những material có IsActive = true)
    /// </summary>
    public async Task<IEnumerable<Material>> GetAllAsync(MaterialListRequest request, CancellationToken cancellationToken = default)
    {
        return await _materialRepository.GetAllAsync(request.Name, request.Code, cancellationToken);
    }

    /// <summary>
    /// Lấy chi tiết material theo id (chỉ lấy material có IsActive = true)
    /// </summary>
    public async Task<Material> ShowMaterialAsync(string id, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Material với id {id} không tồn tại");
        return material;
    }

    /// <summary>
    /// Lấy chi tiết material theo code
    /// </summary>
    public async Task<Material?> GetStationByCode(string code, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetByCodeAsync(code, cancellationToken)
            ?? throw new NotFoundException($"Material với mã {code} không tồn tại");
        return material;
    }

    /// <summary>
    /// Tạo material mới (IsActive mặc định là true)
    /// </summary>
    public async Task<Material> CreateMaterialAsync(MaterialCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Kiểm tra code đã tồn tại chưa (trong các material active)
        var existingActive = await _materialRepository.GetActiveByCodeAsync(request.Code, cancellationToken);
        if (existingActive != null)
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Material với mã {request.Code} đã tồn tại");
        }

        // Kiểm tra xem có material cũ với IsActive = 0 và cùng code không
        // Nếu có, khôi phục nó thay vì tạo mới
        var existingInactive = await _materialRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingInactive != null && existingInactive.IsActive == false)
        {
            // Khôi phục material cũ
            existingInactive.Name = request.Name;
            existingInactive.Code = request.Code;
            existingInactive.Unit = request.Unit;
            existingInactive.Note = request.Note;
            existingInactive.IsActive = true;
            existingInactive.UpdatedAt = DateTime.UtcNow;
            
            await _materialRepository.UpdateAsync(existingInactive, cancellationToken);
            return existingInactive;
        }

        // Tạo material mới
        var material = new Material(
            name: request.Name,
            code: request.Code,
            note: request.Note,
            unit: request.Unit,
            isActive: true  // Mặc định là true khi tạo mới
        );
        
        try
        {
            material = await _materialRepository.CreateAsync(material, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Handle database constraint violations (e.g., unique index)
            if (ex.InnerException?.Message.Contains("IX_Materials_Code_IsActive") == true || 
                ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Material với mã {request.Code} đã tồn tại");
            }
            throw;
        }
        
        return material;
    }

    /// <summary>
    /// Cập nhật material
    /// </summary>
    public async Task<Material> UpdateMaterialAsync(string id, MaterialUpdateRequest request, CancellationToken cancellationToken = default)
    {
        // Cho phép update cả material có IsActive = 0 (để có thể khôi phục)
        var material = await _materialRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Material với id {id} không tồn tại");
        
        // Kiểm tra nếu đổi code hoặc đang khôi phục (IsActive từ 0 -> 1)
        // thì code mới có trùng với material active khác không (trừ chính nó)
        if (material.Code != request.Code || (material.IsActive == false && request.IsActive == true))
        {
            var existing = await _materialRepository.GetActiveByCodeAsync(request.Code, cancellationToken);
            // Nếu tìm thấy một material active khác với code này (không phải chính nó)
            if (existing != null && existing.Id != material.Id)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Material với mã {request.Code} đã tồn tại");
            }
        }

        material.Name = request.Name;
        material.Code = request.Code;
        material.Unit = request.Unit;
        material.Note = request.Note;
        material.IsActive = request.IsActive;
        material.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _materialRepository.UpdateAsync(material, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Handle database constraint violations (e.g., unique index)
            // Chỉ báo lỗi nếu có material active khác với code này
            if (ex.InnerException?.Message.Contains("IX_Materials_Code_IsActive") == true || 
                ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Material với mã {request.Code} đã tồn tại");
            }
            throw;
        }
        
        return material;
    }

    /// <summary>
    /// Xóa material update sản phẩn = 0
    /// </summary>
    public async Task DeleteMaterialAsync(string id, CancellationToken cancellationToken = default)
    {
        var material = await _materialRepository.GetActiveByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Material với id {id} không tồn tại");
        
        await _materialRepository.SoftDeleteAsync(material.Id, cancellationToken);
    }
}
