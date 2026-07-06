using AutoMapper;
using Library.Domain.DTOs.Authors;
using Library.ViewModels.Authors;

namespace Library.Infrastructure.MapperProfiles;

public class AuthorsProfile : Profile
{
    public AuthorsProfile()
    {
        CreateMap<AuthorDTO, AuthorViewModel>();

        CreateMap<AuthorWithBooksCountDTO, AuthorListItemViewModel>()
            .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Count));
    }
}
