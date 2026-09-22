using Registration.Dtos;
using Registration.Exceptions;
using Registration.Infrastructure;
using Registration.Models;

namespace Registration.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public UserService()
    {
        CreateUser(new CreateUserRequest
        {
            Name = "Саша",
            Mail = "blabla@gmail.com",
            Password = "123456"
        });

        CreateUser(new CreateUserRequest
        {
            Name = "Даша",
            Mail = "abab@mail.ru",
            Password = "qwerty"
        });
    }

    public User Authorize(LoginRequest request)
    {
        User user = FindByMail(request.Mail);

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AppException("Неверный пароль", StatusCodes.Status401Unauthorized);
        }

        return user;
    }

    public User CreateUser(CreateUserRequest request)
    {
        EnsureMailIsUnique(request.Mail);

        var user = new User
        {
            Name = request.Name.Trim(),
            Mail = NormalizeMail(request.Mail),
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        _users.Add(user);
        return user;
    }

    public User UpdateUser(string mail, UpdateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)
            && string.IsNullOrWhiteSpace(request.Mail)
            && string.IsNullOrWhiteSpace(request.Password))
        {
            throw new AppException("Укажите хотя бы одно поле для обновления", StatusCodes.Status400BadRequest);
        }

        User user = FindByMail(mail);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Mail))
        {
            EnsureMailIsUnique(request.Mail, user.Id);
            user.Mail = NormalizeMail(request.Mail);
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = PasswordHasher.Hash(request.Password);
        }

        user.UpdatedDate = DateTime.UtcNow;
        return user;
    }

    public void DeleteUser(string mail)
    {
        User user = FindByMail(mail);
        _users.Remove(user);
    }

    public IReadOnlyList<User> GetUsersByPeriod(DateTime from, DateTime to)
    {
        from = ToUtc(from);
        to = ToUtc(to);

        return _users
            .Where(u => IsInPeriod(u.CreatedDate, from, to) || IsInPeriod(u.UpdatedDate, from, to))
            .OrderBy(u => u.CreatedDate)
            .ToList();
    }

    private User FindByMail(string mail)
    {
        string normalized = NormalizeMail(mail);
        User? user = _users.FirstOrDefault(u => u.Mail == normalized);

        if (user is null)
        {
            throw new AppException("Пользователь не найден", StatusCodes.Status404NotFound);
        }

        return user;
    }

    private void EnsureMailIsUnique(string mail, Guid? excludeId = null)
    {
        string normalized = NormalizeMail(mail);
        bool exists = _users.Any(u => u.Mail == normalized && u.Id != excludeId);

        if (exists)
        {
            throw new AppException("Пользователь с такой почтой уже существует", StatusCodes.Status409Conflict);
        }
    }

    private static string NormalizeMail(string mail) => mail.Trim().ToLowerInvariant();

    private static DateTime ToUtc(DateTime value) =>
        value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();

    private static bool IsInPeriod(DateTime date, DateTime from, DateTime to) =>
        date >= from && date <= to;
}
