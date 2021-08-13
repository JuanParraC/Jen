// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using DbDataReader = System.Data.Common.DbDataReader;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#if LOGGER
    using NLog;
#endif
    /// <summary>
    /// Evento que genera el valor de secuencia en función de la definicion del motor de base de datos
    /// La subscripción del evento depende del motor de base de datos y se debe definir en la definicion del registro
    /// </summary>
    [Serializable]
	public class CalcularSecuencia : Evento<Registro>
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif         
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
		public CalcularSecuencia() 
        {
            Id = Clase = Lenguaje.CalcularSecuencia;
        }
        /// <summary>
        /// Contructor que instancia el evento desde la definición binaria
        /// </summary>
        /// <param name="info">información del objeto desde la estructura binaria</param>
        /// <param name="context">contexto para la serialización</param>
		protected CalcularSecuencia(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.CalcularSecuencia;
        }
        /// <summary>
        /// Punto de entrada del delegado
        /// </summary>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            if (!En(Estado, Estado.Excluido))
            {
                //Direcciona el registro, en este caso el contexto del evento
                Registro registro = Contexto;
                //direcciona a la base de datos definida en el registro
                BaseDatos bd;
#if MultipleActiveResultSets
                bd = registro.BaseDeDatos;
#else
                bd = (BaseDatos)Constructor.Embriones[registro.BaseDeDatos.Clase].Germinar();
                bd.Driver = registro.BaseDeDatos.Driver;
#endif

                //direcciona la colección de campos del registro
                Campos campos = registro.Campos;
                // declara una variable del tipo campo
                //Campo campo;
                // obtiene el numero de campos del registro
                //int totalCampos = campos.Largo;
                // itera en todos los campos del registro, sin considerar la exclusión de campos
                //for (int i = 0; i < totalCampos; i++)
				foreach(Campo campo in campos.Contenido)
				{
                    // direcciona el iesimo campo
                    //campo = campos[i];
                    // si el campo es autonumerico y no provee valor
                    if (campo.Autonumerico)
                    {
                        if (Semilla.En(campo.Estado, Estado.Excluido))
                        {
                            // restaura su estado
                            campo.Estado -= Estado.Excluido;
                        }
                            // construye el sql que obtiene la secuencia o autonumerico
                        string sqlSecuencia = campo.Propiedades[Lenguaje.SEQ];
                        
#if TRACE				
						//Jen.Util.ToLog ("pg.txt", "CalcularSecuencia", "Antes de " + sqlSecuencia);
                        _logger.Debug(string.Concat("CalcularSecuencia ", sqlSecuencia));
#endif
                        //recupera el valor y se lo asigna al campo
                        using (ILectorBD secuencia = bd.OperacionBD(sqlSecuencia))
                        {
                            if (secuencia.HasRows)
                            {
                                secuencia.Read();
                                registro.Campos.Recuperar(secuencia);
                            }
                        }
                        break;
                    }
                }
#if !MultipleActiveResultSets
                bd.CerrarConexion();
#endif
            }
        }
    }
}