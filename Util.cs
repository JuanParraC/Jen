// Autor : Juan Parra
// 3Soft



namespace Jen
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using Json;
    using AppSettingReader = System.Configuration.AppSettingsReader;
    using CultureInfo = System.Globalization.CultureInfo;
    using Encoding = System.Text.Encoding;
    using FechaHora = System.DateTime;
    using Formatting = System.Xml.Formatting;
    using MD5CryptoServiceProvider = System.Security.Cryptography.MD5CryptoServiceProvider;
    using MemoryStream = System.IO.MemoryStream;
    using StringBuilder = System.Text.StringBuilder;
    using XmlSerializer = System.Xml.Serialization.XmlSerializer;
    using XmlTextWriter = System.Xml.XmlTextWriter;
    using Ev = System.Environment;
    using System.Security.Cryptography;
    using System.Web;
    using System.Collections.Specialized;

    /// <summary>
    /// Clase de proposito general para proporcionar funcionalidades de generación
    /// </summary>
    public static class Util
    {
        #region campos
        // contador para asignar identificadores por defecto
        private static int _contador;
        // configuraciones del proyecto
        private static AppSettingReader configuracion = new AppSettingReader();
        // instancia de la cultura por defecto para los proyectos
        private static CultureInfo _cultura = crearCultura();

        #endregion

        #region Propiedades
        /// <summary>
        /// cultura por defecto 
        /// </summary>
        public static CultureInfo Cultura
        {
            get
            {
                return _cultura;
            }
        }
        #endregion

        #region Metodos

        #region CalcularAhora
        /// <summary>
        /// genera la strFecha actual segun el formato especificado
        /// </summary>
        /// <mr name="parametros">formato de strFecha a generar</mr>
        /// <returns>retorna la strFecha actual en el formato especificado</returns>
        public static string Ahora(string fmt)
        {
            return FechaHora.Now.ToString(fmt, Util.Cultura);
        }
        public static string FechaHabil(string fmt, double dias = 0)
        {
            FechaHora fechaHab = FechaHora.Now;
            if (dias > 0)
            {
                fechaHab = fechaHab.AddDays(dias);
                switch (fechaHab.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        break;
                    case DayOfWeek.Tuesday:
                        break;
                    case DayOfWeek.Wednesday:
                        break;
                    case DayOfWeek.Thursday:
                        break;
                    case DayOfWeek.Friday:
                        break;
                    case DayOfWeek.Saturday:
                        fechaHab = fechaHab.AddDays(2);
                        break;
                    case DayOfWeek.Sunday:
                        fechaHab = fechaHab.AddDays(2);
                        break;
                }
                return fechaHab.ToString(fmt, Util.Cultura);
            }
           return Lenguaje.Null;
        }
        #endregion

        #region crearCultura
        /// <summary>
        /// <c>crearCultura : </c> crea la cultura
        /// </summary>
        /// <returns>retorna una nombreId</returns>
        private static CultureInfo crearCultura()
        {
            CultureInfo cultura = new CultureInfo(Lenguaje.EsCL);
            cultura.DateTimeFormat.DateSeparator = Lenguaje.Slash;
            cultura.NumberFormat.CurrencyDecimalSeparator = Lenguaje.Punto;
            cultura.NumberFormat.CurrencyGroupSeparator = Lenguaje.Coma;
            cultura.NumberFormat.NumberDecimalSeparator = Lenguaje.Punto;
            cultura.NumberFormat.NumberGroupSeparator = Lenguaje.Coma;
            cultura.NumberFormat.NegativeSign = "-";
            return cultura;

        }
        #endregion

        #region Id
        /// <summary>
        /// <c>Id() : </c> genera la identificación por defecto de los objetos
        /// </summary>
        /// <returns>retorna una nombreId</returns>
        public static string Id()
        {
            return (++_contador).ToString(Util.Cultura);
        }
        public static int Contador()
        {
            return (++_contador);
        }
        #endregion

        #region MD5
        /// <summary>
        /// <c>MD5 : </c> Genera la representación MD5 de una patron
        /// </summary>
        /// <mr name="patron">patron a generar en clave</mr>
        /// <returns>retorna la representacion clave de la patron</returns>
        public static string MD5(string patron)
        {
            System.Security.Cryptography.MD5 md5 = MD5CryptoServiceProvider.Create();
            byte[] bytePatron = Encoding.Default.GetBytes(patron);
            byte[] bs = md5.ComputeHash(bytePatron);
            StringBuilder s = new StringBuilder(bs.Length * 2);
            foreach (byte b in bs)
            {
                s.Append(b.ToString(Lenguaje.x2, Util.Cultura));
            }
            return s.ToString();
        }
        #endregion

        #region ProcErrorSeguridad
        public static void ProcErrorSeguridad(HttpResponse response, string msj = "Error de seguridad, invocación no permitida")
        {
            string sDepurar = Util.Configuracion("Depurar");
            bool depurar = false;
            if (!string.IsNullOrEmpty(sDepurar))
            {
                bool.TryParse(sDepurar, out depurar);
            }
            if (!depurar)
            {
                msj = "Error de seguridad, invocación no permitida";
            }
            response.StatusCode = 401;
            response.StatusDescription = msj;
            response.End();
        }
        #endregion
        public static HttpRequest CrearRequest(string urlParam, HttpRequest reqOri)
        {
            HttpRequest request = new HttpRequest(string.Empty, Util.Configuracion(Lenguaje.URL), urlParam);
            request.Browser = reqOri.Browser;
            return request;
        }
        public static bool ValidarRequest(HttpRequest request, HttpResponse response)
        {
            string[] urlComp = { "plataforma", "cache", "dsId" };
            foreach (string comp in urlComp)
            {
                if (string.IsNullOrEmpty(request[comp]))
                {
                    string sDepurar = "Error: request[" + comp + "] nulo";
                    Util.ProcErrorSeguridad(response, sDepurar);
                    return false;
                }
            }
            return true;
        }
        public static StringBuilder CompletarRequest(string urlParam, NameValueCollection nvc, HttpRequest reqOri, HttpRequest request)
        {
            StringBuilder sbParams = new StringBuilder(urlParam);

            foreach (string k in nvc)
            {
                if (!string.IsNullOrEmpty(k))
                {
                    if (string.IsNullOrEmpty(request[k])) // para no repetir
                    {
                        if (!string.IsNullOrEmpty(reqOri[k]))
                        {
                            sbParams.Append("&");
                            sbParams.Append(k);
                            sbParams.Append("=");
                            sbParams.Append(HttpUtility.UrlEncode(reqOri[k]));
                        }
                    }
                }
            }
            return sbParams;
        }
        public static string JavaScriptStringLiteral(string str, string dcma = "\"")
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(dcma);
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append(dcma);

            return sb.ToString();
        }



        #region Configuracion
        /// <summary>
        /// Permite leer la configuracion del archivo .config
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static string Configuracion(string par)
        {
            return (string)configuracion.GetValue(par, typeof(string));
        }
        #endregion

        #region SerializaXML
        /// <summary>
        /// <c>ToXMLArray: </c> serializa a xml en formato de arreglo de byte
        /// </summary>
        /// <returns>retorna el xml objeto en un arreglo </returns>
        internal static byte[] toXMLArray(object o)
        {
            MemoryStream ms = new MemoryStream();

            // crea un xtw de texto XML
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8);
            //EscritorXml xtw = new EscritorXml(ms);

            // especifica la indentación del documento
            xtw.Formatting = Formatting.None;

            // crea un XmlSerializer xml
            XmlSerializer xs = new XmlSerializer(o.GetType());

            // serializar en memoria
            xs.Serialize(xtw, o);

            // descarga la serializarcion en el stream interno
            xtw.Flush();

            // direcciona el stream interno 
            ms = (MemoryStream)xtw.BaseStream;

            return ms.ToArray();
        }
        #endregion

        #region CrearArchivo
        public static void CrearArchivo(string archivo, string contenido, bool full = false)
        {
            string ruta = string.Concat(Configuracion("Directorio"), archivo);
            if (full)
            {
                ruta = archivo;
            }
            TextWriter tw = new StreamWriter(ruta);
            tw.Write(contenido);
            tw.Close();
        }
        public static void CrearArchivo(string archivo, Recipiente<Dato> rc, bool full = false)
        {
            string ruta = string.Concat(Configuracion("Directorio"), archivo);
            if (full)
            {
                ruta = archivo;
            }
            using (StreamWriter outputFile = new StreamWriter(ruta, false, Encoding.UTF8))
            {
                foreach (Dato d in rc)
                {
                    outputFile.WriteLine(d.Valor);
                }
            }
        }
        #endregion
        #region LeerArchivo
        public static string LeerArchivo(string archivo, bool full = false)
        {
            string ruta = string.Concat(Configuracion("Directorio"), archivo);
            if (full)
            {
                ruta = archivo;
            }
            using (StreamReader lector = new StreamReader(ruta))
            {
                return lector.ReadToEnd();
            }
        }
        public static bool EsBinario(string path, long length)
        {
            //long length = getSize(path);
            if (length == 0)
            {
                return false;
            }

            using (StreamReader stream = new StreamReader(path))
            {
                int ch;
                while ((ch = stream.Read()) != -1)
                {
                    if (isControlChar(ch))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool isControlChar(int ch)
        {
            return (ch > Chars.NUL && ch < Chars.BS)
                || (ch > Chars.CR && ch < Chars.SUB);
        }

        public static class Chars
        {
            public static char NUL = (char)0; // Null char
            public static char BS = (char)8; // Back Space
            public static char CR = (char)13; // Carriage Return
            public static char SUB = (char)26; // Substitute
        }
        public static string GenerarToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        #endregion

        #endregion

    }
}