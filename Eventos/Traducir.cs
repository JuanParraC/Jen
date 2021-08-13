// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que traduce el campo en funcion de la lista de valores de traducción para el campo
    /// </summary>
    [Serializable]
    public class Traducir : Valor
    {
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
        public Traducir()
            : base()
        {
            Id = Clase = Lenguaje.Traducir;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades del evento</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected Traducir(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Traducir;
        }
        /// <summary>
        /// Punto de entrada para la ejecución de la traducción del campo
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // direcciona el semilla
            Campo campo = Contexto;
            // nombre del traductor
            string traductor = campo.Traductor;
            // valor crudo del campo
            Valor = campo.ToString();
            // direcciona el traductor
            Traductor T = ((Registro)campo.Padre).Traductores[traductor];
            if (Semilla.En(T.Estado, Estado.Excluido))
            {
                return;
            }
            // valida si el traductor contiene la traducción para el valor alacenado en el campo
            if (T.Existe(Valor))
            {
                Valor = T[Valor].Valor;
            }
        }
    }
}
