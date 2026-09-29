using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.Dtos;
using Registration.Exceptions;
using Registration.Infrastructure;
using Registration.Models;

namespace Registration.Services;

public class UserService(RegistrationDbContext dbContext) : IUserService
{
    public async Task<User> AuthorizeAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        User user = await FindByMailAsync(request.Mail, cancellationToken);

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AppException("Неверный пароль", StatusCodes.Status401Unauthorized);
        }

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
        return user;
    }

    public async Task DeleteUserAsync(string mail, CancellationToken cancellationToken = default)
    {
        User user = await FindByMailAsync(mail, cancellationToken);
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetUsersByPeriodAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        from = ToUtc(from);
        to = ToUtc(to);

        return await dbContext.Users
            .AsNoTracking()
            .Where(user => (user.CreatedDate >= from && user.CreatedDate <= to)
                || (user.UpdatedDate >= from && user.UpdatedDate <= to))
            .OrderBy(user => user.CreatedDate)
            .ToListAsync(cancellationToken);
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
            throw new AppException("Пользователь с такой почтой уже существует", StatusCodes.Status409Conflict);
        }
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new AppException("Пользователь с такой почтой уже существует", StatusCodes.Status409Conflict);
        }
    }

    private static string NormalizeMail(string mail) => mail.Trim().ToLowerInvariant();

    private static DateTime ToUtc(DateTime value) =>
        value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
}
