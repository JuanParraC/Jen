// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Pinta un CheckBox según el valor de un campo
    /// </summary>
    [Serializable]
    public class CheckBox : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public CheckBox()
            : base()
        {
            Id = Clase = Lenguaje.CheckBox;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected CheckBox(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Clase = Lenguaje.CheckBox;
        }
        /// <summary>
        /// pinta el campo como chequeado si el el valor el 1
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            if (Contexto.Valor.Equals(Lenguaje.Uno))
                return Lenguaje.Chequeado;
            return string.Empty;
        }
    }
}
