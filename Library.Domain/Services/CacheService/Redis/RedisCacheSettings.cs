namespace Library.Domain.Services.CacheService.Redis;

public class RedisCacheSettings
{
    public const string Section = "Redis";

    public int AbsoluteExpirationMinutes { get; set; }

    public TimeSpan AbsoluteExpiration => TimeSpan.FromMinutes(AbsoluteExpirationMinutes);
}
