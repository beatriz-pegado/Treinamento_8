using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

public static class Util
{
    public static string ChecaNulo(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        else
        {
            return str.Trim();
        }
    }

    public static string FormatarData(string str, int style)
    {
        if (!string.IsNullOrEmpty(str))
        {
            if (DateTime.TryParse(str, out DateTime dt))
            {
                return FormatarData(dt, style);
            }
        }

        return string.Empty;
    }

    public static string FormatarData(DateTime dt, int style)
    {
        switch (style)
        {
            case 120:
                return dt.ToString("yyyy-MM-dd HH:mm:ss");

            case 126:
                return dt.ToString("yyyy-MM-ddTHH:mm:ss");

            case 103:
                return dt.ToString("dd/MM/yyyy");

            case 23:
                return dt.ToString("yyyy-MM-dd");

            case 108:
                return dt.ToString("HH:mm:ss");

            case 200:
                return dt.ToString("dd/MM/yyyy HH:mm:ss");

            case 201:
                return dt.ToString("dd/MM/yyyy") + " às " + dt.ToString("HH:mm:ss");

            default:
                break;
        }

        return string.Empty;
    }

    public static string FileToBase64(IFormFile file)
    {
        string base64 = null;

        if (file.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                base64 = Convert.ToBase64String(fileBytes);
            }
        }

        return base64;
    }

    public static void ExceptionHandler(Action action, ModelStateDictionary model)
    {
        try
        {
            action();
        }
        // Tratamento de erro criado na classe ErroExecucaoException
        catch (ErroExecucaoException ex)
        {
            // Adiciona warning
            model.AddModelError("alert-warning", "Por favor, verifique o formulário.");

            // Adiciona erros por input
            foreach (dynamic item in ex.Erros)
            {
                model.AddModelError(item.NomeInput, item.Mensagem);
            }
        }
        catch (Exception ex)
        {
            model.AddModelError("alert-danger", ex.Message);
        }
    }


    public static string IsInvalid(string key, ModelStateDictionary model)
    {
        // Verifica se o dicionário não é nulo e se o campo específico possui erros
        if (model != null && model.GetFieldValidationState(key) == ModelValidationState.Invalid)
        {
            return "is-invalid";
        }

        return string.Empty;
    }

// Converte decimal para string e formata para duas casas decimais
public static string ConverterMoeda(decimal valor, string culture)
    {
        return valor.ToString("N2", new CultureInfo(culture));
    }

    // Converte string para decimal
    public static decimal ConverterMoeda(string valor)
    {
        return (ChecaNulo(valor) != null) ? Convert.ToDecimal(valor) : 0;

    }

    // Formata data e hora no formato dataThora
    public static string FormatarDataHora(string data, string hora, int style)
    {
        if (data != null && hora != null)
            return FormatarData(data, style) + "T" + hora;

        return null;
    }

    public static string FormatarCPF(string cpf)
    {
        if (cpf != null && cpf.Length > 11)
        {
            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");
        }

        return cpf;
    }
}

