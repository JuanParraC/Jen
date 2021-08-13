// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Objeto que permite agrupar un campo en base al valor anterior, supone una iteración
    /// de un conjunto de datos tabulares
    /// </summary>
    [Serializable]
    public class Agrupar : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Agrupar()
            : base()
        {
            Id = Clase = Lenguaje.Agrupar;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Agrupar(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Clase = Lenguaje.Agrupar;
        }
        string valorAnterior = string.Empty;
        /// <summary>
        /// Pinta la info en base al valor anterior
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {
            string valorCampo = Contexto.ToString();
            if (valorCampo.Equals(valorAnterior))
                return string.Empty;
            valorAnterior = valorCampo;
            return Contexto.Valor;
        }
    }
}
