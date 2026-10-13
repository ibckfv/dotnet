using System.ComponentModel.DataAnnotations;

namespace Approval.Dtos;

public sealed class DecisionRequest
{
    [StringLength(1000)]
    public string? Comment { get; init; }
}
