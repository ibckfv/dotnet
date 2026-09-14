namespace Registration;

public class UserService
{
    private readonly List<User> _users = new();

    public User? Authorize(string mail, string password)
    {
        User? user = _users.FirstOrDefault(u => u.Mail == mail);
        if (user is null)
        {
            throw new Exception("Пользователь не найден");
        }

        return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
    }

    public bool CreateUser(string name, string mail, string password)
    {
        if (_users.Any(u => u.Mail == mail))
        {
            return false;
        }

        _users.Add(new User
        {
            Name = name,
            Mail = mail,
            PasswordHash = PasswordHasher.Hash(password)
        });

        return true;
    }

    public bool UpdateUser(string mail, string? newName, string? newMail, string? newPassword)
    {
        User? user = _users.FirstOrDefault(u => u.Mail == mail);
        if (user is null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(newName))
        {
            user.Name = newName;
        }

        if (!string.IsNullOrEmpty(newMail))
        {
            if (_users.Any(u => u.Mail == newMail && u != user))
            {
                return false;
            }

            user.Mail = newMail;
        }

        if (!string.IsNullOrEmpty(newPassword))
        {
            user.PasswordHash = PasswordHasher.Hash(newPassword);
        }

        return true;
    }

    public bool DeleteUser(string mail)
    {
        User? user = _users.FirstOrDefault(u => u.Mail == mail);
        if (user is null)
        {
            return false;
        }

        _users.Remove(user);
        return true;
    }
}
