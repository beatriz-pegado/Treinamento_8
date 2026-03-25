using Treinamento8_0.Interfaces;

public class Identificacao : IIdentificacao
{
    private string _IdOperador;
    private string _Sistema;
    private string _Pagina;
    private string _Modulo;

    private readonly HttpContext _context = new HttpContextAccessor().HttpContext;

    public Identificacao()
    {

        _IdOperador = GetIdOperador();

        string caminhoPagina = _context.Request.Path;

        string[] vetorcaminho = new string[5];
        char[] splitter = { '/' };

        vetorcaminho = caminhoPagina.Split(splitter);

        int count = vetorcaminho.Length;

        if (count - 3 >= 0)
        {
            _Sistema = vetorcaminho[count - 3];
        }

        if (count - 2 >= 0)
        {
            _Modulo = vetorcaminho[count - 2];
        }

        if (count - 1 >= 0)
        {
            _Pagina = vetorcaminho[count - 1].Split('.')[0];
        }
    }

    private string GetIdOperador()
    {
        string val = null;

        if (_context.Session.GetString("IdOperador") != null)
        {
            val = _context.Session.GetString("IdOperador");
        }

        return val;
    }

    public string IdOperador
    {
        get { return _IdOperador; }
    }
    public string Sistema
    {
        get { return _Sistema; }
    }
    public string Modulo
    {
        get { return _Modulo; }
    }
    public string Pagina
    {
        get { return _Pagina; }
    }
}