// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Convert = System.Convert;
    using DateTime = System.DateTime;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento para la validación del formato fecha del campo
    /// </summary>
    [Serializable]
    public class ValidarFecha : Evento<Campo>
    {
        /// <summary>
        /// Constructo por defecto del evento
        /// </summary>
        public ValidarFecha()
            : base()
        {
            Id = Clase = Lenguaje.ValidarFecha;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades serialziadas</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarFecha(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarFecha;
        }
        /// <summary>
        /// Punto de entrada de la valdación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            //direcciona el campo a validar
            Campo campo = Contexto;
            // obtiene el valor crudo del campo
            string valor = campo.ToString();
            // viene un nulo no valia fecha
            if (valor.Equals(Lenguaje.Null))
            {
                return;
            }

            // obtiene el formato de fecha especificado en el campo
            string formato = campo.Formato;
            // valida que el campo tenga un valor
            if (!string.IsNullOrEmpty(valor))  
            {
                // obtiene la fecha 
                DateTime dtFecha = Convert.ToDateTime(valor, Util.Cultura);
                // compara la fecha con el valor fecha del campo
                string strFecha = dtFecha.ToString(formato, Util.Cultura);
                // si no ha concordancia informa el error
                if (!strFecha.Equals(valor))
                {
                    campo.Consejos = string.IsNullOrEmpty(Consejo) ? string.Concat(Contexto.Articulo, Lenguaje.Espacio,
                        Contexto.Clase, Lenguaje.Espacio,
                        Contexto.Etiqueta, Lenguaje.Espacio,
                        Lenguaje.FechaValidaConFormato, 
                        Lenguaje.Espacio, formato) :  Consejo;
                    campo.Estado |= Estado.Error;
                }
            }
        }
    }
}