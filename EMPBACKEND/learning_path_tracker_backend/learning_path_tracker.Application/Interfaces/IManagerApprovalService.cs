using learning_path_tracker.Application.DTOs;

namespace learning_path_tracker.Application.Interfaces;

public interface IManagerApprovalService
{
    Task<List<ApprovalResponseDto>> GetPendingApprovalsAsync(int managerId);
    Task<List<ApprovalHistoryDto>> GetApprovalHistoryAsync(int managerId);
    Task<ApprovalHistoryDto?> GetApprovalByIdAsync(int managerId, int approvalId);
    Task<bool> SubmitApprovalDecisionAsync(int managerId, ApprovalDecisionDto decision);
}
