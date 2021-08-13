// Autor : Juan Parra
// 3Soft
namespace Jen.IU.Generacion
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using Jen;
    using System.Text;
    using Jen.Json;

    /// <summary>
    /// Pinta el largo definido en el campo
    /// </summary>
    [Serializable]
    public class VarPass<T> : ObjetoGrafico<T>
        where T : ISemilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public VarPass()
            : base()
        {
            Id = Clase = "VarPass";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected VarPass(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "VarPass";
        }
        public VarPass(Objeto cfg)
        {
            Id = Clase = cfg["Clase"].ToString();
        }

        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            return string.Concat("${", Clase, "[:]@@1@$");
        }
    }
}
