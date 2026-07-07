using Library.Domain.DTOs.Books;

namespace Library.Domain.DTOs.Reviews;

public class ReviewWithBookInfoDTO : ReviewDTO
{
    public BookWithAuthorsDTO BookInfo { get; set; } = new();
}
