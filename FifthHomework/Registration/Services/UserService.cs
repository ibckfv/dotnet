using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.Dtos;
using Registration.Infrastructure;
using Registration.Models;

namespace Registration.Services;

public class UserService(RegistrationDbContext dbContext, ILogger<UserService> logger) : IUserService
{
    public async Task<User> AuthorizeAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        User user = await FindByMailAsync(request.Mail, cancellationToken);

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login rejected: password verification failed for an existing account");
            throw new AppException("Неверный пароль", StatusCodes.Status401Unauthorized);
        }

        logger.LogInformation("User {UserId} authenticated successfully", user.Id);
        return user;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureMailIsUniqueAsync(request.Mail, null, cancellationToken);

        var user = new User
        {
            Name = request.Name.Trim(),
            Mail = NormalizeMail(request.Mail),
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        dbContext.Users.Add(user);
        await SaveChangesAsync(cancellationToken);
        logger.LogInformation("User {UserId} registered successfully", user.Id);
        return user;
    }

    public async Task<User> UpdateUserAsync(string mail, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name)
            && string.IsNullOrWhiteSpace(request.Mail)
            && string.IsNullOrWhiteSpace(request.Password))
        {
            throw new AppException("Укажите хотя бы одно поле для обновления", StatusCodes.Status400BadRequest);
        }

        User user = await FindByMailAsync(mail, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Mail))
        {
            await EnsureMailIsUniqueAsync(request.Mail, user.Id, cancellationToken);
            user.Mail = NormalizeMail(request.Mail);
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = PasswordHasher.Hash(request.Password);
        }

        user.UpdatedDate = DateTime.UtcNow;
        await SaveChangesAsync(cancellationToken);
        logger.LogInformation("User {UserId} profile updated successfully", user.Id);
        return user;
    }

    public async Task DeleteUserAsync(string mail, CancellationToken cancellationToken = default)
    {
        User user = await FindByMailAsync(mail, cancellationToken);
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("User {UserId} deleted successfully", user.Id);
    }

    public async Task<IReadOnlyList<User>> GetUsersByPeriodAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        from = ToUtc(from);
        to = ToUtc(to);

        IReadOnlyList<User> users = await dbContext.Users
            .AsNoTracking()
            .Where(user => (user.CreatedDate >= from && user.CreatedDate <= to)
                || (user.UpdatedDate >= from && user.UpdatedDate <= to))
            .OrderBy(user => user.CreatedDate)
            .ToListAsync(cancellationToken);

        logger.LogInformation("User report retrieved for period {FromUtc} through {ToUtc}: {UserCount} records",
            from, to, users.Count);
        return users;
    }

    private async Task<User> FindByMailAsync(string mail, CancellationToken cancellationToken)
    {
        string normalizedMail = NormalizeMail(mail);
        User? user = await dbContext.Users
            .SingleOrDefaultAsync(user => user.Mail == normalizedMail, cancellationToken);

        return user ?? throw new AppException("Пользователь не найден", StatusCodes.Status404NotFound);
    }

    private async Task EnsureMailIsUniqueAsync(string mail, Guid? excludeId, CancellationToken cancellationToken)
    {
        string normalizedMail = NormalizeMail(mail);
        bool exists = await dbContext.Users
            .AnyAsync(user => user.Mail == normalizedMail && user.Id != excludeId, cancellationToken);

        if (exists)
        {
            logger.LogWarning("User operation rejected because the requested email is already registered");
            throw new AppException("Пользователь с такой почтой уже существует", StatusCodes.Status409Conflict);
        }
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            logger.LogWarning(exception, "Database rejected a user change due to a unique email constraint");
            throw new AppException("Пользователь с такой почтой уже существует", StatusCodes.Status409Conflict);
        }
    }

    private static string NormalizeMail(string mail) => mail.Trim().ToLowerInvariant();

    private static DateTime ToUtc(DateTime value) =>
        value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
}
