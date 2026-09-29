using Registration.Dtos;
using Registration.Models;

namespace Registration.Services;

public interface IUserService
{
    Task<User> AuthorizeAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(string mail, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string mail, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetUsersByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
