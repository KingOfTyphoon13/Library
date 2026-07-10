using Serilog;

namespace Library.Infrastructure.DI;

public static class AddConfigurationsWebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        });



        return builder;
    }
}
