namespace Library.Infrastructure.DI;

public static class AddServicesWebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        return builder;
    }
}
