namespace Library.Domain.DTOs.Books;

public class BookDTO : BaseDTO
{
    public string Title { get; set; } = string.Empty;

    public int PublicationYear { get; set; }
}
