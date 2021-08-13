// Autor : Juan Parra
// 3Soft
namespace Jen.IU.Generacion.Registro
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using Jen;
    using System.Text;

    /// <summary>
    /// Pinta el largo definido en el campo
    /// </summary>
    [Serializable]
    public class XmlReader : ObjetoGrafico<Registro>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public XmlReader()
            : base()
        {
            Id = Clase = "XmlReader";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected XmlReader(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "XmlReader";
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            StringBuilder ret = new StringBuilder();
            if (Contexto.Plantillas != null)
            {
                foreach (Plantilla plt in Contexto.Plantillas)
                {
                    plt.Pintar(ref ret);
                }
            }
            return ret.ToString();
        }
    }
}
