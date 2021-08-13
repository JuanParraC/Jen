// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que activa los campos autonumericos, supone que se ha ejecutado el evento contrario
    /// </summary>
    [Serializable]
    public class IncluirSecuencia : Evento<Registro>
    {
        /// <summary>
        /// Constructor del delegado
        /// </summary>
        public IncluirSecuencia()
        {
            Id = Clase = Lenguaje.IncluirSecuencia;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Propiedades serializadas del evento</param>
        /// <param name="context">Contexto del proceso de deserialziación</param>
        protected IncluirSecuencia(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.IncluirSecuencia;
        }
        /// <summary>
        /// Punto de entrada del evento
        /// </summary>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            //Direcciona el registro contexto del evento
            Registro registro = Contexto;
            //Campo campo;
            // direcciona la colección de campos del registro
            Campos campos = registro.Campos;
            int totalCampos = campos.Largo;
            // itera en la lista de campos
            //for (int i = 0; i < totalCampos; i++)
			foreach(Campo campo in campos.Contenido)
            {
                // direcciona el iesimo campo
                //campo = campos[i];
                // si el campo es autonumerico y está excluido revierte su estado
                if ((campo.Autonumerico) && Semilla.En(campo.Estado, Estado.Excluido))
                {
                    campo.Estado -= Estado.Excluido;
                    break;
                }
            }
        }
    }
}