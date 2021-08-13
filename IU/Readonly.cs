// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Pinta un campo como solo lectura si la propiedad está seteada como verdadero
    /// </summary>
    [Serializable]
    public class Readonly : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Readonly()
            : base()
        {
            Id = Clase = Lenguaje.Readonly;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Readonly(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            Id = Clase = Lenguaje.Readonly;
        }
        /// <summary>
        /// Pinta el objeto de solo lectura si esta seteada la propiedad
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {
            if (Contexto.SoloLectura)
               return Lenguaje.Readonly;
            return string.Empty;
        }
    }
}
