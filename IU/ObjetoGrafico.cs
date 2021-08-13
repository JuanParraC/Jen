// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Objeto grafico para acceder a valores del modelo de procesamiento y pintar
    /// una salida de texto
    /// </summary>
    [Serializable]
    public abstract class ObjetoGrafico : Semilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        protected ObjetoGrafico()
            : base()
        {
            Genero = Genero.Masculino;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected ObjetoGrafico(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// pinta el objeto grafico
        /// </summary>
        /// <returns>un string con el contenido del pintado del objeto</returns>
        public abstract string Pintar();



    }
    /// <summary>
    /// Objeto grafico con contexto
    /// </summary>
    /// <typeparam name="T">objeto asociado al objeto gráfico</typeparam>
    [Serializable]
    public abstract class ObjetoGrafico<T> : ObjetoGrafico
        where T : ISemilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        protected ObjetoGrafico()
            : base()
        {
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected ObjetoGrafico(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
        }

        #region propiedad contexto
        private T _contexto;
        /// <summary>
        /// representa el ambito del objeto grafico
        /// </summary>
        public T Contexto
        {
            get { return _contexto; }
            set { _contexto = value; }
        }
        #endregion
    }
}