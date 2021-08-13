// Autor : Juan Parra
// 3Soft

#if BaseDeDatoPostgreSQL
namespace Jen.RDBMS
{
    //using System;
    using System.Text;
    using Jen.Extensiones;
    using Jen.Json;
    using EventArgs = System.EventArgs;
    using IDbConnection = System.Data.IDbConnection;
    using NpgsqlDataReader = Npgsql.NpgsqlDataReader;
    using PGCommand = Npgsql.NpgsqlCommand;
    using PGConnection = Npgsql.NpgsqlConnection;
    using PGParameter = Npgsql.NpgsqlParameter;
    using PGTransaction = Npgsql.NpgsqlTransaction;
    using SecurityAction = System.Security.Permissions.SecurityAction;
    using SecurityPermissionAttribute = System.Security.Permissions.SecurityPermissionAttribute;
    using SecurityPermissionFlag = System.Security.Permissions.SecurityPermissionFlag;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#if LOGGER
    using NLog;
#endif

    public class LectorPostgreSQL : ILectorBD
    {
        NpgsqlDataReader _lector;
        object _escalar;
        bool esEscalar = false;

        [System.CLSCompliant(false)]
        public LectorPostgreSQL(NpgsqlDataReader lector)
        {
            _lector = lector;

        }
        public LectorPostgreSQL(object escalar)
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
    public class PostgreSQL : BaseDatos
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif          
        private PGConnection _conexion;
        private PGTransaction _transaccion;
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public PostgreSQL()
            : base()
        {
            Id = Clase = Lenguaje.PostgreSQL;
            _formatosFecha = Constructor.crear<Traductor>(Lenguaje.FmtFechasPGSQL);
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected PostgreSQL(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.PostgreSQL;
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
        #region inicializarConexion
        /// <summary>
        /// <c>InicializarConexion : </c> inicializa la conextividad 
        /// con la base de datos
        /// </summary>
        internal override void InicializarConexion(object sender, EventArgs e)
        {
            base.InicializarConexion(null, null);
#if TRACE
            //Jen.Util.ToLog ("pg.txt", "InicializarConexion", Driver);
            _logger.Debug(string.Concat("InicializarConexion ", Driver));
#endif
            _conexion = new PGConnection(Driver);
            _conexion.Open();
        }
        #endregion

        public override void Exportar(bool recrear, Objeto cfg, Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            // incluye creacion
            Arreglo esquemas = Arreglo.Ref;


            catDestino.BaseDeDatos.Descriptor.SQLCreacionBase(catDestino, cfg,  instr);
            int iId = instr.Largo;
            // crea las tablas
            foreach (Registro reg in catDestino.Registros)
            {
                catDestino.BaseDeDatos.Exportar(recrear, cfg, reg, instr);

            }
            // secuencias

            //catDestino.BaseDeDatos.Descriptor.ScriptSecuencias(Descriptor.UPDTabDescSEQ(catOrigen), catDestino, instr);

            //catDestino.BaseDeDatos.Descriptor.ScriptComentarios(catOrigen, catDestino, instr);

            Descriptor.ScriptInserciones(catOrigen, catDestino, instr);

            //Descriptor.ScriptRelaciones(catOrigen, catDestino, instr);  

            catDestino.BaseDeDatos.Descriptor.ScriptIndices(catDestino, instr);

            //Descriptor.ScriptVistasDominio(catOrigen, catDestino, instr);

            //Descriptor.ScriptFunciones(catOrigen, catDestino, instr);
            //return esquemas;
        }
        public override string Exportar(bool recrear, Objeto cfg, Registro regOrigen, Recipiente<Dato> instr)
        {
            StringBuilder sb = new StringBuilder(300);
            sb.Append("create table ");
            sb.Append(regOrigen.Tabla);
            sb.Append("(");
            foreach (Campo cam in regOrigen.Campos)
            {
                sb.Append(Exportar(cfg, cam));
            }
            sb.Length -= 1;
            sb.Append(");");

            int iId = instr.Largo;
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
                largoTexto = 10485760;

            string val = string.Empty;
            switch (cmOrigen.Tipo)
            {
                case Tipo.Texto:

                    if (cmOrigen.Propiedades != null)
                    {
                        if (cmOrigen.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            sb.Append(" " + val.ToLower() + " ");
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
                    if (cmOrigen.Propiedades != null)
                    {
                        if (cmOrigen.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            sb.Append(" " + val.ToLower() + " ");
                        }
                    }
                    else
                    {
                        sb.Append(" date");
                    }
                    break;
            }
            if (cmOrigen.Clave)
            {
                sb.Append(" constraint pk_" + cmOrigen.Tabla.Split('.')[1] + " primary key");
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
            string campoFormato = campo.Formato;
            // valida si el campo en una expresion
            if (campo.ExpresionSql)
            {
                return string.Concat(campo.Nombre,
                      SeparadorAlias,
                      Lenguaje.ComillaDoble,
                      campo.Id, Lenguaje.ComillaDoble);
            }
            // valida si el origen posee tabla
            string tabla = campo.Tabla;
            if (!string.IsNullOrEmpty(tabla)) tabla += Lenguaje.Punto;
            // recuperacion por origen
            string ret = string.Concat(tabla,
                campo.Nombre,
                SeparadorAlias,
                Lenguaje.ComillaDoble,
                campo.Id, Lenguaje.ComillaDoble);
            // recuperación conexion formato 
            if ((!string.IsNullOrEmpty(campoFormato)) && (tipo == Tipo.Fecha))
            {
                ret = string.Concat(Lenguaje.ToChar, tabla, campo.Nombre, Lenguaje.ComaEspacio,
                      Lenguaje.Comilla, FormatosFecha[campoFormato].Valor,
                      Lenguaje.Comilla, Lenguaje.CierreParentesis,
                      SeparadorAlias, Lenguaje.ComillaDoble, campo.Id, Lenguaje.ComillaDoble);
            }
            // si en igual a formato strFecha
            if (tipo == Tipo.Fecha)
            {
                return ret;
            }
            // si no en strFecha retorna la recuperación por origen
            return string.Concat(tabla, campo.Nombre, SeparadorAlias, Lenguaje.ComillaDoble, campo.Id, Lenguaje.ComillaDoble);
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
            string valor = campo.Valor;
            //System.DateTime oDate = System.Convert.ToDateTime(valor);
            //valor = string.Concat(oDate.Year,"-", oDate.Month,"-", oDate.Day," ", oDate.Hour,":", oDate.Minute, ":",oDate.Second); 
            //valor = oDate.ToString("yyyy-MM-dd HH:mm:ss");
            string formato = campo.Formato;
            string val = string.Empty;
            if (campo.Propiedades != null)
            {
                if (campo.Propiedades.TryGetValue("SubTipo", out val))
                {
                    return string.Concat(Lenguaje.ToTimestamp, Lenguaje.Comilla, valor, Lenguaje.Comilla,
                    Lenguaje.ComaEspacio, Lenguaje.Comilla, FormatosFecha[formato].Valor,
                    Lenguaje.Comilla, Lenguaje.CierreParentesis);
                }
            }
            return string.Concat(Lenguaje.ToDate, Lenguaje.Comilla, valor, Lenguaje.Comilla,
            Lenguaje.ComaEspacio, Lenguaje.Comilla, FormatosFecha[formato].Valor,
            Lenguaje.Comilla, Lenguaje.CierreParentesis);
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
            _logger.Debug(string.Concat("OperacionBD ", sql));
#endif
            PGCommand comando = new PGCommand(sql, _conexion);

            if (par.Length > 1)
            {
                object oParams = par[1];

                if (oParams is PGParameter[])
                {
                    PGParameter[] pgParams = (PGParameter[])oParams;

                    foreach (PGParameter p in pgParams)
                    {
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
            NpgsqlDataReader reader = null;
            SQL = sql;
            LectorPostgreSQL lp = null;

            if (_conexion.State == System.Data.ConnectionState.Open)
            {
#if Try
                try
                {
                    //Jen.Util.ToLog ("pg.txt", "ExecuteReader", "Antes de " + sql + opDbParam.ToString());_logger.Debug(string.Concat("OperacionBD ", sql))
                    _logger.Debug(string.Concat("Prepare ", TipoOperacion.ToString(), " ", sql));
#endif
                comando.CommandType = TipoOperacion;
#if Try
                    try
                    {
#endif
                comando.Prepare();
#if Try
                    }
                    catch (System.Exception e)
                    {
                        System.Console.WriteLine(e.ToString());
                    }
#endif
                if (escalar)
                {

                        /*try
                        {{Npgsql.PostgresException (0x80004005): 42601: syntax error at or near ".174"   at Npgsql.NpgsqlConnector+<>c__DisplayClass161_0.<ReadMessage>g__ReadMessageLong|0 (Npgsql.DataRowLoadingMode dataRowLoadingMode2, System.Boolean readingNotifications2, System.Boole…}
                            object o = comando.ExecuteScalar();
                        }
                        catch (System.Exception e)
                        {
                            System.Console.WriteLine(e.ToString());
                        }*/
                        object o = comando.ExecuteScalar();
                    lp = new LectorPostgreSQL(o);
                }
                else
                {

#if TRACE
                    _logger.Debug(string.Concat("Antes de ExecuteReader"));
#endif
#if Try
                    try
                    {
#endif

                    reader = comando.ExecuteReader();
#if TRACE
                    _logger.Debug(string.Concat("Despues de ExecuteReader"));
#endif
                    lp = new LectorPostgreSQL(reader);
#if TRACE
                    _logger.Debug(string.Concat("Despues de LectorPostgreSQL"));
#endif

#if Try
                        }
                        catch (System.Exception e)
                        {
                            //System.Console.WriteLine(e.ToString());
                            _logger.Error(string.Concat("Exception "), e.ToString());
                        }
#endif
                }

#if Try
            }
                catch (System.Exception e)
                {
                    //Jen.Util.ToLog ("pg.txt", "Error", e.ToString());
                    _logger.Error(string.Concat("Exception "), e.ToString());
                }
                //Jen.Util.ToLog ("pg.txt", "ExecuteReader", "Despues de " + sql + opDbParam.ToString());
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
#endif