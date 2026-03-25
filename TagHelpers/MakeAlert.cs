using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Treinamento8_0.TagHelpers
{
    // Substitui @PageHelper.MakeAlert()
    [HtmlTargetElement("make-alert")]
    public class AlertTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // Remove a tag <make-alert> do HTML final
            var modelState = ViewContext.ModelState;
            var builder = new HtmlContentBuilder();
            foreach (var item in modelState.Where(x => x.Key.StartsWith("alert-")))
            {
                foreach (var error in item.Value.Errors)
                {
                    builder.AppendHtmlLine($@"<div class=""alert {item.Key}"" role=""button"">{error.ErrorMessage}</div>");
                }
            }
            output.Content.SetHtmlContent(builder);
        }
    }
}
