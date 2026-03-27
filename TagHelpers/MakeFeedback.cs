using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

/* 
 * OBS: Essa versão não é a utilizada nos sistemas da LOG e foi feita apenas como teste utilizando TagHelpers. 
 * A versão oficial dos helpers fica em \Pages\Shared\Helpers e não é feita utilizando TagHelpers, mas PartialViews.
 */
namespace Treinamento8_0.TagHelpers
{
    // Substitui @PageHelper.MakeFeedback(key)
    [HtmlTargetElement("make-feedback")]
    public class FeedbackTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string ForKey { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var modelState = ViewContext.ModelState;
            var builder = new HtmlContentBuilder();
            if (modelState.TryGetValue(ForKey, out var entry))
            {
                foreach (var error in entry.Errors)
                {
                    builder.AppendHtmlLine($@"<div class=""invalid-feedback"" style=""display:block"">{error.ErrorMessage}</div>");
                }
            }
            output.Content.SetHtmlContent(builder);
        }
    }
}
