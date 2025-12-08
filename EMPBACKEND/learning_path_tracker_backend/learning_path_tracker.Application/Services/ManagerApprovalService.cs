using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning_path_tracker.Application.Services;

public class ManagerApprovalService : IManagerApprovalService
{
    private readonly AppDbContext _context;

    public ManagerApprovalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApprovalResponseDto>> GetPendingApprovalsAsync(int managerId)
    {
        return await _context.Set<Approval>()
            .Include(a => a.Employee)
            .Where(a => a.ManagerId == managerId && a.Status == "Pending")
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ApprovalResponseDto
            {
                Id = a.Id,
                Type = a.Type,
                Status = a.Status,
                EmployeeName = a.Employee.FullName,
                Payload = a.Payload,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<ApprovalHistoryDto>> GetApprovalHistoryAsync(int managerId)
    {
        return await _context.Set<Approval>()
            .Include(a => a.Employee)
            .Where(a => a.ManagerId == managerId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new ApprovalHistoryDto
            {
                Id = a.Id,
                Type = a.Type,
                Status = a.Status,
                EmployeeName = a.Employee.FullName,
                Payload = a.Payload,
                ReviewerComments = a.ReviewerComments,
                CreatedAt = a.CreatedAt,
                ReviewedAt = a.ReviewedAt
            })
            .ToListAsync();
    }

    public async Task<ApprovalHistoryDto?> GetApprovalByIdAsync(int managerId, int approvalId)
    {
        return await _context.Set<Approval>()
            .Include(a => a.Employee)
            .Where(a => a.Id == approvalId && a.ManagerId == managerId)
            .Select(a => new ApprovalHistoryDto
            {
                Id = a.Id,
                Type = a.Type,
                Status = a.Status,
                EmployeeName = a.Employee.FullName,
                Payload = a.Payload,
                ReviewerComments = a.ReviewerComments,
                CreatedAt = a.CreatedAt,
                ReviewedAt = a.ReviewedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SubmitApprovalDecisionAsync(int managerId, ApprovalDecisionDto decision)
    {
        var approval = await _context.Set<Approval>()
            .FirstOrDefaultAsync(a => a.Id == decision.ApprovalId && a.ManagerId == managerId);

        if (approval == null || approval.Status != "Pending")
            return false;

        approval.Status = decision.Status;
        approval.ReviewerComments = decision.ReviewerComments;
        approval.ReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
