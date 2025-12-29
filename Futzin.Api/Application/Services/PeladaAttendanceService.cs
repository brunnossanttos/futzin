using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Enums;
using Futzin.Api.Domain.Interfaces;

namespace Futzin.Api.Application.Services;

public class PeladaAttendanceService
{
    private readonly IPeladaAttendanceRepository _repository;
    private readonly IUserStatsRepository _statsRepository;

    public PeladaAttendanceService(
        IPeladaAttendanceRepository repository,
        IUserStatsRepository statsRepository)
    {
        _repository = repository;
        _statsRepository = statsRepository;
    }

    public async Task<IEnumerable<PeladaAttendance>> GetPeladaAttendancesAsync(int peladaId)
    {
        return await _repository.GetByPeladaIdAsync(peladaId);
    }

    public async Task<IEnumerable<PeladaAttendance>> GetUserAttendancesAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<PeladaAttendance> CreateAttendanceAsync(int peladaId, int userId)
    {
        var existing = await _repository.GetByPeladaAndUserAsync(peladaId, userId);
        if (existing != null)
            throw new Exception("Confirmação de presença já existe");

        var attendance = new PeladaAttendance
        {
            PeladaId = peladaId,
            UserId = userId,
            Status = AttendanceStatus.Pending
        };

        return await _repository.CreateAsync(attendance);
    }

    public async Task<PeladaAttendance> UpdateAttendanceStatusAsync(int peladaId, int userId, AttendanceStatus status)
    {
        var attendance = await _repository.GetByPeladaAndUserAsync(peladaId, userId);
        if (attendance == null)
            throw new Exception("Confirmação de presença não encontrada");

        attendance.Status = status;
        attendance.ResponseDate = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(attendance);

        if (status == AttendanceStatus.Attended || status == AttendanceStatus.NoShow)
        {
            await _statsRepository.RecalculateStatsAsync(userId);
        }

        return updated;
    }

    public async Task<PeladaAttendance> ConfirmAttendanceAsync(int peladaId, int userId)
    {
        return await UpdateAttendanceStatusAsync(peladaId, userId, AttendanceStatus.Confirmed);
    }

    public async Task<PeladaAttendance> DeclineAttendanceAsync(int peladaId, int userId)
    {
        return await UpdateAttendanceStatusAsync(peladaId, userId, AttendanceStatus.Declined);
    }

    public async Task MarkAttendedAsync(int peladaId, int userId)
    {
        await UpdateAttendanceStatusAsync(peladaId, userId, AttendanceStatus.Attended);
    }

    public async Task MarkNoShowAsync(int peladaId, int userId)
    {
        await UpdateAttendanceStatusAsync(peladaId, userId, AttendanceStatus.NoShow);
    }
}
