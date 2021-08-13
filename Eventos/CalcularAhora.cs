// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que obtiene la fecha-hora actual del sistema y se la asigna al valor del campo
    /// </summary>
    [Serializable]
    public class CalcularAhora : Evento<Campo>
    {
        /// <summary>
        ///  Constructor, invoca a la super clase y setea en que evento se ejecuta el delegado
        /// </summary>
        public CalcularAhora(): base()
        {
            Id = Clase = Lenguaje.CalcularAhora;
        }
        /// <summary>
        /// Contructor que crea el objeto a partir de la información binaria
        /// </summary>
        /// <param name="info">colección de propiedades</param>
        /// <param name="context">contexto de la serialziación</param>
        protected CalcularAhora(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.CalcularAhora;
        }
        /// <summary>
        /// Punto de entrada para la ejecución del delegado
        /// </summary>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // direcciona el semilla que genera el ahora
            Campo campo = Contexto;
            // asigna el valor ahora al semilla
            campo.Valor = Util.Ahora(campo.Formato);
        }
    }
}