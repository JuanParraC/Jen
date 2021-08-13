// Autor : Juan Parra
// 3Soft
#if AplicacionWeb
namespace Jen
{
    using System;
    using System.Runtime.CompilerServices;
    /// <summary>
    /// Evento http para restaurar los semillas ocupados en un request
    /// </summary>
    [CompilerGlobalScope]
    public class Aplicacion : System.Web.HttpApplication
    {
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            Constructor.Restaurar();
        }
        protected void Application_OnError(object sender, EventArgs e)
        {
            Constructor.Restaurar();
        }
    }
}
#endif