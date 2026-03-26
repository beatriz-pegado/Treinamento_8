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
            var path = context.Request.Path;

            if (path.StartsWithSegments("/AreaRestrita") && !path.StartsWithSegments("/ControleAcesso"))
            {
                if (string.IsNullOrEmpty(context.Session.GetString("idSessao")))
                {
                    context.Response.Redirect("/ControleAcesso/Index");
                    return; // Interrompe a execução aqui
                }
            }

            await _next(context); // Segue para o próximo middleware (ou para a página)
        }
    }
}
