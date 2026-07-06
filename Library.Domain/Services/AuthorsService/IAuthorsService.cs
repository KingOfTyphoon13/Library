using Library.Domain.DTOs.Authors;

namespace Library.Domain.Services.AuthorsService;

public interface IAuthorsService
{
    List<AuthorWithBooksCountDTO> GetAuthorWithBooksCounts();

    void AddAuthor(AuthorDTO newAuthor);
}
