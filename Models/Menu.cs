using Treinamento8_0.Interfaces;

namespace Treinamento8_0.Models
{
    public class Pagina
    {
        public string CodigoSistema { get; set; }
        public string CaptionSistema { get; set; }
        public string CodigoModulo { get; set; }
        public string CaptionModulo { get; set; }
        public string CodigoPagina { get; set; }
        public string CaptionPagina { get; set; }
    }
    public class Menu
    {
        public List<Pagina> paginas; 
    }
}
