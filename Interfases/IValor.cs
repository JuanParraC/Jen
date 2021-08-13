// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Diseno para objetos que implementan valor
    /// </summary>
    public interface IValor : IValor<string>
    {

    }

    public interface IValor<T>
    {
        /// <summary>
        /// acceso al valor del objeto
        /// </summary>
        T Valor { get; set; }
    }
}
