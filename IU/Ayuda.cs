// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext; 
    /// <summary>
    /// Pinta la ayuda del campo
    /// </summary>
    [Serializable]
    public class Ayuda : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Ayuda()
            : base()
        {
            Id = Clase = Lenguaje.Ayuda;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Ayuda(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            Clase = Lenguaje.Ayuda;
        }
        /// <summary>
        /// Pinta la ayuda del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            return Contexto.Ayuda;
        }
    }
}
