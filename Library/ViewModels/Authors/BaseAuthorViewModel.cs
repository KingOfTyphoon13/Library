using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Authors;

public abstract class BaseAuthorViewModel
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
}