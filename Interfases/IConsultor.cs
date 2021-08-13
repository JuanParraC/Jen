// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Diseño de un objeto consultor
    /// </summary>
    interface IConsultor : ISemilla, IConsultable
    {
        /// <summary>
        /// link al objeto consultado
        /// </summary>
        IOrigenes Consultado { get; set; }
    }
}
