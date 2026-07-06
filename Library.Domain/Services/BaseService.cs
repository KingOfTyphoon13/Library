using Microsoft.Extensions.Logging;

namespace Library.Domain.Services;

public abstract class BaseService
{
    protected readonly ILogger<BaseService> _logger;

    protected BaseService(ILogger<BaseService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
