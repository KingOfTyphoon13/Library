using Library.Validation;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Books;

public class BookBaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Title")]
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be 1-200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Publication Year")]
    [PublicationYear]
    public int PublicationYear { get; set; }
}
