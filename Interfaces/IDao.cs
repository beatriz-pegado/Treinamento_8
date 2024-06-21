namespace Treinamento8_0.Interfaces
{
    public interface IDao
    {
        void ExecutarProcedure(string procedure, Dictionary<string, object> parametros);
        T ExecutarProcedure<T>(string procedure, Dictionary<string, object> parametros);
        List<T> ExecutarProcedureList<T>(string procedure, Dictionary<string, object> parametros);
        void ExecutarAcao(string acao, Dictionary<string, object> parametros);
        List<T> ExecutarAcaoList<T>(string acao, Dictionary<string, object> parametros);
    }
}
