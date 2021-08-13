// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Obtieve el valor del objeto que implementa IValor
    /// </summary>
    [Serializable]
    public class Valor<T> : ObjetoGrafico<T>
        where T : IValor, ISemilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Valor()
            : base()
        {
            Id = Clase = Lenguaje.Valor;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Valor(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            Id = Clase = Lenguaje.Valor;
        }
        /// <summary>
        /// Retorna el valor del objeto
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {
            return Contexto.Valor;
        }
    }
}