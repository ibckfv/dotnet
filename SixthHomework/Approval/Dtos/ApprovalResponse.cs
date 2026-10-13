using Approval.Models;

namespace Approval.Dtos;

public sealed class ApprovalResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ApprovalStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DecidedAt { get; init; }
    public string? DecisionComment { get; init; }
}
