// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using Double = System.Double;
    using Convert = System.Double;
    /// <summary>
    /// Evento para Validar el formato numerico definido en el campo
    /// </summary>
    [Serializable]
    public class ValidarNumerico : Evento<Campo>
    {
        /// <summary>
        /// Constructor del evento 
        /// </summary>
        public ValidarNumerico()
            : base()
        {
            Id = Clase = Lenguaje.ValidarNumerico;
        }
        /// <summary>
        /// Constructo binario del evento
        /// </summary>
        /// <param name="info">lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected ValidarNumerico(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarNumerico;
        }

        /// <summary>
        /// ejecución de la validación numérica
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // obtiene el valor crudo del campo
            string valor = Contexto.ToString();
            // valida que tenga un valor
            if (!string.IsNullOrEmpty(valor))
            {
                Double retNum;
                bool result = Double.TryParse(valor, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                // realiza la compraración conforme al valor almacenado y el resultado de la expresión regular
                if (!result)
                {
                    // informa la no concordancia entre el valor almacenado y el resultado de 
                    // la expresión regular
                    Contexto.Consejos = string.IsNullOrEmpty(Consejo) ? string.Concat(Contexto.Articulo, Lenguaje.Espacio,
                        Contexto.Clase, Lenguaje.Espacio, 
                        Contexto.Etiqueta, Lenguaje.Espacio,
                        Lenguaje.DebeSerNumerico) : Consejo;
                    Contexto.Estado |= Estado.Error;
                }
            }
        }
    }
}