/* 
Fecha : 21-3-2013 08:44:22
patrón clases que implementa una accion http
3Soft Ltda
*/
namespace Jen
{
	using System.Web;
	using System.Web.SessionState;
    using Jen.Json;

    public abstract class SolicitudWeb : Semilla, IHttpHandler, IReadOnlySessionState
	{
        public Objeto Json
        {
            get; set;
        }
        public abstract void ProcessRequest(HttpContext context);

		public static void NotificarError(HttpResponse Response, string error)
		{
			Constructor.Restaurar();
			Response.ClearHeaders();
			Response.ClearContent();
			Response.Status = "499 Validaci&oacute;n";
			Response.StatusCode = 499;
			Response.TrySkipIisCustomErrors = true;
			Response.StatusDescription = "Un error ha ocurrido";
			Response.Write(error);
			Response.Flush();
			Response.End();
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}

}