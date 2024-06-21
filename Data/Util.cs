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
}