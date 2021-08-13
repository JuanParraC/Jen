// Autor : Juan Parra
// 3Soft

#if BaseDeDatoSQLServer
namespace Jen.RDBMS
{
    using System.Security.Permissions;
    using System.Text;
    using Jen.Json;
    using CommandType = System.Data.CommandType;
    using EventArgs = System.EventArgs;
    using IDbConnection = System.Data.IDbConnection;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using SqlCommand = System.Data.SqlClient.SqlCommand;
    using SqlConnection = System.Data.SqlClient.SqlConnection;
    using SqlDataReader = System.Data.SqlClient.SqlDataReader;
    using SqlDbType = System.Data.SqlDbType;
    using SqlParameter = System.Data.SqlClient.SqlParameter;
    using SqlTransacction = System.Data.SqlClient.SqlTransaction;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    public class LectorSQLServer : ILectorBD
    {
        SqlDataReader _lector;
        object _escalar;
        bool esEscalar = false;
        public LectorSQLServer(SqlDataReader lector)
        {

            _lector = lector;
        }
        public LectorSQLServer(object escalar)
        {

            _escalar = escalar;
            esEscalar = true;
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            return _lector.GetEnumerator();
        }

        public void Dispose()
        {
            _lector.Close();
        }
        public bool HasRows
        {
            get
            {
                return _lector.HasRows;
            }
        }
        public int FieldCount
        {
            get
            {
                return _lector.FieldCount;
            }
        }
        public void Close()
        {
            _lector.Close();
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
    /// Implementa el almacenamiento y recuperación sobre base de datos sqlserver
    /// </summary>

    [Serializable]
    public class SQLServer : BaseDatos
    {
        // objeto para estasblecer la conexion con el motor
        internal SqlConnection _conexion;
        // objeto para realizar transacciones
        internal SqlTransacction _transaccion;
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SQLServer()
            : base()
        {
            fromHint = " with (nolock) ";
            Id = Clase = Lenguaje.SQLServer;
            _formatosFecha = Constructor.crear<Traductor>(Lenguaje.FmtFechasSQL);
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected SQLServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            fromHint = " with (nolock) ";
            Id = Clase = Lenguaje.SQLServer;
            _formatosFecha = Constructor.Embriones[Lenguaje.Traductor].Germinar<Traductor>(info, context);
        }
        #region propiedades
        #region propiedad conexion
        /// <summary>
        /// Propiedad que retorna el objeto de conexion
        /// </summary>
        public override IDbConnection Conexion
        {
            get
            {
                return _conexion;
            }
        }
        public SqlConnection SQLConexion
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
        #region propiedad numeroFila
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
                return Lenguaje.As;
            }
        }
        #endregion
        #region propiedad tipoTransitividad
        // campo que almacena la transitividad del motor
        private static string[][] _tipoTransitividad =
        {
                    new string[]{ string.Empty  , string.Empty  , string.Empty},
                    new string[]{ Lenguaje.Por, string.Empty  , string.Empty},
                    new string[]{ string.Empty, Lenguaje.Por, string.Empty}
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
        #region metodos
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
        #region inicializarConexion
        /// <summary>
        /// Establece la conexion con la base de datos
        /// </summary>
        internal override void InicializarConexion(object sender, EventArgs e)
        {
            base.InicializarConexion(sender, e);
            _conexion = new SqlConnection(Driver);
            _conexion.Open();
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
        #region representacionCampoEnSelect()
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
            {//return string.Concat(Lenguaje.Convert, Lenguaje.Varchar, 
                //    Lenguaje.ComaEspacio, tabla, campoNombre, 
                //    Lenguaje.ComaEspacio, FormatosFecha[campoFormato].Valor, 
                //    Lenguaje.CierreParentesis, SeparadorAlias, campoId);

                return string.Concat(Lenguaje.Format,
                    Lenguaje.AbreParentesis, tabla, campoNombre,
                    Lenguaje.ComaEspacio, Lenguaje.Comilla, campoFormato, Lenguaje.Comilla,
                    Lenguaje.CierreParentesis, SeparadorAlias, campoId);
            }
            // si no en strFecha retorna la recuperación por origen
            return string.Concat(tabla, campoNombre, SeparadorAlias, campoId);
        }
        #endregion
        #region representacionSecuenciaDeCampo()
        /// <summary>
        /// Construye el select para obtener el valor siguiente de la secuencia asociada al campo
        /// </summary>
        /// <param name="campo">Campo destino de la secuencia</param>
        /// <returns>Select que genera el siguiente valor de la secuencia</returns>
        //internal override string RepresentacionSecuenciaDeCampo(Campo campo)
        //{
        //    return string.Concat(Lenguaje.Select,
        //        RepresentacionCampoEnSelect(campo),
        //        Lenguaje.From, campo.Tabla,
        //        Lenguaje.Where,
        //        BaseDatos.RepresentacionIdentificadorDeCampo(campo),
        //        Lenguaje.Igual, Lenguaje.ScopeIdentity);
        //}
        #endregion
        #region representacionFecha
        /// <summary>
        /// Representa la expresion para manejar fecha en el motor de base de datos
        /// </summary>
        public override string RepresentacionFecha(Campo campo)
        {
            string valor = campo.Valor;
            string formato = campo.Formato;
            return string.Concat(Lenguaje.Convert, Lenguaje.DateTime, Lenguaje.ComaEspacio,
                Lenguaje.Comilla, valor, Lenguaje.Comilla,
                Lenguaje.ComaEspacio, FormatosFecha[formato].Valor,
                Lenguaje.CierreParentesis);
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
            IConsultable consultable = Contexto;
            string orden = "order by rowNumber";
            if (consultable.Distinto)
                orden = string.Empty;
            return ") " + SeparadorAlias + " T where rowNumber > " + inicio + " and rowNumber <= (" + inicio + " + " + fin + ") " + orden;
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
                sb.Append(campo.Id);
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
            SqlCommand comando = new SqlCommand(sql, _conexion);

            if (par.Length > 1)
            {
                object oParams = par[1];
                if (oParams is Recipiente<SQLParam>)
                {
                    Recipiente<SQLParam> sqlParams = (Recipiente<SQLParam>)oParams;
                    //comando.CommandType = CommandType.StoredProcedure;
                    foreach (SQLParam sp in sqlParams)
                    {
                        comando.Parameters.Add(new SqlParameter(sp.Id, (SqlDbType)sp.Tipo));
                        comando.Parameters[sp.Id].Value = sp.Valor;
                    }
                }
            }
            // Inicializa la interface para las operaciones de base de datos
            comando.Transaction = _transaccion;

            SqlDataReader reader = null;
            UltimaTransaccion = string.Empty;
            SQL = sql;
            if (_conexion.State == System.Data.ConnectionState.Open)
            {
                reader = comando.ExecuteReader();
#if RuntimeCache
                        if (Atributos.Respaldable(Atributo.datos))
                            Atributos.Agregar(Atributo.datos,
                                delegate()
                                {
                                    if (reader != null)
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

            return new LectorSQLServer(reader);
        }

        #endregion
        public override void Exportar(bool recrear, Objeto cfg, Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            // incluye creacion
            Arreglo esquemas = Arreglo.Ref;


            catDestino.BaseDeDatos.Descriptor.SQLCreacionBase(catDestino, cfg, instr);
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
        #endregion
    }
}
#endif