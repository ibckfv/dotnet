using Approval.Dtos;
using Approval.Models;
using Approval.Services;
using Microsoft.AspNetCore.Mvc;

namespace Approval.Controllers;

[ApiController]
[Route("api/approvals")]
public sealed class ApprovalsController(IApprovalService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApprovalResponse>> Create(
        CreateApprovalRequest request,
        CancellationToken cancellationToken)
    {
        ApprovalRequest approval = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { userId = approval.UserId }, ToResponse(approval));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ApprovalResponse>>> Get(
        [FromQuery] Guid? userId,
        CancellationToken cancellationToken)
    {
        var approvals = await service.GetAsync(userId, cancellationToken);
        return Ok(approvals.Select(ToResponse));
    }

    [HttpPost("{id:guid}/approve")]
    public Task<ActionResult<ApprovalResponse>> Approve(
        Guid id, DecisionRequest request, CancellationToken cancellationToken) =>
        Decide(id, ApprovalStatus.Approved, request, cancellationToken);

    [HttpPost("{id:guid}/reject")]
    public Task<ActionResult<ApprovalResponse>> Reject(
        Guid id, DecisionRequest request, CancellationToken cancellationToken) =>
        Decide(id, ApprovalStatus.Rejected, request, cancellationToken);

    private async Task<ActionResult<ApprovalResponse>> Decide(
        Guid id,
        ApprovalStatus status,
        DecisionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(ToResponse(await service.DecideAsync(id, status, request, cancellationToken)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
    }

    private static ApprovalResponse ToResponse(ApprovalRequest request) => new()
    {
        Id = request.Id,
        UserId = request.UserId,
        Title = request.Title,
        Description = request.Description,
        Status = request.Status,
        CreatedAt = request.CreatedAt,
        DecidedAt = request.DecidedAt,
        DecisionComment = request.DecisionComment
    };
}
