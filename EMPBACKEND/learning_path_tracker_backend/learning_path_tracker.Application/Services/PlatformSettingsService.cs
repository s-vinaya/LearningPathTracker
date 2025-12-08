using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class PlatformSettingsService : IPlatformSettingsService
{
    private readonly AppDbContext _context;

    public PlatformSettingsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PlatformSettingsDto> GetSettingsAsync()
    {
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new PlatformSettings();
            _context.PlatformSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        return new PlatformSettingsDto
        {
            PlatformName = settings.PlatformName,
            CompanyName = settings.CompanyName,
            PlatformDescription = settings.PlatformDescription,
            AutoEnroll = settings.AutoEnroll,
            Certificates = settings.Certificates
        };
    }

    public async Task<PlatformSettingsDto> UpdateSettingsAsync(PlatformSettingsDto dto)
    {
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new PlatformSettings();
            _context.PlatformSettings.Add(settings);
        }

        settings.PlatformName = dto.PlatformName;
        settings.CompanyName = dto.CompanyName;
        settings.PlatformDescription = dto.PlatformDescription;
        settings.AutoEnroll = dto.AutoEnroll;
        settings.Certificates = dto.Certificates;
        settings.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return dto;
    }
}
