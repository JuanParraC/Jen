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
    public class Orden : ObjetoGrafico<Registro>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Orden()
            : base()
        {
            Id = Clase = "Orden";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Orden(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "Orden";
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {

            return "${SQLOrden[:]@@1@}";
        }
    }
}
