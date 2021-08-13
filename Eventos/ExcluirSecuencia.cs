// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que desactiva campos autonumericos
    /// </summary>
    [Serializable]
    public class ExcluirSecuencia : Evento<Registro>
    {
        /// <summary>
        /// Constructor del evento
        /// </summary>
        public ExcluirSecuencia() : base()
        {
            Id = Clase = Lenguaje.ExcluirSecuencia;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">propiedades serializadas del evento</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected ExcluirSecuencia(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ExcluirSecuencia;
        }
        /// <summary>
        /// Punto de entrada del evento
        /// </summary>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            //Direcciona el registro contexto del evento
            Registro registro = Contexto;
            // itera por la colección de campos
			//for (int c = 0; c < registro.Campos.Largo; c++) {
			foreach(Campo campo in registro.Campos){
				// si el campo es autonumerico lo excluye
				if (campo.Autonumerico) {
					campo.Estado |= Estado.Excluido;
					break;
				}
			}
        }

    }
}