using Library.DAL.DataAccess;
using Library.Domain.DataAccess;
using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.BooksService;
using Library.Domain.Services.CacheService;
using Library.Domain.Services.CacheService.Redis;
using Library.Domain.Services.ReviewService;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Library.Infrastructure.DI;

public static class AddServicesWebApplicationBuilderExtension
{
    private const string _connectionStringSection = "DBConnection";

    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddOptions(configuration);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisCacheSettings>>().Value;
            return ConnectionMultiplexer.Connect(settings.ToConfigurationOptions());
        });

        services.AddSingleton<ICacheService, RedisCacheService>();


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

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RedisCacheSettings>()
            .Bind(configuration.GetSection(RedisCacheSettings.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
