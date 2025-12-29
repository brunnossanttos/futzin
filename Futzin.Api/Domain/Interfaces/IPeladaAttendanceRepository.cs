using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Enums;

namespace Futzin.Api.Domain.Interfaces;

public interface IPeladaAttendanceRepository
{
    Task<PeladaAttendance?> GetByIdAsync(int id);
    Task<PeladaAttendance?> GetByPeladaAndUserAsync(int peladaId, int userId);
    Task<IEnumerable<PeladaAttendance>> GetByPeladaIdAsync(int peladaId);
    Task<IEnumerable<PeladaAttendance>> GetByUserIdAsync(int userId);
    Task<PeladaAttendance> CreateAsync(PeladaAttendance attendance);
    Task<PeladaAttendance> UpdateAsync(PeladaAttendance attendance);
    Task UpdateStatusAsync(int id, AttendanceStatus status);
    Task DeleteAsync(int id);
}
