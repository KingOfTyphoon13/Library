using System.ComponentModel.DataAnnotations;

namespace Library.Validation;

public class PublicationYearAttribute : ValidationAttribute
{
    private int _minPublicationYear { get; set; } = -5000;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not int year)
            return ValidationResult.Success;

        if (year < _minPublicationYear || year > DateTime.Today.Year)
            return new ValidationResult($"Publication Year must be between {_minPublicationYear} BCE and {DateTime.Today.Year} CE.");

        return ValidationResult.Success;
    }
}