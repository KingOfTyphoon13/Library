using System.ComponentModel.DataAnnotations;

namespace Library.Validation;

public class ScoreRangeAttribute : ValidationAttribute
{
    private readonly int _min;
    private readonly int _max;

    public ScoreRangeAttribute(int min = 0, int max = 100)
    {
        _min = min;
        _max = max;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not int score) return ValidationResult.Success;
        return score < _min || score > _max
            ? new ValidationResult($"Score must be between {_min} and {_max}.")
            : ValidationResult.Success;
    }
}
