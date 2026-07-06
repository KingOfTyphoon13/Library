namespace Library.Domain.DTOs.Reviews;

public class ReviewDTO : BaseDTO
{
    public int BookId { get; set; }

    public int Score { get; set; }
}
