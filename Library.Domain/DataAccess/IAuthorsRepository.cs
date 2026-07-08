using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Authors;

namespace Library.Domain.DataAccess;

public interface IAuthorsRepository : IRepository
{
    Task<List<AuthorDTO>> GetAuthors(PagedRequest request);

    Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync(PagedRequest request);

    Task<List<AuthorDTO>> GetAuthors(KeysetRequest request);

    Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync(KeysetRequest request);

    Task<int> AddAsync(AuthorDTO dto);

    Task<bool> ExistsAsync(int id);
}
