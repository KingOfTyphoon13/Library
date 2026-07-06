using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.AuthorsService;

public class AuthorsService : BaseService, IAuthorsService
{
    private readonly IAuthorsRepository _authorRepository;

    public AuthorsService(IUnitOfWork unitOfWork, ILogger<AuthorsService> logger) : base(unitOfWork, logger)
    {
        _authorRepository = _unitOfWork.Authors;
    }

    public async Task<List<AuthorDTO>> GetAuthorsAsync()
    {
        return await _authorRepository.GetAuthors();
    }

    public async Task<List<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync()
    {
        return await _authorRepository.GetAllWithBookCountAsync();
    }

    public async Task AddAuthorAsync(AuthorDTO newAuthor)
    {
        if (string.IsNullOrWhiteSpace(newAuthor.Name) && string.IsNullOrWhiteSpace(newAuthor.Surname))
            throw new ArgumentException("Either Name or Surname must be provided.", nameof(newAuthor));

        await _authorRepository.AddAsync(newAuthor);
    }
}
