
namespace Jen.Extensiones
{
    using System.Text;
    using System.Text.RegularExpressions;

    public static partial class Extensiones
    {
        public static string Camel(this string s, bool full = false)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            string[] tokens = s.Split('_');
            string token;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                token = tokens[i];
                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }
                builder.Append(token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower());
            }

            string ret = builder.ToString();
            if (!full)
            {
                return ret.Substring(0, 1).ToLower() + ret.Substring(1);
            }
            return ret.Substring(0, 1).ToUpper() + ret.Substring(1);
        }

        public static string[] ArrIDs(this string s, char sep = '.')
        {
            return s.Split(new char[] { sep }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        // decora la entrada para que sea expresada como criterio
        public static string ValorQuery(this string s)
        {
            if (s.IndexOf('\'') > -1)
            {
                return s;
            }
            return string.Concat("'", s, "'");
        }
        public static string labelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            StringBuilder undercase = new StringBuilder();
            string[] tokens = Regex.Split(s, "([A-Z]{1}[a-z]+)");

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == string.Empty)
                {
                    continue;
                }

                undercase.Append(tokens[i].ToLower());
                if (i < tokens.Length - 2)
                {
                    undercase.Append('_');
                }
            }
            string ret = undercase.ToString();
            ret = ret.Replace("_", " ");
            ret = ret.Substring(0, 1).ToUpper() + ret.Substring(1);

            return ret;
        }
        public static string[] DotSep(this string s)
        {
            return s.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        public static string EliminaAcentos(this string s)
        {
            string ret = s;
            ret = ret.Replace("á", "a");
            ret = ret.Replace("Á", "A");
            ret = ret.Replace("é", "e");
            ret = ret.Replace("É", "E");
            ret = ret.Replace("Í", "I");
            ret = ret.Replace("í", "i");
            ret = ret.Replace("ó", "o");
            ret = ret.Replace("Ó", "O");
            ret = ret.Replace("ú", "u");
            ret = ret.Replace("Ú", "U");
            ret = ret.Replace("ñ", "n");
            ret = ret.Replace("Ñ", "N");
            ret = ret.Replace("º", "");
            return ret;
        }

        public static string CorreccionXml(this string s)
        {
            string ret = s;
            ret = ret.Replace("á", "&aacute;");
            ret = ret.Replace("Á", "&Aacute;");
            ret = ret.Replace("é", "&eacute;");
            ret = ret.Replace("É", "&Eacute;");
            ret = ret.Replace("Í", "&Iacute;");
            ret = ret.Replace("í", "&iacute;");
            ret = ret.Replace("ó", "&oacute;");
            ret = ret.Replace("Ó", "&Oacute;");
            ret = ret.Replace("ú", "&uacute;");
            ret = ret.Replace("Ú", "&Uacute;");
            ret = ret.Replace("ñ", "&ntilde;");
            ret = ret.Replace("Ñ", "&Ntilde;");
            ret = ret.Replace("º", "&ordm;");
            return ret;
        }

        public static string EnlazarBlancos(this string s)
        {
            return  Regex.Replace(s, @"\s+", "_");
        }
        public static string LimpiarBlancos(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            string t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + string.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        public static string qpEncMsj(this string s, params string[] vals)
        {
            
            if (vals != null)
            {
                StringBuilder sb = new StringBuilder(vals.Length * 8);
                string sep = "=";
                bool flag = true;
                foreach (string val in vals)
                {
                    if (flag)
                    {
                        sep = "=";

                    }
                    else
                    {
                        sep = "&";
                    }
                    flag = !flag;
                    sb.Append(val);
                    sb.Append(sep);
                }
                return string.Concat(sb.Length.ToString(), sb.ToString());
            }
            return string.Empty;
        }

    }

}
