using Library.Validation;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels.Reviews;

public class ReviewBaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Book")]
    [Required(ErrorMessage = "Please select a book.")]
    public int BookId { get; set; }

    [Display(Name = "Score")]
    [ScoreRange]
    public int Score { get; set; }
}