// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Interfase que define el comportamiento de los eventos
    /// </summary>
    /// <typeparam name="TContexto">Contexto del del evento</typeparam>
    public interface IEvento<TContexto> : ISemilla, IContexto<TContexto>
        where TContexto : ISemilla
    {
        /// <summary>
        /// inicio de la ejecución
        /// </summary>
        //void Ejecutar(Evento ev);
        void Ejecutar();
    }
}
