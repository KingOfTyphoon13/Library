using Library.Domain.DTOs.Authors;

namespace Library.Domain.DTOs.Books;

public class BookWithAuthorsDTO : BookDTO
{
    public List<AuthorDTO> Authors { get; set; } = [];
}
