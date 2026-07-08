using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Authors;

namespace Library.Domain.Services.AuthorsService;

public interface IAuthorsService
{
    Task<PagedResult<AuthorDTO>> GetAuthorsAsync(PagedRequest request);

    Task<KeysetResult<AuthorDTO>> GetAuthorsAsync(KeysetRequest request);

    Task<PagedResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(PagedRequest request);

    Task<KeysetResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(KeysetRequest request);

    Task AddAuthorAsync(AuthorDTO newAuthor);
}
