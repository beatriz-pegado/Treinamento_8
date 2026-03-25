using System;
using System.Collections.Generic;
using Treinamento8_0.Interfaces;

public class Botao
{
    public string IdAcao { get; set; }
    public string Codigo { get; set; }
    public string Caption { get; set; }
}

public class BotaoDao
{
    // .NET CORE 8, devido às mudanças feitas no Identificacao.cs
    private Identificacao _identificacao;
   
    public List<Botao> GetAll()
    {
        //  .NET 4.8
        //  Identificacao identificacao = new Identificacao();

        // .NET CORE 8
        _identificacao = new Identificacao();
        Dictionary<string, object> parametros = new Dictionary<string, object>();

        parametros.Add("@TipoConsulta", "C_BOTAO");
        parametros.Add("@IdOperador", _identificacao.IdOperador);
        parametros.Add("@CodigoSistema", _identificacao.Sistema);
        parametros.Add("@CodigoModulo", _identificacao.Modulo);
        parametros.Add("@CodigoPagina", _identificacao.Pagina);

        return new Dao().ExecutarProcedureList<Botao>("stp_ABP_MontaMenu", parametros);
    }
}