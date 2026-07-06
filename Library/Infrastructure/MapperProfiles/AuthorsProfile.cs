using AutoMapper;
using Library.Domain.DTOs.Authors;
using Library.ViewModels.Authors;

namespace Library.Infrastructure.MapperProfiles;

public class AuthorsProfile : Profile
{
    public AuthorsProfile()
    {
        CreateMap<AuthorDTO, AuthorViewModel>().ReverseMap();

        CreateMap<AuthorWithBooksCountDTO, AuthorListItemViewModel>()
            .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Count));

        CreateMap<AuthorSlotViewModel, AuthorDTO>()
            .ConvertUsing((src, _, ctx) =>
                src.ExistingAuthorId.HasValue
                    ? new AuthorDTO { Id = src.ExistingAuthorId.Value }
                    : ctx.Mapper.Map<AuthorDTO>(
                        src.NewAuthor ?? throw new InvalidOperationException(
                            "AuthorSlot has neither ExistingAuthorId nor NewAuthor.")));
    }
}
