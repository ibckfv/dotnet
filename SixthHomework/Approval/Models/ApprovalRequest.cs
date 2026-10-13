namespace Approval.Models;

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}

public sealed class ApprovalRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DecidedAt { get; set; }
    public string? DecisionComment { get; set; }
}
