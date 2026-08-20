using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Services;

public class AmrService(IAmrRepository amrRepository)
{
    private readonly IAmrRepository _amrRepository = amrRepository;

    public async Task<IEnumerable<Amr>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var amrs = await _amrRepository.GetListAsync(cancellationToken);
        return amrs;
    }

    public async Task<IEnumerable<Amr>> GetListByAreaAsync(AmrByAreaListRequest request, CancellationToken cancellationToken = default)
    {
        var amrs = await _amrRepository.AmrByAreaListRequest(request.Area, cancellationToken);
        return amrs;
    }

    public async Task<Amr> ShowAmrAsync(string id, CancellationToken cancellationToken = default)
    {
        var amr = await _amrRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Amr với id {id} không tồn tại");
        return amr;
    }

    public async Task<Amr> CreateAmrAsync(AmrCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Kiểm tra mã công đoạn đã tồn tại trong khu vực chưa (với IsActive = 1)
        var area = string.IsNullOrWhiteSpace(request?.Area) ? StageArea.Chip : StageArea.FromString(request.Area);
        var existingAmr = await _amrRepository.GetByCodeAsync(request!.Code, cancellationToken);
        if (existingAmr != null && existingAmr.IsActive == false)
        {
            // Khôi phục cassette cũ
            existingAmr.Name = request.Name;
            existingAmr.Code = request.Code;
            existingAmr.Area = area;
            existingAmr.IsActive = true;
            existingAmr.UpdatedAt = DateTime.UtcNow;

            await _amrRepository.UpdateAsync(existingAmr, cancellationToken);
            return existingAmr;
        }
        if (existingAmr != null)
        {
            throw new ValidationException($"Mã AMR '{request.Code}' đã tồn tại!");
        }

        var amr = new Amr(Guid.NewGuid(), request.Name, request.Code, area);
        amr = await _amrRepository.CreateAsync(amr, cancellationToken);
        return amr;
    }

    public async Task<IEnumerable<Amr>> CreateAmrsAsync(List<AmrCreateRequest> requests, CancellationToken cancellationToken = default)
    {
        if (requests is null || requests.Count == 0)
        {
            throw new ValidationException("Không có dữ liệu gửi lên!");
            // Lấy tất cả AMR từ database và chuyển sang Deactivate
            //return await _amrRepository.DeactivateAsync(cancellationToken);
        }

        // Lấy tất cả AMR từ database để kiểm tra trùng lặp
        var lstAmr = await _amrRepository.GetAllAsync(cancellationToken);

        // =====================================================
        // List Update: Tất cả AMR requests đã có trong Database
        // =====================================================
        List<Amr> lstUpdate = [];

        // =====================================================
        // List New: Tất cả AMR requests không có trong Database
        // =====================================================
        List<Amr> lstNew = [];

        // =====================================================
        // List Deactivate: Tất cả AMR trong Database không có trong requests
        // =====================================================
        List<Amr> lstDeactivate = lstAmr.Where(amr => !requests!.Any(req => req.Code == amr.Code)).ToList();

        foreach (var robot in requests!)
        {
            var exist = lstAmr.Where(a => a.Code == robot.Code).FirstOrDefault();
            if (exist != null)
            {
                // Khôi phục robot cũ đã xóa
                exist.Code = robot.Code;
                exist.Name = robot.Name;
                exist.MapCode = robot.MapCode == null ? string.Empty : robot.MapCode;
                //exist.MapName = robot.MapName == null ? string.Empty : robot.MapName;
                exist.RobotStatus = robot!.RobotStatus == null ? string.Empty : robot.RobotStatus;
                exist.TypeCode = robot!.TypeCode == null ? string.Empty : robot.TypeCode;
                exist.Area = robot.Area == null ? StageArea.Chip : StageArea.FromString(robot.Area);
                exist.Battery = robot!.Battery == null ? string.Empty : robot.Battery;
                exist.Direction = robot!.Direction == null ? string.Empty : robot.Direction;
                exist.Exclude = robot!.Exclude == null ? string.Empty : robot.Exclude;
                exist.ExcludeStr = robot!.ExcludeStr == null ? string.Empty : robot.ExcludeStr;
                exist.OnLine = robot!.OnLine == null ? string.Empty : robot.OnLine;
                exist.PodCode = robot!.PodCode == null ? string.Empty : robot.PodCode;
                exist.PodDir = robot!.PodDir == null ? string.Empty : robot.PodDir;
                exist.PosX = robot!.PosX == null ? string.Empty : robot.PosX;
                exist.PosY = robot!.PosY == null ? string.Empty : robot.PosY;
                exist.Ip = robot!.RobotIp == null ? string.Empty : robot.RobotIp;
                exist.Status = robot!.Status == null ? string.Empty : robot.Status;
                exist.StatusStr = robot!.StatusStr == null ? string.Empty : robot.StatusStr;
                exist.Stop = robot!.Stop == null ? string.Empty : robot.Stop;
                exist.StopStr = robot!.StopStr == null ? string.Empty : robot.StopStr;
                exist.IsActive = true;
                lstUpdate.Add(exist);
            }
            else
            {
                // Nếu không có dữ liệu trùng lặp thì thêm mới
                var amr = new Amr(Guid.NewGuid(), robot.Name, robot.Code, robot.MapCode, robot.RobotStatus, robot.TypeCode);
                //amr.MapName = robot.MapName == null ? string.Empty : robot.MapName;
                amr.Area = robot.Area == null ? StageArea.Chip : StageArea.FromString(robot.Area);
                amr.Battery = robot!.Battery == null ? string.Empty : robot.Battery;
                amr.Direction = robot!.Direction == null ? string.Empty : robot.Direction;
                amr.Exclude = robot!.Exclude == null ? string.Empty : robot.Exclude;
                amr.ExcludeStr = robot!.ExcludeStr == null ? string.Empty : robot.ExcludeStr;
                amr.OnLine = robot!.OnLine == null ? string.Empty : robot.OnLine;
                amr.PodCode = robot!.PodCode == null ? string.Empty : robot.PodCode;
                amr.PodDir = robot!.PodDir == null ? string.Empty : robot.PodDir;
                amr.PosX = robot!.PosX == null ? string.Empty : robot.PosX;
                amr.PosY = robot!.PosY == null ? string.Empty : robot.PosY;
                amr.Ip = robot!.RobotIp == null ? string.Empty : robot.RobotIp;
                amr.Status = robot!.Status == null ? string.Empty : robot.Status;
                amr.StatusStr = robot!.StatusStr == null ? string.Empty : robot.StatusStr;
                amr.Stop = robot!.Stop == null ? string.Empty : robot.Stop;
                amr.StopStr = robot!.StopStr == null ? string.Empty : robot.StopStr;
                lstNew.Add(amr);
            }
        }

        if (lstUpdate.Count > 0)
            await _amrRepository.UpdateBulkAsync(lstUpdate, cancellationToken);

        if (lstNew.Count > 0)
            await _amrRepository.CreateBulkAsync(lstNew, cancellationToken);

        if (lstDeactivate.Count > 0)
            await _amrRepository.DeactivateAsync(lstDeactivate, cancellationToken);

        // Return combined created and updated AMRs
        return lstNew.Concat(lstUpdate);
    }

    public async Task<Amr> UpdateAmrAsync(string id, AmrUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var amr = await _amrRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Amr với id {id} không tồn tại");
        
        var newArea = StageArea.FromString(request.Area);
        
        // Kiểm tra mã công đoạn đã tồn tại trong khu vực chưa (nếu đổi code hoặc đổi area)
        if (amr.Code != request.Code || amr.Area.Value != newArea.Value)
        {
            /*var existingAmr = await _amrRepository.GetByCodeAndAreaAsync(request.Code, newArea, cancellationToken);
            if (existingAmr != null && existingAmr.Id != amr.Id)
            {
                throw new ValidationException($"Mã công đoạn '{request.Code}' đã tồn tại trong khu vực '{request.Area}'");
            }*/
        }

        amr.Name = request.Name;
        amr.Code = request.Code;
        amr.Area = newArea;
        amr.UpdatedAt = DateTime.UtcNow;
        await _amrRepository.UpdateAsync(amr, cancellationToken);
        return amr;
    }

    public async Task DeleteAmrAsync(string id, CancellationToken cancellationToken = default)
    {
        var amr = await _amrRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Amr với id {id} không tồn tại");
        
        // Soft delete - set Status = 0
        amr.Deactivate();
        amr.UpdatedAt = DateTime.UtcNow;
        await _amrRepository.UpdateAsync(amr, cancellationToken);
    }
}