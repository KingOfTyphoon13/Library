using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Services.CacheService.Redis;

public class RedisCacheSettings
{
    public const string Section = "Redis";

    [Required]
    public string Host { get; set; } = default!;

    [Range(1, 65535)]
    public int Port { get; set; }

    public string Password { get; set; } = default!;

    public int AbsoluteExpirationMinutes { get; set; }
    public TimeSpan AbsoluteExpiration => TimeSpan.FromMinutes(AbsoluteExpirationMinutes);

    public ConfigurationOptions ToConfigurationOptions() => new()
    {
        EndPoints = { { Host, Port } },
        Password = Password,
        AbortOnConnectFail = false
    };
}
