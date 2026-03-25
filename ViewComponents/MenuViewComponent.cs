using Treinamento8_0.Models;
using Microsoft.AspNetCore.Mvc;
using Treinamento8_0.Interfaces;

namespace Treinamento8_0.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IIdentificacao _identificacao;
        private List<Pagina> Consultar()
        {
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("@TipoConsulta", "C_Menu");
            parametros.Add("@IdOperador", _identificacao.IdOperador);

            Dao dao = new Dao();
            return dao.ExecutarProcedureList<Pagina>("stp_ABP_MontaMenu", parametros);
        }

        public MenuViewComponent(IHttpContextAccessor accessor, IIdentificacao identificacao)
        {
            _accessor = accessor;
            _identificacao = identificacao;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Menu menu = new Menu();
            menu.paginas = Consultar();
            return View(menu);
        }
    }
}
