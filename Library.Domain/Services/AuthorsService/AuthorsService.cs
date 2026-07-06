using Library.Domain.DTOs.Authors;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.AuthorsService;

public class AuthorsService : BaseService, IAuthorsService
{
    private static List<AuthorWithBooksCountDTO> _authorWithBooksCounts =
    [
        new() { Id = 1, Name = "Alex", Surname = "Michaelides", Count = 1 },
        new() { Id = 2, Name = "Robert", Surname = "Martin", Count = 3 },
        new() { Id = 3, Name = "Terry", Surname = "Pratchett", Count = 5 },
        new() { Id = 4, Name = "Neil", Surname = "Gaiman", Count = 4 },
        new() { Id = 5, Name = "Frank", Surname = "Herbert", Count = 2 },
        new() { Id = 6, Name = "Andy", Surname = "Weir", Count = 1 },
    ];

    private static List<AuthorWithBooksCountDTO> GetDummyAuthors() => _authorWithBooksCounts;

    public AuthorsService(ILogger<AuthorsService> logger) : base(logger)
    {
    }

    public List<AuthorWithBooksCountDTO> GetAuthorWithBooksCounts()
    {
        return GetDummyAuthors();
    }

    public void AddAuthor(AuthorDTO newAuthor)
    {
        if (string.IsNullOrWhiteSpace(newAuthor.Name) && string.IsNullOrWhiteSpace(newAuthor.Surname))
            throw new ArgumentException("Either Name or Surname must be provided.", nameof(newAuthor));

        //_authorWithBooksCounts.Add
    }
}
