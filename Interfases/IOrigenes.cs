// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Diseño contractual para objetos que generan sql
    /// </summary>
    public interface IOrigenes 
    {
        /// <summary>
        /// los origenes representan la clausula from
        /// </summary>
        /// <param name="campos">campos determinan que origenes</param>
        /// <returns>un contenedor de registros</returns>
        Recipiente<Registro> Origenes(params Recipiente<Campo>[] campos);
        /// <summary>
        /// relaciones que definen una transitividad de un modelo
        /// </summary>
        Recipiente<Relacion> Transitividad { get; }
        /// <summary>
        /// driver de conexion a la base de datos
        /// </summary>
        BaseDatos BaseDeDatos { get; }
        /// <summary>
        /// encapsulador de request en el contexto de ejecución http
        /// </summary>
        Requerimiento Requerimiento { get; }
    }
}
