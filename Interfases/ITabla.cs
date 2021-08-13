// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Objetos que implementan tabla
    /// </summary>
    public interface ITabla : ISemilla
    {
        /// <summary>
        /// acceso a la tabla del objeto
        /// </summary>
        string Tabla { get; set; }
    }
}
