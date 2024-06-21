using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Reflection;
using Treinamento8_0.Interfaces;

public class Dao : IDao
{
    private class Autorizacao
    {
        public string NomeProcedure { get; }
    }

    private readonly IIdentificacao _identificacao;
    private readonly string stringConexao = "Server=WINSERVER2022; Database= dbTreinamento; Trusted_Connection=True;";

    public void ExecutarProcedure(string procedure, Dictionary<string, object> parametros)
    {

        SqlConnection conn = new SqlConnection(stringConexao);

        conn.Open();

        SqlCommand cmmd = NovoCmmd(procedure, conn);

        AdicionarParametros(cmmd, parametros);

        cmmd.ExecuteNonQuery();

        cmmd.Dispose();

        conn.Close();
        conn.Dispose();
    }

    public T ExecutarProcedure<T>(string procedure, Dictionary<string, object> parametros)
    {
        List<T> list = ExecutarProcedureList<T>(procedure, parametros);

        if (list != null && list.Count > 0)
        {
            return list.FirstOrDefault();
        }
        else
        {
            return default(T);
        }
    }

    public List<T> ExecutarProcedureList<T>(string procedure, Dictionary<string, object> parametros)
    {
        List<T> list = null;

        SqlConnection conn = new SqlConnection(stringConexao);

        conn.Open();

        SqlCommand cmmd = NovoCmmd(procedure, conn);

        AdicionarParametros(cmmd, parametros);

        SqlDataReader dr = cmmd.ExecuteReader();

        list = CriaLista<T>(dr);

        cmmd.Dispose();

        conn.Close();
        conn.Dispose();

        return list;
    }

    public void ExecutarAcao(string acao, Dictionary<string, object> parametros)
    {
        string procedure = GetNomeProcedure(acao);
        ExecutarProcedure(procedure, parametros);
    }

    public List<T> ExecutarAcaoList<T>(string acao, Dictionary<string, object> parametros)
    {
        string procedure = GetNomeProcedure(acao);
        return ExecutarProcedureList<T>(procedure, parametros);
    }

    private string GetNomeProcedure(string acao)
    {
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("@acao", "C_Autorizacao");
        parametros.Add("@idOperador", _identificacao.IdOperador);
        parametros.Add("@CodigoSistema", _identificacao.Sistema);
        parametros.Add("@CodigoModulo", _identificacao.Modulo);
        parametros.Add("@CodigoPagina", _identificacao.Pagina);
        parametros.Add("@CodigoAcao", acao);

        Autorizacao autorizacao = ExecutarProcedureList<Autorizacao>("stp_Sys_MontaMenu", parametros).FirstOrDefault();

        return autorizacao.NomeProcedure;
    }

    private void AdicionarParametros(SqlCommand cmmd, Dictionary<string, object> parametros)
    {
        if (parametros != null)
        {
            foreach (var item in parametros)
            {
                cmmd.Parameters.AddWithValue(item.Key, item.Value);
            }
        }
    }

    private SqlCommand NovoCmmd(string procedure, SqlConnection conn)
    {
        return new SqlCommand(procedure, conn)
        {
            CommandType = System.Data.CommandType.StoredProcedure,
            CommandTimeout = 60
        };
    }

    private int GetColumnOrdinal(SqlDataReader dr, string columnName)
    {
        int ordinal = -1;

        for (int i = 0; i < dr.FieldCount; i++)
        {
            if (string.Equals(dr.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
            {
                ordinal = i;
                break;
            }
        }

        return ordinal;
    }

    private List<T> CriaLista<T>(SqlDataReader dr)
    {
        List<T> list = null;

        if (dr.HasRows)
        {
            list = new List<T>();
            while (dr.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    string nomecoluna;

                    if (property.GetCustomAttribute<ColumnAttribute>() != null)
                    {
                        nomecoluna = property.GetCustomAttribute<ColumnAttribute>().Name;
                    }
                    else
                    {
                        nomecoluna = property.Name;
                    }

                    int i = GetColumnOrdinal(dr, nomecoluna);

                    // se não achar a coluna no datareader, continua o laço
                    if (i < 0) continue;

                    // se for DBNull, continua o laço
                    if (dr[nomecoluna] == DBNull.Value) continue;

                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(item, Enum.Parse(property.PropertyType, dr[nomecoluna].ToString()));
                    }
                    else
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, Convert.ChangeType(dr[nomecoluna], convertTo), null);
                    }
                }
                list.Add(item);
            }
        }
        return list;
    }
}