using System.ComponentModel.DataAnnotations;

namespace Registration.Dtos;

public class UpdateUserRequest
{
    [MinLength(2, ErrorMessage = "Имя должно содержать минимум 2 символа")]
    [MaxLength(100, ErrorMessage = "Имя слишком длинное")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Некорректный формат почты")]
    public string? Mail { get; set; }

    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string? Password { get; set; }
}
