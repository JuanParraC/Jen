// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using System.Security.Permissions;
    using DbDataReader = System.Data.Common.DbDataReader;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que permite validar campos con expresión SQL
    /// </summary>
    [Serializable]
    public class ValidarExpresionSQL : Evento<Registro>
    {
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
        public ValidarExpresionSQL()
            : base()
        {
            Id = Clase = Lenguaje.ValidarExpresionSQL;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de desrialización</param>
        protected ValidarExpresionSQL(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarExpresionSQL;
            _expresion = Constructor.Embriones[Lenguaje.Plantilla].Germinar<Plantilla>(info, context);
        }
        /// <summary>
        /// Punto de entrada de la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            //direcciona el regPadre
            Registro registro = Contexto;
            BaseDatos bd;
#if MultipleActiveResultSets
                bd = registro.BaseDeDatos;
#else
            bd = (Jen.BaseDatos)Constructor.Embriones[registro.BaseDeDatos.Clase].Germinar();
            bd.Driver = Contexto.BaseDeDatos.Driver;
#endif            
            _expresion.Enlazar(registro);
            string expresion = _expresion.Pintar();
            //si la expresion tiene valor  
            if (!string.IsNullOrEmpty(expresion))
            {
                // ejecuta la consulta de validación
				using (ILectorBD datos = bd.OperacionBD (expresion)) {
					// si existen datos informa el error
					if (datos.HasRows)
						while (datos.Read ())
							if (!(datos.GetValue (0).ToString ().Equals (Lenguaje.Uno))) {
								registro.Consejos = string.IsNullOrEmpty (Consejo) ? string.Concat (registro.Articulo, Lenguaje.Espacio,
									registro.Clase, Lenguaje.Espacio,
									registro.Etiqueta,
									Lenguaje.FalloAlValidar) : Consejo;
								registro.Estado |= Estado.Error;
								break;
							}
				}
            }
#if !MultipleActiveResultSets
            bd.CerrarConexion();
#endif
        }
        private Plantilla _expresion;
        /// <summary>
        /// plantilla que representa la expresion sql
        /// </summary>
        public Plantilla Expresion
        {
            get { return _expresion; }
            set { _expresion = value; }
        }
        #region GetObjectData
            /// <summary>
            /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
            /// </summary> 
            [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                _expresion.GetObjectData(info, context);
            }
        #endregion
    }
}