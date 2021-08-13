// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// Evento para la validación de criterios
    /// </summary>
    [Serializable]
    public class ValidarCriterios : Evento<Registro>
    {
        /// <summary>
        /// Constructor por defecto 
        /// </summary>
        public ValidarCriterios()
            : base()
        {
            Id = Clase = Lenguaje.ValidarCriterios;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">Lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarCriterios(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarCriterios;
        }

        /// <summary>
        /// Punto de entrada para la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // si no existe la coleccion de criterios
            if (Contexto.Criterios == null)
            {
                Contexto.Estado |= Estado.Error;
                return;
            }
            // si no existen elementos en la colección de criterio informa el error
            if (Contexto.Criterios.Largo == 0)
            {
                Contexto.Estado |= Estado.Error;
            }
        }
    }
}