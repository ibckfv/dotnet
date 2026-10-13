using System.ComponentModel.DataAnnotations;

namespace Approval.Dtos;

public sealed class CreateApprovalRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required, StringLength(200, MinimumLength = 3)]
    public string Title { get; init; } = string.Empty;

    [Required, StringLength(4000, MinimumLength = 1)]
    public string Description { get; init; } = string.Empty;
}
