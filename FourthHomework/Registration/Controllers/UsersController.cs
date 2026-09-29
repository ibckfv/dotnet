using Microsoft.AspNetCore.Mvc;
using Registration.Dtos;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;

namespace Registration.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetByPeriod(
        [FromQuery] UsersPeriodRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<User> users = await userService.GetUsersByPeriodAsync(
                request.From!.Value,
                request.To!.Value,
                cancellationToken);
            return Ok(users.Select(ToResponse));
        }
        catch (AppException exception)
        {
            return Problem(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            User user = await userService.CreateUserAsync(request, cancellationToken);
            return Created($"/api/users/{user.Mail}", ToResponse(user));
        }
        catch (AppException exception)
        {
            return Problem(exception);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            User user = await userService.AuthorizeAsync(request, cancellationToken);
            return Ok(ToResponse(user));
        }
        catch (AppException exception)
        {
            return Problem(exception);
        }
    }

    [HttpPut("{mail}")]
    public async Task<ActionResult<UserResponse>> Update(
        string mail,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            User user = await userService.UpdateUserAsync(mail, request, cancellationToken);
            return Ok(ToResponse(user));
        }
        catch (AppException exception)
        {
            return Problem(exception);
        }
    }

    [HttpDelete("{mail}")]
    public async Task<IActionResult> Delete(string mail, CancellationToken cancellationToken)
    {
        try
        {
            await userService.DeleteUserAsync(mail, cancellationToken);
            return NoContent();
        }
        catch (AppException exception)
        {
            return Problem(exception);
        }
    }

    private ObjectResult Problem(AppException exception) =>
        StatusCode(exception.StatusCode, new { error = exception.Message });

    private static UserResponse ToResponse(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Mail = user.Mail,
        CreatedDate = user.CreatedDate,
        UpdatedDate = user.UpdatedDate
    };
}
