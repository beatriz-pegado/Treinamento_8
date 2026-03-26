using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Treinamento8_0.TagHelpers
{
    // Substitui @PageHelper.MakeBotao(botoes, acao)
    [HtmlTargetElement("make-botao")]
    public class BotaoTagHelper : TagHelper
    {
        public List<Botao> Lista { get; set; }
        public string Acao { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var botao = Lista?.FirstOrDefault(x => x.Codigo == Acao);
            if (botao == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "button";
            output.Attributes.SetAttribute("class", "btn btn-outline-primary");
            output.Attributes.SetAttribute("type", "button");
            output.Attributes.SetAttribute("onclick", $"executarAcao('{botao.Codigo}')");
            output.Content.SetContent(botao.Caption);
        }
    }
}