using Microsoft.AspNetCore.Mvc;
using Registration.Dtos;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;

namespace Registration.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserResponse>> GetByPeriod([FromQuery] UsersPeriodRequest request)
    {
        try
        {
            IReadOnlyList<User> users = _userService.GetUsersByPeriod(request.From!.Value, request.To!.Value);
            return Ok(users.Select(ToResponse));
        }
        catch (AppException ex)
        {
            return Problem(ex);
        }
    }

    [HttpPost]
    public ActionResult<UserResponse> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            User user = _userService.CreateUser(request);
            return Created($"/api/users/{user.Mail}", ToResponse(user));
        }
        catch (AppException ex)
        {
            return Problem(ex);
        }
    }

    [HttpPost("login")]
    public ActionResult<UserResponse> Login([FromBody] LoginRequest request)
    {
        try
        {
            User user = _userService.Authorize(request);
            return Ok(ToResponse(user));
        }
        catch (AppException ex)
        {
            return Problem(ex);
        }
    }

    [HttpPut("{mail}")]
    public ActionResult<UserResponse> Update(string mail, [FromBody] UpdateUserRequest request)
    {
        try
        {
            User user = _userService.UpdateUser(mail, request);
            return Ok(ToResponse(user));
        }
        catch (AppException ex)
        {
            return Problem(ex);
        }
    }

    [HttpDelete("{mail}")]
    public IActionResult Delete(string mail)
    {
        try
        {
            _userService.DeleteUser(mail);
            return NoContent();
        }
        catch (AppException ex)
        {
            return Problem(ex);
        }
    }

    private ObjectResult Problem(AppException ex) =>
        StatusCode(ex.StatusCode, new { error = ex.Message });

    private static UserResponse ToResponse(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Mail = user.Mail,
        CreatedDate = user.CreatedDate,
        UpdatedDate = user.UpdatedDate
    };
}
