// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que pobla la colección de campos con el primer registro recuperado en la consulta
    /// </summary>
    [Serializable]
    public class CargarPrimerRegistro : Evento<Consultor>
    {
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
        public CargarPrimerRegistro()
        {
            Id = Clase = Lenguaje.CargarPrimerRegistro;
        }
        /// <summary>
        /// constructor binario del objeto
        /// </summary>
        /// <param name="info">información para la deserialziación</param>
        /// <param name="context">contexto para la deserialización</param>
        protected CargarPrimerRegistro(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.CargarPrimerRegistro;
        }
        /// <summary>
        /// Punto de entrada del evento
        /// </summary>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
			//Jen.Util.ToLog ("pg.txt", "CargarPrimerRegistro", "Antes de recuperar");
            //direcciona el consultor
            Consultor consultor = Contexto;
            consultor.datos.Read();
            consultor.Campos.Recuperar(consultor.datos);
			//Jen.Util.ToLog ("pg.txt", "CargarPrimerRegistro", "Despues de recuperar");
        }
    }
}