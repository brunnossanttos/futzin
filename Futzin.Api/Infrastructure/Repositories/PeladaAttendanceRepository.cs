using Microsoft.EntityFrameworkCore;
using Futzin.Api.Domain.Entities;
using Futzin.Api.Domain.Enums;
using Futzin.Api.Domain.Interfaces;
using Futzin.Api.Infrastructure.Data;

namespace Futzin.Api.Infrastructure.Repositories;

public class PeladaAttendanceRepository : IPeladaAttendanceRepository
{
    private readonly FutzinDbContext _context;

    public PeladaAttendanceRepository(FutzinDbContext context)
    {
        _context = context;
    }

    public async Task<PeladaAttendance?> GetByIdAsync(int id)
    {
        return await _context.PeladaAttendances
            .Include(a => a.Pelada)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<PeladaAttendance?> GetByPeladaAndUserAsync(int peladaId, int userId)
    {
        return await _context.PeladaAttendances
            .Include(a => a.Pelada)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.PeladaId == peladaId && a.UserId == userId);
    }

    public async Task<IEnumerable<PeladaAttendance>> GetByPeladaIdAsync(int peladaId)
    {
        return await _context.PeladaAttendances
            .Include(a => a.User)
            .Where(a => a.PeladaId == peladaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PeladaAttendance>> GetByUserIdAsync(int userId)
    {
        return await _context.PeladaAttendances
            .Include(a => a.Pelada)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Pelada.Date)
            .ToListAsync();
    }

    public async Task<PeladaAttendance> CreateAsync(PeladaAttendance attendance)
    {
        _context.PeladaAttendances.Add(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task<PeladaAttendance> UpdateAsync(PeladaAttendance attendance)
    {
        attendance.UpdatedAt = DateTime.UtcNow;
        _context.PeladaAttendances.Update(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }

    public async Task UpdateStatusAsync(int id, AttendanceStatus status)
    {
        var attendance = await _context.PeladaAttendances.FindAsync(id);
        if (attendance != null)
        {
            attendance.Status = status;
            attendance.ResponseDate = DateTime.UtcNow;
            attendance.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var attendance = await _context.PeladaAttendances.FindAsync(id);
        if (attendance != null)
        {
            _context.PeladaAttendances.Remove(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
