using EMPBACKEND.Data;
using EMPBACKEND.Interfaces.Repositories;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class VideoProgressRepository : IVideoProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VideoProgress>> GetAllAsync()
        {
            return await _context.VideoProgresses
                .Include(vp => vp.User)
                .Include(vp => vp.Video)
                .ToListAsync();
        }

        public async Task<VideoProgress?> GetByIdAsync(int id)
        {
            return await _context.VideoProgresses
                .Include(vp => vp.User)
                .Include(vp => vp.Video)
                .FirstOrDefaultAsync(vp => vp.Id == id);
        }

        public async Task<IEnumerable<VideoProgress>> GetByUserIdAsync(int userId)
        {
            return await _context.VideoProgresses
                .Include(vp => vp.User)
                .Include(vp => vp.Video)
                .Where(vp => vp.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VideoProgress>> GetByVideoIdAsync(int videoId)
        {
            return await _context.VideoProgresses
                .Include(vp => vp.User)
                .Include(vp => vp.Video)
                .Where(vp => vp.VideoId == videoId)
                .ToListAsync();
        }

        public async Task<VideoProgress?> GetByUserAndVideoAsync(int userId, int videoId)
        {
            return await _context.VideoProgresses
                .Include(vp => vp.User)
                .Include(vp => vp.Video)
                .FirstOrDefaultAsync(vp => vp.UserId == userId && vp.VideoId == videoId);
        }

        public async Task<VideoProgress> CreateAsync(VideoProgress videoProgress)
        {
            _context.VideoProgresses.Add(videoProgress);
            await _context.SaveChangesAsync();
            return videoProgress;
        }

        public async Task<VideoProgress> UpdateAsync(VideoProgress videoProgress)
        {
            var existingVideoProgress = await _context.VideoProgresses.FindAsync(videoProgress.Id);
            if (existingVideoProgress == null) throw new ArgumentException("VideoProgress not found");

            existingVideoProgress.UserId = videoProgress.UserId;
            existingVideoProgress.VideoId = videoProgress.VideoId;
            existingVideoProgress.WatchedDuration = videoProgress.WatchedDuration;
            existingVideoProgress.LastWatchedDate = videoProgress.LastWatchedDate;
            existingVideoProgress.IsCompleted = videoProgress.IsCompleted;

            await _context.SaveChangesAsync();
            return existingVideoProgress;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var videoProgress = await _context.VideoProgresses.FindAsync(id);
            if (videoProgress == null) return false;

            _context.VideoProgresses.Remove(videoProgress);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.VideoProgresses.AnyAsync(vp => vp.Id == id);
        }
    }
}