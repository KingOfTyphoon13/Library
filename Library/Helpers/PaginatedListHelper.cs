using Library.ViewModels.Common.Pagination;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Encodings.Web;

namespace Library.Helpers;

public static class PaginatedListHelper
{
    public static async Task<IHtmlContent> RenderPaginatedListAsync<T>(
    this IHtmlHelper html,
    PaginatedViewModel<T> model,
    Func<T, HelperResult> itemTemplate)
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"element-grid\">");
        foreach (var item in model.Items)
        {
            sb.Append("<div class=\"element-card\">");

            using (var itemWriter = new StringWriter())
            {
                itemTemplate(item).WriteTo(itemWriter, HtmlEncoder.Default);
                sb.Append(itemWriter.ToString());
            }

            sb.Append("</div>");
        }
        sb.Append("</div>");

        var pagination = await html.PartialAsync("_Pagination", model.Pagination);
        using var writer = new StringWriter();
        pagination.WriteTo(writer, HtmlEncoder.Default);
        sb.Append(writer.ToString());

        return new HtmlString(sb.ToString());
    }
}
