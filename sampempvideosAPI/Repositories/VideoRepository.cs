using EMPBACKEND.Data;
using EMPBACKEND.Interfaces;
using EMPBACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace EMPBACKEND.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Video>> GetAllAsync()
        {
            return await _context.Videos
                .Include(v => v.Course)
                .ToListAsync();
        }

        public async Task<Video?> GetByIdAsync(int id)
        {
            return await _context.Videos
                .Include(v => v.Course)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Video>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Videos
                .Where(v => v.CourseId == courseId)
                .Include(v => v.Course)
                .ToListAsync();
        }

        public async Task<Video> CreateAsync(Video video)
        {
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<Video> UpdateAsync(Video video)
        {
            _context.Entry(video).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video == null) return false;

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Videos.AnyAsync(v => v.Id == id);
        }
    }
}