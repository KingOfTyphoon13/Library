using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.BooksService;
using Library.Domain.Services.ReviewService;

namespace Library.Infrastructure.DI;

public static class AddServicesWebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<IAuthorsService, AuthorsService>();

        services.AddScoped<IReviewService, ReviewService>();

        services.AddScoped<IBooksService, BooksService>();


        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());


        return builder;
    }
}
