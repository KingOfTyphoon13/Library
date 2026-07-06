using Library.Domain.Services.AuthorsService;

namespace Library.Infrastructure.DI;

public static class AddServicesWebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<IAuthorsService, AuthorsService>();


        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());


        return builder;
    }
}
