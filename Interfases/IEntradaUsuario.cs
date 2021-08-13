// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Define un metodo que recibe un coleccion de nombre valor que contiene la información
    /// ingresada por el usuario
    /// </summary>
    public interface IEntradaUsuario
    {
        /// <summary>
        /// Entrada de usuario
        /// </summary>
        /// <param name="request">coleccion de nombre valor</param>
        /// <returns>coleccion de campos</returns>
        Recipiente<Campo> EntradaUsuario(System.Collections.Specialized.NameValueCollection request);
    }
}
