using Library.Domain.DTOs.Authors;

namespace Library.Domain.DataAccess;

public interface IAuthorRepository
{
    Task<int> AddAsync(AuthorDTO dto);
    Task<AuthorDTO?> GetByIdAsync(int id);
    Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync();
    Task<bool> ExistsAsync(int id);
}
