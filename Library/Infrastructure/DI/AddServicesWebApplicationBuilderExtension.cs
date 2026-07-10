using Library.DAL.DataAccess;
using Library.Domain.DataAccess;
using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.BooksService;
using Library.Domain.Services.CacheService;
using Library.Domain.Services.CacheService.Redis;
using Library.Domain.Services.ReviewService;
using StackExchange.Redis;

namespace Library.Infrastructure.DI;

public static class AddServicesWebApplicationBuilderExtension
{
    private const string _connectionStringSection = "DBConnection";
    private const string _connectionStringRedisSection = "RedisConnection";

    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddOptions(configuration);

        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration.GetConnectionString(_connectionStringRedisSection)));

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
            .ValidateOnStart();

        return services;
    }
}
