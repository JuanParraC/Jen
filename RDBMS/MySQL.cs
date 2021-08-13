#if BaseDeDatoMySQL

namespace Jen
{
    using DataSet = System.Data.DataSet;
    using DataTable = System.Data.DataTable;
    using EventArgs = System.EventArgs;
    using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
    using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;
    using MySqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
    using MySqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
    using DbDataReader = System.Data.Common.DbDataReader;
    using Serializable = System.SerializableAttribute;

    /// <summary>
    /// <c>MySQL : </c> permite la persistencia base de datos MySQL
    /// </summary>
    [Serializable]
    public class MySQL : BaseDeDatos
    {
        internal MySqlConnection conexion;

        #region propiedades
        #region propiedad numeroFila
        /// <summary>
        /// <c>separadorAlias : </c> propiedad separadorAlias.
        /// </summary>  
        internal override string numeroFila
        {
            get
            {
                return Lenguaje.rowNumber;
            }
        }
        #endregion
                #region propiedad separadorAlias
                    /// <summary>
                    /// <c>separadorAlias : </c> propiedad separadorAlias.
                    /// </summary>  
                    internal override string separadorAlias
                    {
                        get
                        {
                             return Lenguaje._as_;
                        }
                    }
                #endregion

                 #region propiedad tipoTransitividad
                 /// <summary>
                 /// <c>tipoTransitividad : </c> propiedad tipoTransitividad.
                 /// </summary>  
                 private static string[][] _tipoTransitividad = 
                {
                    new string[]{ string.Empty, string.Empty, string.Empty},
                    new string[]{ string.Empty, string.Empty, string.Empty},
                    new string[]{ string.Empty, string.Empty, string.Empty}
                };
                 internal override string[][] tipoTransitividad
                 {
                     get
                     {
                         return _tipoTransitividad;
                     }
                 }
                 #endregion

        #endregion

        #region metodos

            #region actualizar
                /// <summary>
                /// Actualiza en la base datos
                /// </summary>
                public override void actualizar()
                {
                    conectarBaseDeDatos();
                    contexto.ejecutarEvento(Evento.antesDeActualizar);
                    MySqlCommand comando = new MySqlCommand(componerSQLActualizacion(), conexion);
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Close();
                }
            #endregion

            #region insertarse de datos
                /// <summary>
                /// Inserta en la base de datos
                /// </summary>
                public override void insertar()
                {
                    conectarBaseDeDatos();
                    contexto.ejecutarEvento(Evento.antesDeInsertar);
                    MySqlCommand comando = new MySqlCommand(componerSQLInsercion(), conexion);
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Close();
                    contexto.ejecutarEvento(Evento.despuesDeInsertar);
                }
            #endregion

            #region eliminar
                /// <summary>
                /// Elimina en la base de datos
                /// </summary>
                public override void eliminar()
                {
                    conectarBaseDeDatos();
                    MySqlCommand comando = new MySqlCommand(componerSQLEliminar(), conexion);
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Close();
                }
            #endregion
            #region cerrarConeccion
                internal override void cerrarConeccion()
                {
                    conexion.Close();
                }
            #endregion
            #region consultar
                /// <summary>
                /// Ejecuta la consulta en la base de datos
                /// </summary>
                /// <param name="sql"></param>
                /// <returns></returns>
                public override DbDataReader consultar(string sql)
                {
                    DbDataReader datos;
                    conectarBaseDeDatos();
                    MySqlCommand comando = new MySqlCommand(sql, conexion);
                    MySqlDataAdapter adaptador = new MySqlDataAdapter(sql, conexion);
                    datos = comando.ExecuteReader();
                    atributos.Add(claveAtributo.datos,
                        delegate()
                        {
                            if (datos != null)
                            {
                                datos.Close();
                                datos = null;
                            }
                        }
                    );
                    return datos;
                }
            #endregion

            #region inicializarConexion
                /// <summary>
                /// <c>inicializarConexion : </c> inicializa la conextividad 
                /// con la base de datos
                /// </summary>
                internal override void inicializarConexion(object sender, EventArgs e)
                {
                    base.inicializarConexion(sender, e);
                    conexion = new MySqlConnection(componerCadenaDeConexion());
                    conexion.Open();
                }

            #endregion

            #region representacionCampoEnSelect
                internal override string representacionCampoEnSelect(Campo campo)
                {
                    // obtiene el Tipo del origen
                    Tipo tipo = campo.tipo;
                    string campoNombre = campo.nombre;
                    string campoId = campo.id;
                    string campoFormato = campo.formato;


                    // valida si el campo en una expresion
                    if (campo.expresionSql) return string.Concat(campoNombre, separadorAlias, campoId);
                    
                    // valida si el origen posee tabla
                    string tabla = campo.tabla;

                    if (!string.IsNullOrEmpty(tabla)) tabla += Lenguaje.punto;
                    
                    // recuperacion por origen
                    string ret = string.Concat(tabla, campoNombre, separadorAlias, campoId);

                    // recuperación conexion formato 
                    if ((!string.IsNullOrEmpty(campoFormato)) && (tipo == Tipo.fecha))
                        ret = string.Concat(Lenguaje.mysql_case, tabla, campoNombre, 
                                            Lenguaje.mysql_when, Lenguaje.cero, Lenguaje.mysql_then,
                                            Lenguaje.dobleComilla, Lenguaje.mysql_else,
                                            Lenguaje.date_format, tabla, campoNombre, 
                                            Lenguaje.comaEspacio,Lenguaje.comilla,
                                            formatosFecha[campoFormato].valor, 
                                            Lenguaje.comilla, Lenguaje.cierreParentesis,
                                            Lenguaje.mysql_end,  separadorAlias, campoId);

                    // si en igual a formato strFecha
                    if (tipo == Tipo.fecha) return ret;

                    // si no en strFecha retorna la recuperación por origen
                    return string.Concat(tabla, campoNombre, separadorAlias, campoId);
                }
            #endregion

            #region representacionSecuenciaDeCampo()
                internal override string representacionSecuenciaDeCampo(Campo campo)
                {
                    
                    return string.Empty;

                }
            #endregion

            #region representacionFecha
                /// <summary>
                /// <c>representacionFecha : </c> retorna la conversión de fecha en el rdbms
                /// </summary>
                internal override string representacionFecha(string valor, string formato)
                {
                    return string.Concat(Lenguaje.str_to_date, Lenguaje.comilla,
                                         valor, Lenguaje.comilla, Lenguaje.comaEspacio, Lenguaje.comilla,
                                         formatosFecha[formato].valor, Lenguaje.comilla, 
                                         Lenguaje.cierreParentesis);
                }
            #endregion

            #region transaccion
                /// <summary>
                /// Ejecuta una transacción en la base de datos
                /// </summary>
                /// <param name="sql"></param>
                public override void transaccion(string sql)
                {
                    conectarBaseDeDatos();
                    MySqlCommand comando = new MySqlCommand(sql, conexion);
                    MySqlDataReader reader = comando.ExecuteReader();
                    reader.Close();
                }
            #endregion

        #endregion
    }
}
#endif