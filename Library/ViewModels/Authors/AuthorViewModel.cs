using Library.Domain.Common.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Authors;

public class AuthorViewModel : IValidatableObject, IIdentifiable
{
    public int Id { get; set; }

    [Display(Name = "Name")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
    [RegularExpression(@"^[\p{L}]+([ '\-][\p{L}]+)*$",
        ErrorMessage = "Name may only contain letters, spaces, hyphens or apostrophes.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Surname")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Surname must be 2-100 characters.")]
    [RegularExpression(@"^[\p{L}]+([ '\-][\p{L}]+)*$",
        ErrorMessage = "Surname may only contain letters, spaces, hyphens or apostrophes.")]
    public string Surname { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Surname))
            yield return new ValidationResult("Either Name or Surname must be provided.", new[] { nameof(Name), nameof(Surname) });
    }
}

