using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class OtpCodeRepository : IOtpCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public OtpCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OtpCode>> GetAllAsync()
        {
            return await _context.OtpCodes.ToListAsync();
        }

        public async Task<OtpCode?> GetByIdAsync(int id)
        {
            return await _context.OtpCodes.FindAsync(id);
        }

        public async Task<OtpCode?> GetByEmailAndCodeAsync(string email, string code)
        {
            return await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.Email == email && o.Code == code && !o.IsUsed);
        }

        public async Task<OtpCode> CreateAsync(OtpCode otpCode)
        {
            _context.OtpCodes.Add(otpCode);
            await _context.SaveChangesAsync();
            return otpCode;
        }

        public async Task<OtpCode> UpdateAsync(OtpCode otpCode)
        {
            _context.OtpCodes.Update(otpCode);
            await _context.SaveChangesAsync();
            return otpCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var otpCode = await _context.OtpCodes.FindAsync(id);
            if (otpCode == null) return false;

            _context.OtpCodes.Remove(otpCode);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.OtpCodes.AnyAsync(o => o.Id == id);
        }
    }
}