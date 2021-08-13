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
    public class AtrsAdd : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public AtrsAdd()
            : base()
        {
            Id = Clase = "AtrsAdd";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected AtrsAdd(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "AtrsAdd";
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
                    break;
                case Tipo.Texto:
                    break;
                case Tipo.Fecha:
                    ret = string.Concat(", mapping: '", 
                                        Contexto.Id, 
                                        "Desc', convert: function (separador) { return Date.fromUKFormat(separador) }");
                    break;
            }
            return ret;
        }
    }
}
