using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Authors;

public class AuthorViewModel : BaseAuthorViewModel, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Surname))
            yield return new ValidationResult("Either Name or Surname must be provided.", new[] { nameof(Name), nameof(Surname) });
    }
}
