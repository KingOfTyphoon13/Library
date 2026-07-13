using Library.Domain.DataAccess;
using Library.Domain.Services.CacheService;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork _unitOfWork;

    protected readonly ICacheService _cacheService;

    protected readonly ILogger<BaseService> _logger;

    protected BaseService(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<BaseService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
