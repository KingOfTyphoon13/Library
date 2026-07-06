using Library.Domain.DataAccess;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork _unitOfWork;

    protected readonly ILogger<BaseService> _logger;

    protected BaseService(IUnitOfWork unitOfWork, ILogger<BaseService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
