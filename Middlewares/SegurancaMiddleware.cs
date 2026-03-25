namespace Treinamento8_0.Middlewares
{
    public class SegurancaMiddleware
    {
        private readonly RequestDelegate _next;

        public SegurancaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Definimos quais caminhos precisam de proteção
            // Aqui protegemos a "AreaRestrita", mas ignoramos a página de Login para evitar loop
            var path = context.Request.Path.Value?.ToLower();

            if (path != null && path.Contains("/arearestrita") && !path.Contains("/controleacesso"))
            {
                // 2. Verificação da Sessão (Equivalente ao seu if Session["idSessao"] == null)
                if (string.IsNullOrEmpty(context.Session.GetString("idSessao")))
                {
                    // 3. Redirecionamento de Servidor (Substitui o script JS e o Response.End)
                    context.Response.Redirect("/ControleAcesso/Index");
                    return; // Encerra a requisição aqui, sem processar a página
                }
            }

            // Se estiver logado ou for página pública, segue o fluxo
            await _next(context);
        
        }
    }
}
