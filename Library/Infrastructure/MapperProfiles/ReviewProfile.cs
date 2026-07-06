using AutoMapper;
using Library.Domain.DTOs.Reviews;
using Library.ViewModels.Reviews;

namespace Library.Infrastructure.MapperProfiles;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<ReviewCreateViewModel, ReviewDTO>();

        CreateMap<ReviewWithBookInfoDTO, ReviewListItemViewModel>()
            .ForMember(d => d.BookSummary, o => o.MapFrom(s => s.BookInfo));
    }
}
