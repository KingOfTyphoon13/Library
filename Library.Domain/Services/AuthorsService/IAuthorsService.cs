using Library.Domain.DTOs.Authors;

namespace Library.Domain.Services.AuthorsService;

public interface IAuthorsService
{
    Task<List<AuthorDTO>> GetAuthorsAsync();

    Task<List<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync();

    Task AddAuthorAsync(AuthorDTO newAuthor);
}
