// Autor : Juan Parra
// 3Soft
namespace Jen.IU.Generacion
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// Pinta el largo definido en el campo
    /// </summary>
    [Serializable]
    public class TipoJS : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public TipoJS()
            : base()
        {
            Id = Clase = "TipoJS";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected TipoJS(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "TipoJS";
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            string ret = string.Empty;
            switch (Contexto.Tipo)
            {
                case Tipo.Numerico:
                    ret = "int";
                    break;
                case Tipo.Texto:
                    ret = "string";
                    break;
                case Tipo.Fecha:
                    ret = "date";
                    break;
                default:
                    ret = "string";
                    break;
            }
            return ret;
        }
    }
}
