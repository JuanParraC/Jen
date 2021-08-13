// Autor : Juan Parra
// 3Soft
namespace Jen.RDBMS
{
    using System.Text;
    using Jen.Json;
    using EventArgs = System.EventArgs;
    using IDbConnection = System.Data.IDbConnection;
    using SecurityAction = System.Security.Permissions.SecurityAction;
    using SecurityPermissionAttribute = System.Security.Permissions.SecurityPermissionAttribute;
    using SecurityPermissionFlag = System.Security.Permissions.SecurityPermissionFlag;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
    using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
    using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
    using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
    using SQLiteTransaction = Mono.Data.Sqlite.SqliteTransaction;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

#if LOGGER
    using NLog;
#endif
    public class LectorSQLite : ILectorBD
    {

        SQLiteDataReader _lector;
        object _escalar;
        bool esEscalar = false;

        [System.CLSCompliant(false)]
        public LectorSQLite(SQLiteDataReader lector)
        {
            _lector = lector;

        }
        public LectorSQLite(object escalar)
        {
            _escalar = escalar;
            esEscalar = true;

        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _lector.GetEnumerator();
        }
        public bool HasRows
        {
            get
            {
                return _lector.HasRows;
            }
        }
        public void Close()
        {
            _lector.Close();
        }
        public void Dispose()
        {
            _lector.Close();
        }
        public int FieldCount
        {
            get
            {
                return _lector.FieldCount;
            }
        }



        public bool Read()
        {
            return _lector.Read();
        }
        public string GetName(int i)
        {
            return _lector.GetName(i);
        }
        public string GetString(int i)
        {
            return _lector.GetString(i);
        }
        public object GetValue(int i)
        {
            return _lector.GetValue(i);
        }
        public object GetScalar()
        {
            return _escalar;
        }
        public bool IsScalar
        {
            get { return esEscalar; }

        }
    }
    /// <summary>
    /// <c>PostgreSQL : </c> permite la persistencia base de datos postgres
    /// </summary>
    [Serializable]
    public class SQLite : BaseDatos
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif             
        private SQLiteConnection _conexion;
        private SQLiteTransaction _transaccion;
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SQLite()
            : base()
        {
            Id = Clase = Lenguaje.SQLite;
            _formatosFecha = Constructor.crear<Traductor>(Lenguaje.FmtFechasSQLite);
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected SQLite(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.SQLite;
            _formatosFecha = Constructor.Embriones[Lenguaje.Traductor].Germinar<Traductor>(info, context);
        }
        #region propiedades
        #region propiedad conexion
        public override IDbConnection Conexion
        {
            get
            {
                return _conexion;
            }
        }
        #endregion
        #region propiedad formatosFecha
        private static Traductor _formatosFecha;
        /// <summary>
        /// Lista de posibles formatos de fecha soportados
        /// </summary>  
        public override Traductor FormatosFecha
        {
            get
            {
                return _formatosFecha;
            }
        }
        #endregion
        #region propiedad NumeroFila
        /// <summary>
        /// Devuelve el dialecto para generar un numero único por cada fila
        /// </summary>  
        internal override string NumeroFila
        {
            get
            {
                return Lenguaje.RowNumber;
            }
        }
        #endregion
        #region propiedad separadorAlias
        /// <summary>
        /// Devuelve el separador de alias del motor de base de datos
        /// </summary>  
        public override string SeparadorAlias
        {
            get
            {
                return Lenguaje.Espacio;
            }
        }
        #endregion
        #region propiedad tipoTransitividad
        // campo que almacena la transitividad del motor
        private static string[][] _tipoTransitividad =
             {
                    new string[]{ string.Empty  , string.Empty  , string.Empty},
                    new string[]{ Lenguaje.MasEntreParentesis, string.Empty  , string.Empty},
                    new string[]{ string.Empty  , string.Empty  , Lenguaje.MasEntreParentesis}
                 };
        /// <summary>
        /// Devuelve la matriz para construir la transitividad segun el dialecto del motor
        /// </summary>  
        internal override string[][] TipoTransitividad
        {
            get
            {
                return _tipoTransitividad;
            }
        }
        #endregion
        #endregion
        #region Metodos

        #region componerSQLInsercion
        /// <summary>
        /// <c>componerSQLInsercion :</c> construye el string sql para insertar en la
        /// a la base de datos.
        /// </summary>
        /// <returns>retorna el string de conexion a la base de datos</returns>
        public override string ComponerSQLInsertar()
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
            return UltimaTransaccion = sb.ToString();
            //return ret;
        }
        #endregion
        public override string ComponerSQLActualizar()
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(Lenguaje.Update);
            sqlOrigen(sb);
            sb.Append(Lenguaje.Set);
            sqlListaCampoActualizacion(sb);
            sb.Append(Lenguaje.Espacio);
            SqlCriterios(sb);

            return UltimaTransaccion = sb.ToString();
        }

        public override string ComponerSQLEliminar()
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(Lenguaje.Delete);

            sb.Append(Lenguaje.From);
            sqlOrigen(sb);
            SqlCriterios(sb);
            //_ultimaTransaccion = sb.ToString();
            return UltimaTransaccion = sb.ToString();
        }

        public override void Exportar(bool recrear, Objeto cfg, Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;
            // crea las tablas

            foreach (Registro reg in catOrigen.Registros.Contenido)
            {
                if (recrear)
                {
                    instr.Agregar(new Dato()
                    {
                        Id = (++iId).ToString(),
                        Valor = "drop table if exists " + reg.Tabla + ";"
                    });

                }
            }
            foreach (Registro reg in catOrigen.Registros.Contenido)
            {
                /*                        
                                        instr.Agregar(new Dato()
                                        {
                                            Id = (++iId).ToString(),
                                            Valor = string.Concat("DROP TABLE IF EXISTS '", reg.Tabla, "';")
                                        });
                */

                Exportar(recrear, cfg, reg, instr);
               /*instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = Exportar(recrear, cfg, reg)
                });*/
            }

            // secuencias
            //Descriptor.ScriptSecuencias(cat, instr);
            //Descriptor.ScriptComentarios(cat, instr);
            Descriptor.ScriptInserciones(catOrigen, catDestino, instr);
            //Descriptor.ScriptRelaciones(cat, instr);
            //Descriptor.DescriptorDestino.ScriptIndices(cat, instr);
            //Descriptor.ScriptVistasDominio(cat, instr);
            //Descriptor.ScriptFunciones(cat, instr);
            //return esquemas;
        }

        public override string Exportar(bool recrear, Objeto cfg, Registro regOrigen, Recipiente<Dato> instr)
        {
            StringBuilder sb = new StringBuilder(300);



            sb.Append("create table ");
            //sb.Append("CREATE TABLE ");
            //sb.Append(reg.Tabla.Replace(".","_"));
            sb.Append(regOrigen.Tabla);
            sb.Append(" (");
            //sb.Append(" (");
            foreach (Campo cam in regOrigen.Campos)
            {
                sb.Append(Exportar(cfg, cam));
            }
            sb.Length -= 1;
            sb.Append(");");
            int iId = instr.Largo;

            if (recrear)
            {
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = "drop table if exists " + regOrigen.Tabla + ";"
                });

            }
            instr.Agregar(new Dato()
            {
                Id = (++iId).ToString(),
                Valor = sb.ToString()
            });
            return sb.ToString();
        }

        public override string Exportar(Objeto cfg, Campo cmOrigen)
        {
            StringBuilder sb = new StringBuilder(30);

            sb.Append(string.Concat(cmOrigen.Nombre, " "));
            int largoTexto = int.Parse(cmOrigen.Largo);
            if (largoTexto <= 0)
            {
                largoTexto = 10485760;
            }

            string val = string.Empty;
            switch (cmOrigen.Tipo)
            {
                case Tipo.Texto:

                    if (cmOrigen.Propiedades != null)
                    {
                        if (cmOrigen.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            sb.Append(" " + val + " ");
                        }
                        else
                        {
                            sb.Append(" varchar(" + largoTexto.ToString() + ")");
                        }
                    }
                    else
                    {
                        sb.Append(" varchar(" + largoTexto.ToString() + ")");
                    }
                    break;
                case Tipo.Numerico:
                    sb.Append(" numeric(" + largoTexto.ToString() + ")");
                    break;
                case Tipo.Fecha:
                    sb.Append(" datetime");
                    cmOrigen.Formato = "dd/MM/yyyy HH:mm:ss";
                    break;
            }
            if (cmOrigen.Clave)
            {
                //sb.Append(" constraint pk_" + cmOrigen.Tabla.Split('.')[1] + " primary key");
                sb.Append(" constraint pk_" + cmOrigen.Tabla.Substring(1, cmOrigen.Tabla.Length-2).Replace('.','_') + " primary key");
            }
            else
                if (cmOrigen.Existe("ValidarRequerido"))
            {
                if (cmOrigen.Unico)
                {
                    sb.Append(" unique ");
                }
                sb.Append(" not null");

            }
            sb.Append(",");

            return sb.ToString();
        }

        #region inicializarConexion
        /// <summary>
        /// <c>InicializarConexion : </c> inicializa la conextividad 
        /// con la base de datos
        /// </summary>
        internal override void InicializarConexion(object sender, EventArgs e)
        {
            base.InicializarConexion(null, null);
#if TRACE
            //Jen.Util.ToLog ("sqlite.txt", "InicializarConexion", Driver);
            _logger.Debug(string.Concat("InicializarConexion ", Driver));
#endif
            _conexion = new SQLiteConnection(Driver);
            _conexion.Open();
        }
        #endregion

        #region GetObjectData
        /// <summary>
        /// Devuelve las propiedades serializadas del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            _formatosFecha.GetObjectData(info, context);
        }
        #endregion

        #region representacionCampoEnSelect
        /// <summary>
        /// Construye la expresión sql para el campo en la clausula select
        /// </summary>
        /// <param name="campo">Campo a representar</param>
        /// <returns>Texto con la representación del campo</returns>
        internal override string RepresentacionCampoEnSelect(Campo campo)
        {
            // obtiene el Tipo del origen
            Tipo tipo = campo.Tipo;
            string campoNombre = campo.Nombre;
            string campoId = campo.Id;
            string campoFormato = campo.Formato;
            // valida si el campo en una expresion
            if (campo.ExpresionSql) return string.Concat(campoNombre, SeparadorAlias, campoId);
            // valida si el origen posee tabla
            string tabla = campo.Tabla;
            if (!string.IsNullOrEmpty(tabla))
            {
                tabla += Lenguaje.Punto;
            }
            // recuperacion por origen
            //string ret = string.Concat(tabla, campoNombre, separadorAlias, campoId);
            // recuperación conexion formato 
            if ((!string.IsNullOrEmpty(campoFormato)) && (tipo == Tipo.Fecha))
            {
                //return string.Concat(Lenguaje.Convert, Lenguaje.Varchar, 
                //    Lenguaje.ComaEspacio, tabla, campoNombre, 
                //    Lenguaje.ComaEspacio, FormatosFecha[campoFormato].Valor, 
                //    Lenguaje.CierreParentesis, SeparadorAlias, campoId);

                return string.Concat(Lenguaje.StrFTime,
                Lenguaje.AbreParentesis, Lenguaje.Comilla, campoFormato, Lenguaje.Comilla,
                Lenguaje.ComaEspacio, tabla, campoNombre,
                Lenguaje.CierreParentesis, SeparadorAlias, campoId);
            }
            // si no en strFecha retorna la recuperación por origen
            return string.Concat(tabla, campoNombre, SeparadorAlias, campoId);
        }
        #endregion
        #region representacionIdentificadorDeCampo
        /// <summary>
        /// <c>representacionIdentificadorDeCampo : </c> contruye la identificacion 
        /// del origen para las construcciones de sql como insert, delete o update
        /// </summary>
        /// <returns>retorna un string conexion la representacion del origen</returns>

        internal override string RepresentacionIdentificadorDeCampo(Campo campo)
        {
            // caso canonico
            return campo.Nombre;
        }

        #endregion

        #region representacionFecha
        /// <summary>
        /// Representa la expresion para manejar fecha en el motor de base de datos
        /// </summary>
        public override string RepresentacionFecha(Campo campo)
        {
            //string valor = campo.Valor;
            string valor = campo.Valor;

            System.DateTime oDate;// = System.DateTime.ParseExact(valor, campo.Formato, Util.Cultura);
            if (System.DateTime.TryParseExact(valor, campo.Formato, Util.Cultura, System.Globalization.DateTimeStyles.AssumeLocal, out oDate))
            {
                valor = oDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            //System.DateTime oDate = System.Convert.ToDateTime(valor);
            //valor = string.Concat(oDate.Year,"-", oDate.Month,"-", oDate.Day," ", oDate.Hour,":", oDate.Minute, ":",oDate.Second); 
            //valor = oDate.ToString("yyyy-MM-dd HH:mm:ss");
            //string formato = FormatosFecha[campo.Formato].Valor;
            //string val = string.Empty;
            /*return string.Concat(Lenguaje.StrFTime, Lenguaje.AbreParentesis, Lenguaje.Comilla, formato,
                 Lenguaje.Comilla, Lenguaje.ComaEspacio, Lenguaje.Comilla, valor, 
                 Lenguaje.Comilla, Lenguaje.CierreParentesis);
                 */
            return string.Concat(Lenguaje.Comilla, valor, Lenguaje.Comilla);
        }
        #endregion

        #region comprometer
        /// <summary>
        ///  compromete los cambios en la base de datos
        /// </summary>
        public override void Comprometer()
        {
            _transaccion.Commit();
            _transaccion = null;
        }
        #endregion
        #region deshacer
        /// <summary>
        ///  deshace los cambios en la base de datos
        /// </summary>
        public override void Deshacer()
        {
            // verifica si hubo transacción antes de deshacer
            _transaccion.Rollback();
            _transaccion = null;
        }
        #endregion
        #region iniciarTransaccion
        /// <summary>
        ///  indica al motor de base de datos que va a iniciar una transaccion
        /// </summary>
        public override void IniciarTransaccion()
        {
            ConectarBaseDatos();
            _transaccion = _conexion.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }
        #endregion
        #region SqlPag
        /// <summary>
        /// Compone el SQL para paginar el set de datos de la consulta con el inicio y fin
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        internal override string SqlPag(string inicio, string fin)
        {
            return " order by rowNumber) " + SeparadorAlias + " T where rowNumber > " + inicio + " and rowNumber <= (" + inicio + " + " + fin + ")";
        }
        #endregion
        #region SqlPsqlSeleccionPaginacionag
        /// <summary>
        /// Construye la clausula de seleccion de campos para la paginación
        /// </summary>
        /// <param name="sb"></param>
        internal override void sqlSeleccionPaginacion(System.Text.StringBuilder sb)
        {
            foreach (Campo campo in Contexto.Campos)
            {
                sb.Append(Lenguaje.ComillaDoble);
                sb.Append(campo.Id);
                sb.Append(Lenguaje.ComillaDoble);
                sb.Append(Lenguaje.ComaEspacio);
            }
            sb.Length -= Lenguaje.ComaEspacio.Length;
        }
        #endregion
        #region OperacionBD
        public override ILectorBD OperacionBD(params object[] par)
        {
            ConectarBaseDatos();
            string sql = (string)par[0];
            bool escalar = false;
            if (par.Length > 2)
            {
                escalar = (bool)par[2];
            }

#if TRACE
            //StringBuilder opDbParam = new StringBuilder(" ");
            //Jen.Util.ToLog ("sqlite.txt", "OperacionBD", sql);
            _logger.Debug(string.Concat("OperacionBD ", sql));

#endif

            SQLiteCommand comando = new SQLiteCommand(sql, _conexion);

            if (par.Length > 1)
            {
                object oParams = par[1];

                if (oParams is SQLiteParameter[])
                {
                    SQLiteParameter[] pgParams = (SQLiteParameter[])oParams;

                    foreach (SQLiteParameter p in pgParams)
                    {
#if TRACE
                        //opDbParam.Append(string.Concat(sp.Id, " = " , sp.Valor ));
                        // Jen.Util.ToLog ("sqlite.txt", "Parametros", string.Concat(sp.Id, "-", sp.Tipo.ToString(), "=", sp.Valor ));
#endif
                        comando.Parameters.Add(p);
                        if (p.Direction == System.Data.ParameterDirection.Output)
                        {
                            escalar = false;
                        }
                    }
                }
            }

            //inicializa transaccion 
            comando.Transaction = _transaccion;
            SQLiteDataReader reader = null;
            SQL = sql;
            LectorSQLite lp = null;

            if (_conexion.State == System.Data.ConnectionState.Open)
            {
#if TRACE
                try
                {
                    //Jen.Util.ToLog ("sqlite.txt", "ExecuteReader", "Antes de " + sql + opDbParam.ToString());
                    _logger.Debug(string.Concat("Prepare ", sql));
#endif
                    comando.CommandType = TipoOperacion;
                    comando.Prepare();
                    if (escalar)
                    {
                        //object o = comando.ExecuteScalar();
                        lp = new LectorSQLite(comando.ExecuteScalar());
                    }
                    else
                    {

                        reader = comando.ExecuteReader();
                        lp = new LectorSQLite(reader);
                    }

#if TRACE
                }
                catch (System.Exception e)
                {
                    _logger.Debug(string.Concat("Exception ", e.ToString()));
                    //Jen.Util.ToLog ("sqlite.txt", "Error", e.ToString());
                }
                //Jen.Util.ToLog ("sqlite.txt", "ExecuteReader", "Despues de " + sql + opDbParam.ToString());
#endif

#if RuntimeCache
                    if (Atributos.Respaldable(Atributo.datos))
                        Atributos.Agregar(Atributo.datos,
                            delegate ()
                            {
                                if (reader != null)
                                    if (!reader.IsClosed)
                                    {
                                        reader.Close();
                                        reader = null;
                                    }

                            }
                        );
#endif
                UltimaTransaccion = sql;
            }
            //FilasAfectadas = reader.RecordsAffected;
            //UltimaTransaccion = sql;

            return lp;
        }


        #endregion
        #endregion
    }
}