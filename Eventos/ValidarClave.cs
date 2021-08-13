// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento para validar las claves unicas definidas en el registro que no existan en la base de datos
    /// </summary>
    [Serializable]
    public class ValidarClave : Evento<Registro>
    {
        /// <summary>
        /// Constructo por defecto del evento
        /// </summary>
        public ValidarClave()
            : base()
        {
            Id = Clase = Lenguaje.ValidarClave;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarClave(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarClave;
        }
    
        /// <summary>
        /// Punto de entrada para la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // instancia un consultor para la validación por clave
            Consultor clave = new Consultor();
            // recupera el contexto de la base de datos
            //IConsultable contextoBD = Contexto.BaseDeDatos.Contexto;
            // setea el context del consultor
            //clave.Consultado = Contexto;
#if ! MultipleActiveResultSets
            clave.BaseDeDatos = (Jen.BaseDatos)Constructor.Embriones[Contexto.BaseDeDatos.Clase].Germinar();
            clave.BaseDeDatos.Driver = Contexto.BaseDeDatos.Driver;
#endif

            // crea la salida del consultor
            clave.Campos = new Campos();
            clave.Campos.Agregar(new Campo() 
            {
                Id = Lenguaje.d,
                Nombre = Lenguaje.Uno,
                ExpresionSql = true,
                Tabla = Contexto.Tabla
            });
            // crea los criterios del consultor conforme e la validacion de clave
            clave.Criterios = new Recipiente<Campo>();
            foreach (Campo campo in Contexto.Campos)
            {
                if (campo.Clave)
                {
                    // si la clave está conformada por un autonumerico aborta la operacion puesto que la unicidad está asegurada
                    if (campo.Autonumerico)
                    {
                        return;
                    }
                    clave.Criterios.Agregar(campo);
                }
            }

            // recuera la tupla clave del registro
            if (clave.Recuperar())
            // si existe en la base de datos informa el error
            {
                if (clave.datos.HasRows)
                {
                    Contexto.Consejos = string.IsNullOrEmpty(Consejo) ? string.Concat(Lenguaje.RegistroYaExiste, Contexto.Etiqueta) : Consejo;
                    Contexto.Estado |= Estado.Error;
                }
            }
            // restaura el contexto de la base de datos
            //Contexto.BaseDeDatos.Contexto = contextoBD;
#if !MultipleActiveResultSets
            clave.BaseDeDatos.CerrarConexion();
#endif
        }
    }
}