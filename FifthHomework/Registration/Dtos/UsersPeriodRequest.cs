using System.ComponentModel.DataAnnotations;

namespace Registration.Dtos;

public class UsersPeriodRequest : IValidatableObject
{
    [Required(ErrorMessage = "Укажите начало промежутка (from)")]
    public DateTime? From { get; set; }

    [Required(ErrorMessage = "Укажите конец промежутка (to)")]
    public DateTime? To { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From.HasValue && To.HasValue && From.Value > To.Value)
        {
            yield return new ValidationResult(
                "Начало промежутка не может быть позже конца",
                [nameof(From), nameof(To)]);
        }
    }
}
