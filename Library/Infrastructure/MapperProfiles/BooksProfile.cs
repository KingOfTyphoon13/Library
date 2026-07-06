using AutoMapper;
using Library.Domain.DTOs.Books;
using Library.ViewModels.Books;

namespace Library.Infrastructure.MapperProfiles;

public class BooksProfile : Profile
{
    public BooksProfile()
    {
        CreateMap<BookDTO, BookSummaryViewModel>()
            .ForMember(d => d.Authors, o => o.Ignore());

        CreateMap<BookWithAuthorsDTO, BookSummaryViewModel>()
            .IncludeBase<BookDTO, BookSummaryViewModel>()
            .ForMember(d => d.Authors, o => o.MapFrom(s => s.Authors));

        CreateMap<BookReviewStatsDTO, BookReviewStatsItemViewModel>();

        CreateMap<BookCreateViewModel, CreateBookDTO>();
    }
}

