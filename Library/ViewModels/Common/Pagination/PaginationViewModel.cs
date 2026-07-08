namespace Library.ViewModels.Common.Pagination;



public class PaginationViewModel
{
    public const string DefaultContainerId = "paged-content";

    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string ContainerId { get; set; } = DefaultContainerId;
}