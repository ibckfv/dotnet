using Approval.Dtos;
using Approval.Models;

namespace Approval.Services;

public interface IApprovalService
{
    Task<ApprovalRequest> CreateAsync(CreateApprovalRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ApprovalRequest>> GetAsync(Guid? userId, CancellationToken cancellationToken);
    Task<ApprovalRequest> DecideAsync(Guid id, ApprovalStatus status, DecisionRequest request, CancellationToken cancellationToken);
}
