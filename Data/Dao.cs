using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Treinamento8_0.Interfaces;

public class Dao : IDao
{
    private class Autorizacao
    {
        public string NomeProcedure { get; set; }
    }

    private Identificacao _identificacao;
    private readonly string stringConexao = "Server=.\\SQLEXPRESS; Database=dbTreinamento; Trusted_Connection = True;";
    //private readonly string stringConexao = "Server=WINSERVER2022; Database= dbTreinamento; Trusted_Connection=True;";

    public void ExecutarProcedure(string procedure, Dictionary<string, object> parametros)
    {
        // Tratamento de erros vindos do Sql
        SqlErrorCollection erros = null;

        SqlConnection conn = new SqlConnection(stringConexao);

        conn.Open();

        SqlCommand cmmd = NovoCmmd(procedure, conn);

        AdicionarParametros(cmmd, parametros);

        // Seta a configuração para disparar um evento quando acontecer um erro de baixa relevância na procedure.
        conn.FireInfoMessageEventOnUserErrors = true;

        // Função lambda para tratar cada erro disparado pela procedure.
        conn.InfoMessage += new SqlInfoMessageEventHandler((object sender, SqlInfoMessageEventArgs e) =>
        {
            erros = e.Errors;
        });

        // Executar sem devolver nenhum valor
        cmmd.ExecuteNonQuery();

        // Verifica se aconteceu algum erro
        if (erros != null)
        {
            throw new ErroExecucaoException(erros);
        }

        // Indicação para que o garbage collector limpe a memória
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

    public DataTable ExecutarProcedureDt(string procedure, Dictionary<string, object> parametros)
    {
        DataTable dt = new DataTable();

        SqlConnection conn = new SqlConnection(stringConexao);

        conn.Open();

        SqlCommand cmmd = NovoCmmd(procedure, conn);

        AdicionarParametros(cmmd, parametros);

        SqlDataReader dr = cmmd.ExecuteReader();

        dt.BeginLoadData();
        dt.Load(dr);
        dt.EndLoadData();

        dr.Close();
        dr.Dispose();

        cmmd.Dispose();

        conn.Close();
        conn.Dispose();

        return dt;
    }

    private string GetNomeProcedure(string acao)
    {
        _identificacao = new Identificacao();
        Dictionary<string, object> parametros = new Dictionary<string, object>();
        parametros.Add("@TipoConsulta", "C_ACAO");
        parametros.Add("@IdOperador", _identificacao.IdOperador);
        parametros.Add("@CodigoSistema", _identificacao.Sistema);
        parametros.Add("@CodigoModulo", _identificacao.Modulo);
        parametros.Add("@CodigoPagina", _identificacao.Pagina);
        parametros.Add("@CodigoAcao", acao);

        List<Autorizacao> autorizacoes = ExecutarProcedureList<Autorizacao>("stp_ABP_MontaMenu", parametros);

        if (autorizacoes == null)
        {
            throw new InvalidOperationException("Operador não autorizado para executar essa ação.");
        }

        return autorizacoes.FirstOrDefault().NomeProcedure;
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