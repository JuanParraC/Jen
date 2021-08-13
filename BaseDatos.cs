// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using EventArgs = System.EventArgs;
    using IDbConnection = System.Data.IDbConnection;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using StringBuilder = System.Text.StringBuilder;
    using CommandType = System.Data.CommandType;
    using Jen.Json;
#if LOGGER
    using NLog;
#endif
    public class SQLParam : Dato
    {
        public object Tipo { get; set; }
    }


    /// <summary>
    /// BaseDeDatos es la clase base para realizar operaciones de base de datos
    /// </summary>
    [Serializable]
    public abstract class BaseDatos : Semilla, IContexto<IConsultable>
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif   
        internal event System.EventHandler AntesDeRealizarUnaOperacion;
        public string fromHint = string.Empty;
        /// <summary>
        /// contructor de la base de datos
        /// </summary>
        protected BaseDatos() : base()
        {
            IniBaseDatos();
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected BaseDatos(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            IniBaseDatos();
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            // obtiene el modo de recuperación de la base de datos
            _recuperacion = (Recuperacion)info.GetValue(serie.Valor(), typeof(Recuperacion));
            _driver = info.GetString(serie.Valor());
        }
        void IniBaseDatos()
        {
#if !RuntimeCache
            Atributos = new Atributos();
            Atributos.Padre = this;
#endif
            AntesDeRealizarUnaOperacion += InicializarConexion;
        }
        #region campos
        // _variables de paginación
        internal string inicio, fin;
        internal Catalogo catalogo;
        public Catalogo Catalogo
        {
            get
            {
                return catalogo;
            }
            set
            {
                catalogo = value;
            }
        }
        #endregion
        #region propiedades
        #region propiedad Conexion
        public abstract IDbConnection Conexion
        {
            get;
        }
        internal abstract string SqlPag(string lIni, string lFin);
        #endregion
        #region propiedad Contexto
        private IConsultable _contexto;
        /// <summary>
        /// <c>context : </c> representa el ambito de la base de datos
        /// </summary>
        public IConsultable Contexto
        {
            get { return _contexto; }
            set { _contexto = value; }
        }
        #endregion
        #region propiedad TipoOperacion
        private CommandType _tipoOperacion = CommandType.Text;
        /// <summary>
        /// <c>context : </c> representa el ambito de la base de datos
        /// </summary>
        public CommandType TipoOperacion
        {
            get { return _tipoOperacion; }
            set { _tipoOperacion = value; }
        }
        #endregion

        #region propiedad Descriptor
        //declara el origen _descriptor privado para la propiedad
        private RDBMS.Descriptores.DescBD _descriptor = null;
        /// <summary>
        /// Gets or sets the descriptor.
        /// </summary>
        /// <value>The descriptor.</value>
        public RDBMS.Descriptores.DescBD Descriptor
        {
            get { return _descriptor; }
            set
            {
                _descriptor = value;
            }

        }
        #endregion
        #region propiedad Driver
        //declara el origen _driver privado para la propiedad
        private string _driver = string.Empty;
        /// <summary>
        /// <c>driver : </c> propiedad driver.
        /// </summary>  
        public string Driver
        {
            get { return _driver; }
            set
            {

#if RuntimeCache
						string drv = _driver;
                        if (Atributos.Respaldable(Atributo.driver))
                            Atributos.Agregar(Atributo.driver, 
                                delegate() 
                                { 
                                    _driver = drv; 
                                });
#endif
                _driver = value;
            }

        }
        #endregion
        #region propiedad Usuario
        //declara el origen _driver privado para la propiedad
        private string _usuario = string.Empty;
        /// <summary>
        /// <c>Usuario : </c> cuenta de usuario de la base de datos.
        /// </summary>  
        public string Usuario
        {
            get { return _usuario; }
            set
            {

#if RuntimeCache
						string usr = _usuario;
                        if (Atributos.Respaldable(Atributo.cuenta))
                            Atributos.Agregar(Atributo.cuenta, 
                                delegate() 
                                {
                                    _usuario = usr; 
                                });
#endif
                _usuario = value;
            }
        }
        #endregion
        #region propiedad Error
        //declara el _error privado para la propiedad
        private string _error;
        /// <summary>
        /// <c>error :</c> propiedad error.
        /// </summary>  
        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }
        #endregion
        #region propiedad FormatosFecha
        /// <summary>
        /// <c>formatosFecha :</c> propiedad formatosFecha.
        /// </summary>  
        public abstract Traductor FormatosFecha
        {
            get;
        }
        #endregion
        #region propiedad Recuperacion
        private Recuperacion _recuperacion;
        /// <summary>
        /// <c>modoDeRecuperacion : </c> especifica el modo que consulta a la base de datos
        /// por defecto en recuperación por registroClave
        /// </summary>
        public Recuperacion Recuperacion
        {
            get
            {
                return _recuperacion;
            }
            set
            {

#if RuntimeCache
						Recuperacion mr = _recuperacion;
                        // respalda el primer valor seteado
                        if (Atributos.Respaldable(Atributo.recuperacion))
                            Atributos.Agregar(Atributo.recuperacion, delegate() { _recuperacion = mr; });
#endif
                _recuperacion = value;
            }
        }
        #endregion
        #region propiedad numeroFila
        /// <summary>
        /// <c>separadorAlias : </c> propiedad separadorAlias.
        /// </summary>  
        internal abstract string NumeroFila
        {
            get;
        }
        #endregion
        #region propiedad separadorAlias
        /// <summary>
        /// <c>separadorAlias : </c> propiedad separadorAlias.
        /// </summary>  
        public abstract string SeparadorAlias
        {
            get;
        }
        #endregion
        #region propiedad sqlContador
        //declara el origen _sqlContador privado para la propiedad
        private string _sqlContador = string.Empty;
        /// <summary>
        /// <c>sqlContador : </c> propiedad sqlContador.
        /// </summary>  
        public string SqlContador
        {
            get
            {
                // eliminar la paginación antes de contar
                inicio = fin = string.Empty;
                return _sqlContador;
            }
            set
            {
                _sqlContador = value;
            }
        }
        #endregion
        #region propiedad tipoTransitividad
        /// <summary>
        /// <c>tipoTransitividad : </c> propiedad tipoTransitividad.
        /// </summary>  
        internal abstract string[][] TipoTransitividad
        {
            get;
        }
        #endregion
        #region propiedad ultimaTransaccion
        //declara el origen _ultimaTransaccion privado para la propiedad
        private string _ultimaTransaccion = string.Empty;
        /// <summary>
        /// <c>UltimaTransaccion : </c> propiedad UltimaTransaccion.
        /// </summary>  
        public string UltimaTransaccion
        {
            get { return _ultimaTransaccion; }
            set { _ultimaTransaccion = value; }
        }
        #endregion




        #region propiedad SQL
        public string SQL { get; set; }
        #endregion

        #endregion
        #region métodos
        #region actualizar
        /// <summary>
        /// <c>actualizar : </c> ejecuta una transacción sobre la base de datos
        /// considerando la especificacion sql
        /// </summary>
        public virtual void Actualizar()
        {
            if (Contexto.Ejecutar(Evento.AntesDeActualizar))
            {
                using (ILectorBD lector = OperacionBD(ComponerSQLActualizar()))
                {
                    Contexto.Ejecutar(Evento.DespuesDeActualizar);
                }
            }
        }
        /// <summary>
        /// inserta en una base de datos
        /// </summary>
        public virtual void Insertar()
        {
            if (Contexto.Ejecutar(Evento.AntesDeInsertar))
            {
                using (ILectorBD lector = OperacionBD(ComponerSQLInsertar()))
                {
                    Contexto.Ejecutar(Evento.DespuesDeInsertar);
                }
            }
        }
        /// <summary>
        /// elimina en una base de datos
        /// </summary>
        public virtual void Eliminar()
        {
            if (Contexto.Ejecutar(Evento.AntesDeEliminar))
            {
                using (ILectorBD lector = OperacionBD(ComponerSQLEliminar()))
                {
                    Contexto.Ejecutar(Evento.DespuesDeEliminar);
                }
            }
        }
        #endregion
        #region cerrarConeccion
        public void CerrarConexion()
        {
            _error = string.Empty;
            AntesDeRealizarUnaOperacion += InicializarConexion;
            if (Conexion != null)
            {
                //if (Conexion.State == System.Data.ConnectionState.Open
                Conexion.Close();
            }
        }
        #endregion
        #region Exportar

        public abstract void Exportar(bool recrear, Objeto cfg, Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr);
        public abstract string Exportar(bool recrear, Objeto cfg, Registro regOrigen, Recipiente<Dato> instr);
        public abstract string Exportar(Objeto cfg, Campo cmOrigen);
        #endregion

        #region componerSQLActualizacion
        /// <summary>
        /// <c>componerSQLActualizacion :</c> construye el string sql para actualizar en la
        /// a la base de datos.
        /// </summary>
        /// <returns>retorna el string de conexion a la base de datos</returns>
        public virtual string ComponerSQLActualizar()
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(Lenguaje.Update);
            sqlOrigen(sb);
            sb.Append(Lenguaje.Set);
            sqlListaCampoActualizacion(sb);
            sb.Append(Lenguaje.Espacio);
            SqlCriterios(sb);
            _ultimaTransaccion = sb.ToString();
#if TRACE

            _logger.Debug(string.Concat("ComponerSQLActualizar ", _ultimaTransaccion));
#endif
            return _ultimaTransaccion;
        }
        #endregion
        #region componerSQLInsercion
        /// <summary>
        /// <c>componerSQLInsercion :</c> construye el string sql para insertar en la
        /// a la base de datos.
        /// </summary>
        /// <returns>retorna el string de conexion a la base de datos</returns>
        public virtual string ComponerSQLInsertar()
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(Lenguaje.InsertInto);
            sqlOrigen(sb);
            sb.Append(Lenguaje.Espacio);
            sb.Append(Lenguaje.AbreParentesis);
            sqlListaCampoInsercion(sb);
            sb.Append(Lenguaje.CierreParentesis);
            sb.Append(Lenguaje.Values);
            sb.Append(sqlValores());
            sb.Append(Lenguaje.CierreParentesis);
            _ultimaTransaccion = sb.ToString();
#if TRACE

            _logger.Debug(string.Concat("ComponerSQLInsertar ", _ultimaTransaccion));
#endif
            return _ultimaTransaccion;
        }
        #endregion
        #region componerSQLEliminar
        /// <summary>
        /// <c>componerSQLEliminar :</c> construye el string sql para eliminar 
        /// </summary>
        /// <returns>retorna el string de conexion a la base de datos</returns>
        public virtual string ComponerSQLEliminar()
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(Lenguaje.Delete);
            sb.Append(Lenguaje.From);
            sqlOrigen(sb);
            SqlCriterios(sb);
            _ultimaTransaccion = sb.ToString();
#if TRACE

            _logger.Debug(string.Concat("ComponerSQLEliminar ", _ultimaTransaccion));
#endif
            return _ultimaTransaccion;
        }
        #endregion
        #region componerSQLConsulta
        /// <summary>
        /// <c>componerSQLConsulta :</c> construye el string sql para consultar en la
        /// a la base de datos.
        /// </summary>
        /// <returns>retorna el string de conexion a la base de datos</returns>
        public string ComponerSQLConsulta(string lIni, string lFin)
        {
            string distinto = string.Empty;
            IConsultable consultable = Contexto;
            if (consultable.Distinto) distinto = Lenguaje.Distinct;
            bool paginacion = !string.IsNullOrEmpty(lFin);
            StringBuilder sb = new StringBuilder(500);
            if (paginacion)
            {
                sb.Append(Lenguaje.Select);
                sb.Append(distinto);
                distinto = string.Empty;
                sqlSeleccionPaginacion(sb);
                sb.Append(" from (");
            }
            sb.Append(Lenguaje.Select);
            sb.Append(distinto);
            sqlSeleccion(sb);
            if (paginacion)
            {
                if (_contexto.Ordenadores == null)
                {
                    sb.Append(", row_number() over(order by " + Contexto.Campos[0].Nombre + ") " + SeparadorAlias + " rowNumber");
                }
                else
                {
                    sb.Append(", row_number() over(");
                    sqlOrdenacion(sb);
                    sb.Append(") " + SeparadorAlias + " rowNumber");
                }
            }
            sb.Append(Lenguaje.From);
            string origen = sqlOrigen(sb);
            sb.Append(fromHint);
            string condicion = SqlCriterios(sb, true);
            if (!paginacion)
            {
                sqlOrdenacion(sb);
            }
            else
            {
                sb.Append(SqlPag(lIni, lFin));
            }
            _ultimaTransaccion = sb.ToString();
            sb.Length = 0;
            sb.Append(Lenguaje.SelectCount);
            sb.Append(SeparadorAlias);
            sb.Append(Lenguaje.CampoCuenta);
            sb.Append(Lenguaje.From);
            sb.Append(origen);
            sb.Append(fromHint);
            sb.Append(condicion);
            _sqlContador = sb.ToString();
#if TRACE
            _logger.Debug(string.Concat("ComponerSQLConsulta ", _ultimaTransaccion));
#endif
            return _ultimaTransaccion;
        }
        #endregion
        #region comprometer
        /// <summary>
        ///  compromete los cambios en la base de datos
        /// </summary>
        public abstract void Comprometer();
        #endregion
        #region consultar
        /// <summary>
        /// <c>OperacionBD : </c> ejecuta una operacion sobre la base de datos
        /// </summary>
        /// <mr name="par">arreglo de parametros</mr>
        /// <returns>retorna un lector de base de datos </returns>

        public abstract ILectorBD OperacionBD(params object[] par);

        /// <summary>
        /// <c>consultar : </c> ejecuta una consulta sobre la base de datos
        /// considerando la especificacion de campos del context
        /// </summary>
        /// <returns>retorna el resultado de la consulta en un DataSet</returns>
        public ILectorBD Consultar()
        {
            Requerimiento requerimiento = Contexto.Requerimiento;
            // resetea inicio y fin 
            inicio = fin = string.Empty;
            if (requerimiento != null)
            {
                if (requerimiento.Parametros[Lenguaje.Inicio] != null)
                {
                    inicio = requerimiento.Parametros[Lenguaje.Inicio];
                }
                if (requerimiento.Parametros[Lenguaje.Limite] != null)
                {
                    fin = requerimiento.Parametros[Lenguaje.Limite];
                }
            }
            _error = string.Empty;
            return OperacionBD(ComponerSQLConsulta(inicio, fin));
        }

        #endregion
        #region deshacer
        /// <summary>
        ///  deshace los cambios en la base de datos
        /// </summary>
        public abstract void Deshacer();
        #endregion
        #region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            info.AddValue(serie.Valor(), _recuperacion, typeof(Recuperacion));
            info.AddValue(serie.Valor(), _driver);
        }
        #endregion
        #region inicializarConexion
        /// <summary>
        /// <c>inicializarConexion : </c> inicializa la conextividad 
        /// con la base de datos
        /// </summary>
        internal virtual void InicializarConexion(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(_driver))
            {
                _driver = Util.Configuracion(Id);
            }
            // desconecta el evento de conexión durante todo el procesamiento del request
            AntesDeRealizarUnaOperacion -= InicializarConexion;

#if RuntimeCache
                    
                    if (Atributos.Respaldable(Atributo.conexion))
                            // agrega el restaurador que permite el cierre de la conexion
                            Atributos.Agregar(Atributo.conexion,
                                delegate() 
                                {
                                    //vuelve a conectar el evento
                                    AntesDeRealizarUnaOperacion += InicializarConexion;
                                    //cierra la conexion a la base de datos
                                    CerrarConexion(); 
                                } 
                            );
#endif
        }
        /// <summary>
        /// ejecuta la conexion a una base de datos
        /// </summary>
        /// <param name="e">parametros de conexion a la base de datos</param>
        protected void ConectarBaseDatos()
        {
            if (AntesDeRealizarUnaOperacion != null)
            {
                AntesDeRealizarUnaOperacion(this, null);
            }
        }
        #endregion
        #region iniciarTransaccion
        /// <summary>
        ///  indica al motor de base de datos que va a iniciar una transaccion
        /// </summary>
        public abstract void IniciarTransaccion();
        #endregion
        #region iniciarTransaccion
        /// <summary>
        ///  indica al motor de base de datos que va a iniciar una transaccion
        /// </summary>
        #endregion
        #region representacionCampoEnSelect
        /// <summary>
        /// <c>representacionCampoEnSelect : </c> construye la representacion 
        /// que tendrá del origen dentro de un select-list
        /// </summary>
        /// <returns>retorna un string conexion la representacion del origen</returns>
        internal abstract string RepresentacionCampoEnSelect(Campo campo);
        #endregion
        #region representacionFecha
        /// <summary>
        /// <c>representacionFecha : </c> retorna la conversión de fecha en el rdbms
        /// </summary>
        public abstract string RepresentacionFecha(Campo campo);
        #endregion
        #region representacionIdentificadorDeCampo
        /// <summary>
        /// <c>representacionIdentificadorDeCampo : </c> contruye la identificacion 
        /// del origen para las construcciones de sql como insert, delete o update
        /// </summary>
        /// <returns>retorna un string conexion la representacion del origen</returns>
        internal virtual string RepresentacionIdentificadorDeCampo(Campo campo)
        {
            string tabla = campo.Tabla;
            if (string.IsNullOrEmpty(tabla))
            {
                return campo.Nombre;
            }
            // caso canonico
            return string.Concat(tabla, Lenguaje.Punto, campo.Nombre);
        }
        #endregion
        #region representacionSecuenciaDeCampo
        /// <summary>
        /// <c>representacionSecuenciaDeCampo : </c>construye la representación del valor de secuencia
        /// </summary>
        /// <returns>retorna un string conexion la representacion del origen</returns>
        //internal abstract string RepresentacionSecuenciaDeCampo(Campo campo);
        #endregion
        #region representacionValorDeCampo
        /// <summary>
        /// <c>representacionValorDeCampo : </c>construye la representación del valor del origen
        /// </summary>
        /// <returns>retorna un string conexion la representacion del origen</returns>
        internal string RepresentacionValorDeCampo(Campo campo)
        {
            string ret = Lenguaje.Null;
            string valDefecto = string.Empty;
            Tipo campoTipo = campo.Tipo;
            // valor del semilla se usa ToString para obtener el valor sin traducción
            string campoValor = campo.ToString();
            if (campo.Propiedades != null)
            {
                if (campo.Propiedades.ContainsKey("Defecto"))
                {
                    ret = valDefecto = campo.Propiedades["Defecto"];
                }
            }
            if (campoValor.ToLower().Equals("null"))
            {
                return "null";
            }
            if ((campoTipo == Tipo.Texto) && (string.IsNullOrEmpty(campoValor)))
            {
                if (!string.IsNullOrEmpty(valDefecto))
                {
                    ret = campoValor = valDefecto;
                }
                else
                {
                    return Lenguaje.Comilla + Lenguaje.Comilla;
                }
            }

            // si el valor del origen en vacío retorna null
            if (string.IsNullOrEmpty(campoValor)) return ret;
            // existe valor, se captura desde el origen
            ret = campoValor;
            if (campoTipo == Tipo.Numerico)
            {
                string op = campo.Operador.Trim();
                if (op.Equals("=") ||
                    op.Equals(">") ||
                    op.Equals(">=") ||
                    op.Equals("<") ||
                    op.Equals("<=") ||
                    op.Equals("<>") ||
                    op.Equals("!="))
                    return ret.Replace(Lenguaje.Coma, Lenguaje.Punto);
            }
            if (campoTipo == Tipo.Texto)
            {
                return string.Concat(Lenguaje.Comilla,
                      ret.Replace(Lenguaje.Comilla, Lenguaje.DobleComilla),
                      Lenguaje.Comilla);
            }
            if (campoTipo == Tipo.Fecha)
            {
                //string campoFormato = campo.Formato;
                if (string.IsNullOrEmpty(campo.Formato))
                {
                    campo.Formato = Lenguaje.FmtAhora;
                }
                return RepresentacionFecha(campo);
            }
            return ret;
        }
        #endregion
        #region sqlCriterios
        internal string SqlCriterios(StringBuilder sb, bool expresionSql = false)
        {
            _contexto.Ejecutar(Evento.AntesDeComponerCriterios);
            StringBuilder clausulaSql = new StringBuilder();
            Recipiente<Relacion> transitividad = Contexto.Transitividad;
            Recipiente<Campo> criterios = _contexto.Criterios;
            // valida el caso sin condiciones
            bool sinCriterios = true;
            if (criterios != null)
            {
                if (criterios.Largo > 0)
                {
                    sinCriterios = false;
                }
            }
            bool sinTransitividad = true;
            if (transitividad != null)
            {
                if (transitividad.Largo > 0)
                {
                    sinTransitividad = false;
                }
            }
            // Sin criterios ni transitividad ?
            if (sinCriterios && sinTransitividad)
            {
                return string.Empty;
            }
            clausulaSql.Append(Lenguaje.Where);
            clausulaSql.Append(Lenguaje.AbreParentesis);
            int largoCnnWhere = 0;  //  hay 6 si no vienen criterios;
            bool hayCriterios = false;
            bool hayRelaciones = false;
            Campo ultimoCampo = null;
            if (criterios != null)
            {
                
                foreach (Campo campo in criterios.Contenido)
                {
                    if ((Recuperacion == Recuperacion.PorConsulta) || (campo.Clave))
                    {
                        if (!string.IsNullOrEmpty(campo.ToString()))
                        {
                            if ((!campo.ExpresionSql) || (expresionSql))
                            {
                                clausulaSql.Append(RepresentacionIdentificadorDeCampo(campo));
                                clausulaSql.Append(campo.Operador);
                                clausulaSql.Append(RepresentacionValorDeCampo(campo));
                                //clausulaSql.Append(Lenguaje.And);
                                clausulaSql.Append(campo.AndOr);

                                hayCriterios = true;
                                ultimoCampo = campo;
                            }
                        }

                    }
                }
                if (hayCriterios)
                {
                    largoCnnWhere = ultimoCampo.AndOr.Length;
                }
            }
            //Relacion rel;
            //for (int i = 0; i < transitividad.Largo; i++)
            foreach (Relacion rel in transitividad.Contenido)
            {
                for (int j = 0; j < rel.Madre.Largo; j++)
                {
                    string[] dialectoTransitividad = TipoTransitividad[(int)rel.TipoRelacion];
                    clausulaSql.Append(RepresentacionIdentificadorDeCampo(catalogo.Registros[rel.Madre.Id].Campos[rel.Madre[j].Id]));
                    clausulaSql.Append(dialectoTransitividad[0]);
                    clausulaSql.Append(Lenguaje.Igual);
                    clausulaSql.Append(dialectoTransitividad[1]);
                    clausulaSql.Append(RepresentacionIdentificadorDeCampo(catalogo.Registros[rel.Hijo.Id].Campos[rel.Hijo[j].Id]));
                    clausulaSql.Append(dialectoTransitividad[2]);
                    clausulaSql.Append(Lenguaje.And);
                    
                    hayRelaciones = true;
                }
            }
            if (hayRelaciones)
            {
                largoCnnWhere = Lenguaje.And.Length;
            }
            // quita el ultimo conector de criterios
            clausulaSql.Length -= largoCnnWhere;
            // cierra el parentesis del where
            clausulaSql.Append(Lenguaje.CierreParentesis);
            // comprueba si pasaron criterios
            if (!hayCriterios)
            {
                clausulaSql.Clear();
            }
            string condicion = clausulaSql.ToString();
            sb.Append(condicion);
            return condicion;
        }
        #endregion
        #region sqlListaCampoActualizacion
        /// <summary>
        /// <c>sqlListaCampoActualizacion : </c> construye la lista updateList basandose en los campos del context
        /// </summary>
        /// <returns>retorna el string updateList</returns>
        internal void sqlListaCampoActualizacion(StringBuilder sb)
        {
            foreach (Campo campo in _contexto.Campos)
            {
                if ((!campo.Autonumerico) && (!campo.ExpresionSql))
                {
                    sb.Append(RepresentacionIdentificadorDeCampo(campo));
                    sb.Append(Lenguaje.Igual);
                    sb.Append(RepresentacionValorDeCampo(campo));
                    sb.Append(Lenguaje.ComaEspacio);
                }
            }
            sb.Length -= Lenguaje.ComaEspacio.Length;
        }
        #endregion
        #region sqlListaCampoInsercion
        /// <summary>
        /// <c>sqlListaCampoActualizacion : </c> construye la lista updateList basandose en los campos del context
        /// </summary>
        /// <returns>retorna el string updateList</returns>
        protected void sqlListaCampoInsercion(StringBuilder sb)
        {
            foreach (Campo campo in _contexto.Campos)
            {
                if (!campo.ExpresionSql)
                {
                    sb.Append(campo.Nombre.ToLower());
                    sb.Append(Lenguaje.ComaEspacio);
                }
            }
            sb.Length -= Lenguaje.ComaEspacio.Length;
        }
        #endregion
        #region sqlOrdenacion
        private void sqlOrdenacion(StringBuilder sb)
        {
            Recipiente<Campo> ordenadores = _contexto.Ordenadores;
            if (ordenadores == null)
            {
                return;
            }
            sb.Append(Lenguaje.OrderBy);
            foreach (Campo campo in ordenadores)
            {
                sb.Append(RepresentacionIdentificadorDeCampo(campo));
                sb.Append(campo.ordenamiento);
                sb.Append(Lenguaje.Coma);
            }
            sb.Length -= Lenguaje.Coma.Length;
        }
        #endregion
        #region sqlOrigen
        protected string sqlOrigen(StringBuilder sb)
        {
            Recipiente<Registro> origenes = _contexto.Origenes();
            StringBuilder clausulaSql = new StringBuilder();
            //for (int i = 0; i< origenes.Largo; i++)
            foreach (Registro r in origenes)
            {
                clausulaSql.Append(r.Tabla.ToLower());
                clausulaSql.Append(Lenguaje.ComaEspacio);
            }
            clausulaSql.Length -= Lenguaje.ComaEspacio.Length;
            string origen = clausulaSql.ToString();
            sb.Append(origen);
            return origen;
        }
        #endregion
        #region sqlSeleccion
        internal void sqlSeleccion(StringBuilder sb)
        {
            foreach (Campo campo in _contexto.Campos)
            {
                sb.Append(RepresentacionCampoEnSelect(campo));
                sb.Append(Lenguaje.ComaEspacio);
            }
            sb.Length -= Lenguaje.ComaEspacio.Length;
        }
        internal abstract void sqlSeleccionPaginacion(StringBuilder sb);
        #endregion
        #region sqlValores
        /// <summary>
        /// <c>sqlFrom : </c> construye la clausula sqlFrom basandose en los campos del context
        /// </summary>
        /// <returns>retorna la clausula sqlFrom</returns>
        protected string sqlValores()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Campo campo in _contexto.Campos)
            {
                if (!campo.ExpresionSql)
                {
                    sb.Append(RepresentacionValorDeCampo(campo));
                    sb.Append(Lenguaje.ComaEspacio);
                }
            }
            sb.Length -= Lenguaje.ComaEspacio.Length;
            return sb.ToString();
        }
        #endregion
        #region transaccion
        /// <summary>
        /// ejecuta una operacion sql sin resultados de retorno
        /// </summary>
        /// <param name="sql"></param>
        public virtual ILectorBD Transaccion(params object[] par)
        {
            return OperacionBD(par);
        }
        #endregion
        #endregion
    }
}