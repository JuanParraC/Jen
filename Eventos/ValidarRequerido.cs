// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento para validar campos requeridos 
    /// </summary>
    [Serializable]
    public class ValidarRequerido: Evento<Campo>
    {
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
        public ValidarRequerido()
            : base()
        {
            Id = Clase = Lenguaje.ValidarRequerido;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarRequerido(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarRequerido;
        }
        /// <summary>
        /// Punto de entrada de la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // direcciona al campo a validar ddMedioDestino
            Campo campo = Contexto;
            // obtiene el valor crudo del evento
            string valor = campo.ToString();
            // si el campo es autonumerico no valida el requerido
            if (campo.Autonumerico)
            {
                return;
            }
            // si el campo contiene el evento que asigna la hora no valida lo requerido.
            if (campo.Existe(Lenguaje.CalcularAhora))
            {
                return;
            }
            // si es nulo informa el error
            if (string.IsNullOrEmpty(valor) || (valor.ToLower().Equals(Lenguaje.Null)))
            {
                campo.Consejos = string.IsNullOrEmpty(Consejo) ? string.Concat(Contexto.Articulo, Lenguaje.Espacio,
                    Contexto.Clase, Lenguaje.Espacio,
                    Contexto.Etiqueta, Lenguaje.Espacio,
                    Lenguaje.DebeSerNoNulo) :  Consejo;
                campo.Estado |= Estado.Error;
            }
        }
    }
}