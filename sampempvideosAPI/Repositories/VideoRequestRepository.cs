using EMPBACKEND.Data;
using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class VideoRequestRepository : IVideoRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VideoRequest>> GetAllAsync()
        {
            return await _context.VideoRequests
                .Include(vr => vr.User)
                .OrderByDescending(vr => vr.RequestedDate)
                .ToListAsync();
        }

        public async Task<VideoRequest?> GetByIdAsync(int id)
        {
            return await _context.VideoRequests
                .Include(vr => vr.User)
                .FirstOrDefaultAsync(vr => vr.Id == id);
        }

        public async Task<IEnumerable<VideoRequest>> GetByUserIdAsync(int userId)
        {
            return await _context.VideoRequests
                .Where(vr => vr.UserId == userId)
                .OrderByDescending(vr => vr.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<VideoRequest>> GetByStatusAsync(string status)
        {
            return await _context.VideoRequests
                .Where(vr => vr.Status == status)
                .Include(vr => vr.User)
                .OrderByDescending(vr => vr.RequestedDate)
                .ToListAsync();
        }

        public async Task<VideoRequest> CreateAsync(VideoRequest videoRequest)
        {
            _context.VideoRequests.Add(videoRequest);
            await _context.SaveChangesAsync();
            return videoRequest;
        }

        public async Task<VideoRequest> UpdateAsync(VideoRequest videoRequest)
        {
            _context.Entry(videoRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return videoRequest;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var videoRequest = await _context.VideoRequests.FindAsync(id);
            if (videoRequest == null) return false;

            _context.VideoRequests.Remove(videoRequest);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}