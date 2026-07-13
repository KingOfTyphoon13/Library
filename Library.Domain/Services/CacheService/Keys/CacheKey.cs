namespace Library.Domain.Services.CacheService.Keys;

public sealed class CacheKey
{
    internal string Resource { get; }
    internal string Suffix { get; }
    internal bool IsVersioned { get; }

    private CacheKey(string resource, string suffix, bool isVersioned)
    {
        Resource = resource;
        Suffix = suffix;
        IsVersioned = isVersioned;
    }

    internal static CacheKey Versioned(string resource, string suffix) => new(resource, suffix, isVersioned: true);
    internal static CacheKey Plain(string resource, string suffix) => new(resource, suffix, isVersioned: false);

    public override string ToString() => $"{Resource}:{Suffix}";
}
