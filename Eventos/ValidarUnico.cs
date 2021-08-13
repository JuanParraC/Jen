// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#if LOGGER
    using NLog;
#endif
    /// <summary>
    /// Evento para validar registros con especificación unica para sus campos
    /// </summary>
    [Serializable]
    public class ValidarUnico : Evento<Registro>
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif        
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
        public ValidarUnico()
            : base()
        {
            Id = Clase = Lenguaje.ValidarUnico;
        }
        /// <summary>
        /// Constructor para la deserialización binaria
        /// </summary>
        /// <param name="info">Lista de propiedades</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarUnico(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarUnico;
        }
        /// <summary>
        /// Punto de entrada para la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // instancia un consultor para la validación unica
            Consultor unico = new Consultor();
            // recupera el contexto de la base de datos
            //IConsultable contextoBD = Contexto.BaseDeDatos.Contexto;
            // setea el context del consultor
            Registro registro = Contexto;
            unico.Consultado = registro;

            BaseDatos bd;
#if MultipleActiveResultSets
            bd = registro.BaseDeDatos;
#else
            bd = (Jen.BaseDatos)Constructor.Embriones[registro.BaseDeDatos.Clase].Germinar();
            bd.Driver = registro.BaseDeDatos.Driver;            
#endif   
            unico.BaseDeDatos = bd;
            // crea la salida del consultor
            unico.Campos = new Campos();
            unico.Campos.Agregar(new Campo() 
            {
                Id = Lenguaje.d,
                Nombre = Lenguaje.Uno,
                ExpresionSql = true
            });
            // crea los criterios del consultor
            unico.Criterios = new Recipiente<Campo>();

            foreach(Campo campo in Contexto.Campos)
                if (campo.Unico)
                    unico.Criterios.Agregar(campo);

			bd.Recuperacion = Recuperacion.PorConsulta;
#if TRACE				
			//Jen.Util.ToLog ("pg.txt", "Insertar", "Antes de validar unico para " + registro.Id  );
            _logger.Debug(string.Concat("Antes de validar unico para ", registro.Id));
#endif	
			if (unico.Recuperar ()) {
				if (unico.hayDatos) {   // hay datos
					registro.Consejos = string.IsNullOrEmpty (Consejo) ? string.Concat (registro.Articulo, Lenguaje.Espacio,
						registro.Clase, Lenguaje.Espacio,
						registro.Etiqueta, Lenguaje.Espacio,
						Lenguaje.YaExiste, Lenguaje.Espacio,
						Contexto.Tabla) : Consejo;
					registro.Estado |= Estado.Error;
				}
			}
            // restaura el contexto de la base de datos
            //Contexto.BaseDeDatos.Contexto = contextoBD;
#if !MultipleActiveResultSets
            bd.CerrarConexion();
#endif
		}
    }
}