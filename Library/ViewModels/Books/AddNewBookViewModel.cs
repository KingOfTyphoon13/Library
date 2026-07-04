using Library.ViewModels.Authors;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Books;

public class AuthorSlotViewModel
{
    public int? ExistingAuthorId { get; set; }
    public AuthorViewModel? NewAuthor { get; set; }
}

public class AddNewBookViewModel : BookBaseViewModel, IValidatableObject
{
    public List<AuthorSlotViewModel> Authors { get; set; } = [];

    [BindNever]
    public List<AuthorViewModel> AvailableAuthors { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {

        var filledCount = Authors.Count(a => a.ExistingAuthorId.HasValue
            || !string.IsNullOrWhiteSpace(a.NewAuthor?.Name)
            || !string.IsNullOrWhiteSpace(a.NewAuthor?.Surname));

        if (filledCount == 0)
            yield return new ValidationResult("At least one author is required.", new[] { nameof(Authors) });
        if (filledCount > 5)
            yield return new ValidationResult("A book can have at most 5 authors.", new[] { nameof(Authors) });

        var existingIds = Authors.Where(a => a.ExistingAuthorId.HasValue).Select(a => a.ExistingAuthorId!.Value);
        if (existingIds.Distinct().Count() != existingIds.Count())
            yield return new ValidationResult("The same author is selected more than once.", new[] { nameof(Authors) });
    }
}