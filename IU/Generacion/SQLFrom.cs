// Autor : Juan Parra
// 3Soft
namespace Jen.IU.Generacion
{
    using System.Text;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// Pinta el largo definido en el campo
    /// </summary>
    [Serializable]
    public class SQLFrom : ObjetoGrafico<Jen.Registro>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SQLFrom()
            : base()
        {
            Id = Clase = "SQLFrom";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected SQLFrom(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "SQLFrom";
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            return Contexto.Tabla;
        }
    }
}
