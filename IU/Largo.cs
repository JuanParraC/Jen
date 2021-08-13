// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// Pinta el largo definido en el campo
    /// </summary>
    [Serializable]
    public class Largo : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Largo()
            : base()
        {
            Id = Clase = Lenguaje.Largo;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Largo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Largo;
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            return Contexto.Largo;
        }
    }
}
