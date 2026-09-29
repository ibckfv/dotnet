using System.ComponentModel.DataAnnotations;

namespace Registration.Dtos;

public class LoginRequest
{
    [Required(ErrorMessage = "Почта обязательна")]
    [EmailAddress(ErrorMessage = "Некорректный формат почты")]
    public string Mail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}
