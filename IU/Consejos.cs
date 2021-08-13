// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Pinta consejos provisto por el contexto
    /// </summary>
    /// <typeparam name="Contexto">Contexto de los consejos</typeparam>
    [Serializable]
    public class Consejos<T> : ObjetoGrafico<T>
        where T :  ISemilla, IConsejos
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Consejos()
            : base()
        {
            Id = Clase = Lenguaje.Consejos;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Consejos(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Clase = Lenguaje.Consejos;
        }
        /// <summary>
        /// Pinta los consejos del contexto
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {
            return Contexto.Consejos;
        }
    }
}
