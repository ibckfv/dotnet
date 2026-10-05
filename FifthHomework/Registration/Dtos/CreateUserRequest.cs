using System.ComponentModel.DataAnnotations;

namespace Registration.Dtos;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Имя обязательно")]
    [MinLength(2, ErrorMessage = "Имя должно содержать минимум 2 символа")]
    [MaxLength(100, ErrorMessage = "Имя слишком длинное")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Почта обязательна")]
    [EmailAddress(ErrorMessage = "Некорректный формат почты")]
    public string Mail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; } = string.Empty;
}
