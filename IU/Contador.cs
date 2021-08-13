// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Pinta el numero de registros recuperados por el consultor
    /// </summary>
    [Serializable]
    public class Contador : ObjetoGrafico<Consultor>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Contador()
            : base()
        {
            Id = Clase = Lenguaje.Contador;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Contador(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Contador;
        }
        /// <summary>
        /// Pinta el número de registros de la consulta
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {
            Consultor contador = Contexto._contador;
            if (Contexto.hayDatos)
				if (contador.Recuperar()) {
#if !MultipleActiveResultSets
					contador.BaseDeDatos.CerrarConexion();
#endif			
				    return contador.Campos[Lenguaje.Cuenta].ToString();
				}
            return string.Empty;
        }
    }
}
