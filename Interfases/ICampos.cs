// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Define una coleccion de campos como parte del objeto
    /// </summary>
    public interface ICampos : ISemilla
    {
        /// <summary>
        /// acceso a la coleccion de campos del objeto
        /// </summary>
        Campos Campos { get; }
    }
}