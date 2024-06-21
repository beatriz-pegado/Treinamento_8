using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Treinamento8_0.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _accessor;

        public IndexModel(IHttpContextAccessor acessor)
        {
            _accessor = acessor;
        }

        public void OnGet()
        {
            //_accessor.HttpContext.Session.SetString("IdOperador", "1");
            string idOperador = _accessor.HttpContext.Session.GetString("IdOperador");

            if (string.IsNullOrWhiteSpace(idOperador))
            {
                _accessor.HttpContext.Response.Redirect("/ControleAcesso");
            }
            else
            {
                _accessor.HttpContext.Response.Redirect("/AreaRestrita");
            }
        }
    }
}
