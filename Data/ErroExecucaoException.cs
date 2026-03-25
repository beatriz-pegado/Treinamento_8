using System.Data.SqlClient; // Recomendado para .NET 8
using System;
using System.Collections.Generic;
using System.Text.Json; // Namespace oficial do .NET 8

public class ErroExecucaoException : Exception
{
    public class ErroItem
    {
        public string NomeInput { get; set; }
        public string Mensagem { get; set; }
    }
    // Usar uma classe tipada é muito melhor que dynamic no .NET 8
    public List<ErroItem> Erros { get; set; } = new List<ErroItem>();

    public ErroExecucaoException(SqlErrorCollection erros)
    {
        foreach (SqlError item in erros)
        {
            // 3609 = Erro de trigger/transação abortada (apenas ignoramos)
            if (item.Number == 3609) continue;

            if (item.Number == 50001)
            {
                try
                {
                    // Deserializa o JSON vindo da mensagem do SQL
                    var erroObjeto = JsonSerializer.Deserialize<ErroItem>(item.Message);

                    if (erroObjeto != null)
                        Erros.Add(erroObjeto);
                }
                catch (JsonException)
                {
                    // Caso o JSON no SQL esteja mal formatado, tratamos como erro comum
                    throw new Exception(item.Message);
                }
            }
            else
            {
                throw new Exception(item.Message);
            }
        }
    }
}