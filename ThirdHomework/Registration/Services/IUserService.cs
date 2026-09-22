using Registration.Dtos;
using Registration.Models;

namespace Registration.Services;

public interface IUserService
{
    User Authorize(LoginRequest request);
    User CreateUser(CreateUserRequest request);
    User UpdateUser(string mail, UpdateUserRequest request);
    void DeleteUser(string mail);
    IReadOnlyList<User> GetUsersByPeriod(DateTime from, DateTime to);
}
