using Approval.Data;
using Approval.Dtos;
using Approval.Models;
using Microsoft.EntityFrameworkCore;

namespace Approval.Services;

public sealed class ApprovalService(ApprovalDbContext db) : IApprovalService
{
    public async Task<ApprovalRequest> CreateAsync(CreateApprovalRequest request, CancellationToken cancellationToken)
    {
        var approval = new ApprovalRequest
        {
            UserId = request.UserId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim()
        };
        db.Requests.Add(approval);
        await db.SaveChangesAsync(cancellationToken);
        return approval;
    }

    public async Task<IReadOnlyList<ApprovalRequest>> GetAsync(Guid? userId, CancellationToken cancellationToken)
    {
        var query = db.Requests.AsNoTracking();
        if (userId.HasValue)
        {
            query = query.Where(item => item.UserId == userId.Value);
        }

        return await query.OrderByDescending(item => item.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<ApprovalRequest> DecideAsync(
        Guid id,
        ApprovalStatus status,
        DecisionRequest request,
        CancellationToken cancellationToken)
    {
        ApprovalRequest? approval = await db.Requests.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (approval is null)
        {
            throw new KeyNotFoundException("Заявка не найдена");
        }

        if (approval.Status != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Решение по заявке уже принято");
        }

        approval.Status = status;
        approval.DecidedAt = DateTime.UtcNow;
        approval.DecisionComment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
        await db.SaveChangesAsync(cancellationToken);
        return approval;
    }
}
