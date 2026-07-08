using Library.Domain.Common.Pagination;

namespace Library.Domain.DTOs;

public abstract class BaseDTO : IIdentifiable
{
    public int Id { get; set; }
}
