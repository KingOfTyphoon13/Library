namespace Library.ViewModels.Common;

public class PaginationViewModel
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public string ActionName { get; set; } = "Index";
    public string ControllerName { get; set; } = "";
    public string ContainerId { get; set; } = "paged-content";
}