// Autor : Juan Parra
// 3Soft
#if BaseDeDatoOracle
namespace Jen.RDBMS
{
    using System.Data;
    using System.Security.Permissions;
    using System.Text;
    using Jen.Extensiones;
    using Jen.Json;
    using EventArgs = System.EventArgs;
    using IDbConnection = System.Data.IDbConnection;
    using OracleCommand = Oracle.DataAccess.Client.OracleCommand;
    using OracleConnection = Oracle.DataAccess.Client.OracleConnection;
    using OracleDataReader = Oracle.DataAccess.Client.OracleDataReader;
    using OracleTransaction = Oracle.DataAccess.Client.OracleTransaction;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using SqlParameter = System.Data.SqlClient.SqlParameter;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    public class LectorORACLE : ILectorBD
    {
        OracleDataReader _lector;
        bool esEscalar = false;
        object _escalar;
        public LectorORACLE(OracleDataReader lector)
        {
            _lector = lector;
        }
        public LectorORACLE(object escalar)
        {
            _escalar = escalar;
            esEscalar = true;

        }
        public void Dispose()
        {
            _lector.Close();
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
        public bool IsScalar
        {
            get { return esEscalar; }

        }
        public object GetScalar()
        {
            return _escalar;
        }
    }

    /// <summary>
    /// Implementa el almacenamiento y recuperación sobre base de datos Oracle
    /// </summary>
    [Serializable]
    public class ORACLE : BaseDatos
    {
        // objeto para establecer la conexion con el motor
        private OracleConnection _conexion;
        // objeto para realizar transacciones
        private OracleTransaction _transaccion;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public ORACLE()
            : base()
        {
            Id = Clase = Lenguaje.Oracle;
            _formatosFecha = Constructor.crear<Traductor>(Lenguaje.FmtFechasORA);
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected ORACLE(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Oracle;
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
        /// Establece la conexion con la base de datos
        /// </summary>
        internal override void InicializarConexion(object sender, EventArgs e)
        {
            //OracleConnection
            base.InicializarConexion(null, null);
            _conexion = new OracleConnection(Driver);
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
            string campoFormato = campo.Formato;
            // valida si el campo en una expresion
            if (campo.ExpresionSql)
                return string.Concat(campo.Nombre,
                                     SeparadorAlias,
                                     Lenguaje.ComillaDoble,
                                     campo.Id, Lenguaje.ComillaDoble);
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
                ret = string.Concat(Lenguaje.ToChar, tabla, campo.Nombre, Lenguaje.ComaEspacio,
                                    Lenguaje.Comilla, FormatosFecha[campoFormato].Valor,
                                    Lenguaje.Comilla, Lenguaje.CierreParentesis,
                                    SeparadorAlias, Lenguaje.ComillaDoble, campo.Id, Lenguaje.ComillaDoble);
            // si en igual a formato strFecha
            if (tipo == Tipo.Fecha) return ret;
            // si no en strFecha retorna la recuperación por origen
            return string.Concat(tabla, campo.Nombre, SeparadorAlias, Lenguaje.ComillaDoble, campo.Id, Lenguaje.ComillaDoble);
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
        //        campo.Propiedades[Lenguaje.SEQ], Lenguaje.NextVal, Lenguaje.Espacio, 
        //        Lenguaje.ComillaDoble, campo.Id, Lenguaje.ComillaDoble,
        //        Lenguaje.From, Lenguaje.Dual);
        //}
        #endregion
        #region representacionFecha
        /// <summary>
        /// Representa la expresion para manejar fecha en el motor de base de datos
        /// </summary>
        public override string RepresentacionFecha(Campo campo)
        {
            string formato = campo.Formato;
            string valor = campo.Valor;
            return string.Concat(Lenguaje.ToDate, Lenguaje.Comilla, valor, Lenguaje.Comilla,
                 Lenguaje.ComaEspacio, Lenguaje.Comilla, FormatosFecha[formato].Valor,
                 Lenguaje.Comilla, Lenguaje.CierreParentesis);
        }
        #endregion
        #region OperacionBD
        /// <summary>
        /// Permite realizar una operacion en la base de datos
        /// </summary>
        /// <param name="sql"></param>
        public override ILectorBD OperacionBD(params object[] par)

        {
            ConectarBaseDatos();
            string sql = (string)par[0];
            OracleCommand comando = new OracleCommand(sql, _conexion);
            if (par.Length > 1)
            {
                object oParams = par[1];
                if (oParams is Recipiente<SQLParam>)
                {
                    Recipiente<SQLParam> sqlParams = (Recipiente<SQLParam>)oParams;
                    comando.CommandType = CommandType.StoredProcedure;
                    foreach (SQLParam sp in sqlParams)
                    {
                        comando.Parameters.Add(new SqlParameter(sp.Id, (SqlDbType)sp.Tipo));
                        comando.Parameters[sp.Id].Value = sp.Valor;
                    }

                }
            }
            // inicializa transaccion
            comando.Transaction = _transaccion;
            OracleDataReader reader = null;
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
            return new LectorORACLE(reader);
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
        ///  Indica al motor de base de datos que va a iniciar una transaccion
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

        public override void Exportar(bool recrear, Objeto cfg, Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            // incluye creacion
            Arreglo esquemas = Arreglo.Ref;
            int iId = instr.Largo;
            if (cfg.ContainsKey("BD"))
            {
                Objeto bd = (Objeto)cfg["BD"];
                //  /var/lib/postgresql/9.4/main/pg_tblspc/
                //  creacion de base de datos
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = string.Concat("--create tablespace ts_", catOrigen.Id, " owner us_", catOrigen.Id, " location '", bd["Ubicacion"], catOrigen.Id, "' size 3m autoextend on;")
                });
                // creacion de la base de datos
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = string.Concat("--create database db_", catOrigen.Id, " OWNER US_", catOrigen.Id, " TABLESPACE ", "TS_", catOrigen.Id, ";")
                });

                // reprocesa los esquemas
                Recipiente<Semilla> rs = new Recipiente<Semilla>();
                foreach (Registro reg in catOrigen.Registros)
                {
                    string[] ids = reg.Tabla.ArrIDs();
                    if (!rs.Existe(ids[0]))
                    {
                        rs.Agregar(new Semilla() { Id = ids[0] });
                    }
                }
                //esquemas = new Arreglo(rs.Largo);
                foreach (Semilla s in rs)
                {
                    //esquemas.Add(s.Id);
                    instr.Agregar(new Dato()
                    {
                        Id = (++iId).ToString(),
                        Valor = string.Concat("drop schema if exists ", s.Id, " cascade ", ";")
                    });

                    instr.Agregar(new Dato()
                    {
                        Id = (++iId).ToString(),
                        Valor = string.Concat("create schema ", s.Id, " authorization US_", catOrigen.Id, ";")
                    });
                }

            }
           /*if (cat.BaseDeDatos.Clase.Equals(Lenguaje.PostgreSQL))
            {
                foreach (Registro reg in cat.Registros)
                {
                    reg.Tabla = string.Concat(cat.Id, ".", reg.Tabla.Replace('.', '_'));
                    foreach (Campo cm in reg.Campos)
                    {
                        cm.Tabla = reg.Tabla;
                    }

                }
            }*/
            // crea las tablas
            foreach (Registro reg in catOrigen.Registros)
            {
                Exportar(recrear, cfg, reg, instr);
                /*instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = Exportar(recrear, cfg, reg, instr)
                });*/
            }
            // secuencias
            /*Descriptor.ScriptSecuencias(catOrigen, instr);
            Descriptor.ScriptComentarios(catOrigen, instr);
            Descriptor.ScriptInserciones(catOrigen, instr, bdDestino);
            Descriptor.ScriptRelaciones(catOrigen, instr);
            Descriptor.ScriptIndices(catOrigen, instr);
            Descriptor.ScriptVistasDominio(catOrigen, instr);
            Descriptor.ScriptFunciones(catOrigen, instr);*/
            //return esquemas;
        }
        public override string Exportar(bool recrear, Objeto cfg, Registro regOrigen, Recipiente<Dato> instr)
        {
            StringBuilder sb = new StringBuilder(300);
           //sb.Append(string.Concat("begin", System.Environment.NewLine));
            sb.Append(string.Concat("drop table ", regOrigen.Tabla, " cascade constraints;", System.Environment.NewLine));
            /*sb.Append(string.Concat("exception", System.Environment.NewLine));
            sb.Append(string.Concat("when others then", System.Environment.NewLine));
            sb.Append(string.Concat("if sqlcode != -942 then", System.Environment.NewLine));
            sb.Append(string.Concat("raise;", System.Environment.NewLine));
            sb.Append(string.Concat("end if;", System.Environment.NewLine));
            sb.Append(string.Concat("end if;", System.Environment.NewLine));*/

            sb.Append("create table ");
            sb.Append(regOrigen.Tabla);
            sb.Append("(");
            string clave = string.Empty;
            foreach (Campo cam in regOrigen.Campos)
            {
                sb.Append(Exportar(cfg, cam));
                if (cam.Clave)
                {
                    clave = " primary key(" + cam.Nombre + ")";
                }
            }

            //sb.Length -= 1;
            sb.Append(clave);
            sb.Append(");");
            int iId = instr.Largo;
            instr.Agregar(new Dato()
            {
                Id = (++iId).ToString(),
                Valor = sb.ToString()
            });
            instr.Agregar(new Dato()
            {
                Id = (++iId).ToString(),
                Valor = string.Concat("grant select, insert, delete, update on ", regOrigen.Tabla, " to ", regOrigen.Padre.Id, "_intra;")
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
                largoTexto = 4000;
            }else if(largoTexto > 4000)
            {
                largoTexto = 4000;
            }
            string val = string.Empty;
            switch (cmOrigen.Tipo)
            {
                case Tipo.Texto:

                    if (cmOrigen.Propiedades != null)
                    {
                        if (cmOrigen.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            //sb.Append(" " + val.ToLower() + " ");
                            sb.Append(" varchar2(4000)");
                        }
                        else
                        {
                            sb.Append(" varchar2(" + largoTexto.ToString() + ")");
                        }
                    }
                    else
                    {
                        sb.Append(" varchar2(" + largoTexto.ToString() + ")");
                    }
                    break;
                case Tipo.Numerico:
                    sb.Append(" number(" + largoTexto.ToString() + ")");
                    break;
                case Tipo.Fecha:
                    if (cmOrigen.Propiedades != null)
                    {
                        if (cmOrigen.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            sb.Append(" " + val.ToLower() + " ");
                        }
                        break;
                    }
                    sb.Append(" date");
                    break;
            }
            /*
            if (cmOrigen.Clave)
            {
                sb.Append(" primary key(" + cmOrigen.Nombre + ")");
            }
            else 
            */
            if (cmOrigen.Existe("ValidarRequerido"))
            {
               /*if (cmOrigen.Unico)
                {
                    sb.Append(" unique ");
                }*/
                sb.Append(" not null");

            }
            sb.Append(",");

            return sb.ToString();
        }

        #endregion
    }
}
#endif