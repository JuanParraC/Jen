// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Pinta el Identificador de un Semilla
    /// </summary>
    [Serializable]
    public class Id<T> : ObjetoGrafico<T>
        where T : ISemilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Id()
            : base()
        {
            Id = Clase = Lenguaje.Id;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Id(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Id;
        }
        /// <summary>
        /// pinta el identificador del objeto
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            return Contexto.Id;
        }
    }
}
