// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Pinta el Identificador de un Semilla
    /// </summary>
    [Serializable]
    public class IdPadre<T> : ObjetoGrafico<T>
        where T : ISemilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public IdPadre()
            : base()
        {
            Id = Clase = "IdPadre";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected IdPadre(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "IdPadre";
        }
        /// <summary>
        /// pinta el identificador del objeto
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            return Contexto.Padre.Id;
        }
    }
}
