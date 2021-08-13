// Autor : Juan Parra
// 3Soft
namespace Jen.IU.Generacion
{
    using System.Text;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using Jen;
    /// <summary>
    /// Pinta el largo definido en el campo
    /// </summary>
    [Serializable]
    public class SQLSelect : ObjetoGrafico<Jen.Registro>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SQLSelect()
            : base()
        {
            Id = Clase = "SQLSelect";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected SQLSelect(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "SQLSelect";
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            StringBuilder ret = new StringBuilder(100);

            foreach (Campo cm in Contexto.Campos)
            {
                ret.Append(cm.Nombre);
                ret.Append(" ,");

            }
            ret.Length--;
            return ret.ToString();
        }
    }
}
