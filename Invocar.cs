namespace Jen
{
    using System.IO;
    using System.Text;
    using System.Web;

    public static class Invocar
    {
        public static HttpCookieCollection Cookies { get; set; }
        public static HttpContext Context { get; set; }

        public static string Solicitud(SolicitudWeb solicitud, string parametros)
        {
            StringBuilder sb = new StringBuilder(2000);
            StringWriter sw = new StringWriter(sb);
            HttpResponse response = new HttpResponse(sw);
            HttpRequest request = new HttpRequest(string.Empty, Util.Configuracion(Lenguaje.URL), parametros);
            HttpContext context = new HttpContext(request, response);
            Context = HttpContext.Current = context;
            solicitud.ProcessRequest(context);
            return sb.ToString();
        }
        public static string Solicitud(SolicitudWeb solicitud, HttpRequest request)
        {
            StringBuilder sb = new StringBuilder(2000);
            StringWriter sw = new StringWriter(sb);
            HttpResponse response = new HttpResponse(sw);
            HttpContext context = new HttpContext(request, response);
            Context = HttpContext.Current = context;
            solicitud.ProcessRequest(context);
            return sb.ToString();
        }
    }
}
