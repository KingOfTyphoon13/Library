using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Authors;

namespace Library.Domain.Services.AuthorsService;

public interface IAuthorsService
{
    Task<PagedResult<AuthorDTO>> GetAuthorsAsync(PagedRequest request);

    Task<PagedResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(PagedRequest request);

    Task<int> GetAuthorsNumberAsync();

    Task<int> AddAuthorAsync(AuthorDTO newAuthor);
}
