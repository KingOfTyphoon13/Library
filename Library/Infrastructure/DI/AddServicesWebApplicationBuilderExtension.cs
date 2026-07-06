using Library.DAL.DataAccess;
using Library.Domain.DataAccess;
using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.BooksService;
using Library.Domain.Services.ReviewService;

namespace Library.Infrastructure.DI;

public static class AddServicesWebApplicationBuilderExtension
{
    private const string _connectionStringSection = "DefaultConnection";

    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IUnitOfWork, UnitOfWork>(_ =>
        {
            var connectionString = configuration.GetConnectionString(_connectionStringSection);

            return new UnitOfWork(connectionString);
        });

        services.AddScoped<IAuthorsService, AuthorsService>();

        services.AddScoped<IReviewService, ReviewService>();

        services.AddScoped<IBooksService, BooksService>();

        return builder;
    }
}
