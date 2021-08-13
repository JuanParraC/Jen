// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Interface para los objetos que poseen contexto
    /// </summary>
    /// <typeparam name="Contexto">acceso al contexto de la clase</typeparam>
    public interface IContexto<T>
        where T : ISemilla
    {
        /// <summary>
        /// Acceso al contexto de la clase
        /// </summary>
        T Contexto { get; set; }

    }
}
