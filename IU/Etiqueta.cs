// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Etiqueta de un contexto
    /// </summary>
    [Serializable]
    public class Etiqueta<T> : ObjetoGrafico<T>
         where T : ISemilla, IEtiqueta
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Etiqueta()
            : base()
        {
            Id = Clase = Lenguaje.Etiqueta;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Etiqueta(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Etiqueta;
        }
        /// <summary>
        /// Pinta la etiqueta del contexto
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {
            return Contexto.Etiqueta;
        }
    }
}
