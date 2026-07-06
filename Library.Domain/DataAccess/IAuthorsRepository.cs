using Library.Domain.DTOs.Authors;

namespace Library.Domain.DataAccess;

public interface IAuthorsRepository
{
    Task<List<AuthorDTO>> GetAuthors();

    Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync();

    Task<AuthorDTO?> GetByIdAsync(int id);

    Task<int> AddAsync(AuthorDTO dto);

    Task<bool> ExistsAsync(int id);
}
