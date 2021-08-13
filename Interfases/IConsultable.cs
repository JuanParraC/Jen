// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Define el diseño de un objeto consultable
    /// </summary>
    public interface IConsultable : ICampos, IOrigenes
    {
        /// <summary>
        ///  todos distintos?
        /// </summary>
        bool Distinto { get; set; }
        /// <summary>
        /// definicion de criterios
        /// </summary>
        Recipiente<Campo> Criterios { get; set; }
        /// <summary>
        /// definicion de ordenadores
        /// </summary>
        Recipiente<Campo> Ordenadores { get; set; }
        /// <summary>
        /// definicion de totalizadores
        /// </summary>
        Recipiente<Campo> Totalizadores { get; set; }
        /// <summary>
        /// Lanzador del evento
        /// </summary>
        /// <param name="trigger">Disparador del evento</param>
        /// <returns></returns>
        bool Ejecutar(Evento trigger);
    }
}
