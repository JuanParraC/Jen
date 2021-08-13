// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Web;
    using DbDataReader = System.Data.Common.DbDataReader;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
#if LOGGER
    using NLog;
#endif

    /// <summary>
    /// <c>Consultor : </c> objeto para realizar consultas relacionales
    /// </summary>
    [Serializable]
    public class Consultor : Eventos<Consultor>, IConsultor
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif  
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Consultor()
            : base()
        {
            Id = Lenguaje.Consultor;
            IniConsultor();

        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Consultor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniConsultor();
        }
        protected override Consultor Ambito()
        {
            return this;
        }
        public Consultor(string sql) : this()
        {
            _sql = sql;
        }
        void IniConsultor()
        {
            Clase = Lenguaje.Consultor;
            // establece el estado de la semilla
            Estado = Estado.Instancia | Estado.Inicializado;
            // setea las propiedades para determinar el articulo del objeto
            Genero = Genero.Masculino;
            Numero = Numero.Singular;
            _campos = new Campos();
            _campos.Padre = this;
        }

        #region campos
        // puntero a la consulta
        internal ILectorBD datos;
        internal bool hayDatos;
        #endregion

        #region propiedades
        #region baseDeDatos
        //declara el origen _baseDeDatos privado para la propiedad
        private BaseDatos _baseDeDatos;

        /// <summary>
        /// <c>requerido : </c> propiedad requerido.
        /// </summary>  
        public BaseDatos BaseDeDatos
        {
            get
            {
                //setea el origen del baseDeDatos
                _baseDeDatos.Contexto = this;
                return _baseDeDatos;
            }
            set { _baseDeDatos = value; }
        }
        #endregion

        #region campos
        private Campos _campos;
        /// <summary>
        /// Campos de salida de la consulta
        /// </summary>
        public Campos Campos
        {
            get { return _campos; }
            set
            {
                _campos = value;
            }
        }
        #endregion

        #region criterios
        private Recipiente<Campo> _criterios;
        /// <summary>
        /// criterios de la consulta
        /// </summary>
        public Recipiente<Campo> Criterios
        {
            get { return _criterios; }
            set { _criterios = value; }
        }
        #endregion

        #region contador
        internal Consultor _contador;
        /// <summary>
        /// Numero de registros de la consulta
        /// </summary>
        public Consultor Contador
        {
            get
            {
                return _contador;
            }
            set
            {
                _contador = value;
            }
        }
        #endregion

        #region consultado
        private IOrigenes _consultado;
        /// <summary>
        /// Objeto consultado por el consultor
        /// </summary>
        public IOrigenes Consultado
        {
            get
            {
                return _consultado;
            }
            set
            {
                _consultado = value;
                if (value.BaseDeDatos != null)
                {
                    BaseDeDatos = value.BaseDeDatos;
                }
                if (value.Requerimiento != null)
                {
                    Requerimiento = value.Requerimiento;
                }
            }
        }
        #endregion

        #region distinto
        private bool _distinto = false;
        /// <summary>
        /// Todos distintos
        /// </summary>
        public bool Distinto
        {
            get { return _distinto; }
            set
            {
                _distinto = value;
            }

        }
        #endregion

        #region ordenadores
        private Recipiente<Campo> _ordenadores;
        /// <summary>
        /// Campos ordenadores de la consulta
        /// </summary>
        public Recipiente<Campo> Ordenadores
        {
            get { return _ordenadores; }
            set { _ordenadores = value; }
        }
        #endregion

        #region requerimiento
        //declara el origen _requerimiento privado para la propiedad
        private Requerimiento _requerimiento;

        /// <summary>
        /// <c>requerimiento : </c> propiedad requerimiento.
        /// </summary>  
        public Requerimiento Requerimiento
        {
            get { return _requerimiento; }
            set
            {
                _requerimiento = value;
            }

        }
        #endregion

        #region propiedad sql
        //declara el origen _sql privado para la propiedad
        private string _sql = string.Empty;

        /// <summary>
        /// <c>sql : </c> propiedad sql.
        /// </summary>  
        public string SQL
        {
            get { return _sql; }
            set { _sql = value; }

        }
        #endregion

        #region totalizadores
        private Recipiente<Campo> _totalizadores;
        /// <summary>
        /// Totalizadores de la consulta
        /// </summary>
        public Recipiente<Campo> Totalizadores
        {
            get { return _totalizadores; }
            set { _totalizadores = value; }
        }
        #endregion

        #region transitividad
        /// <summary>
        /// <c>transitividad : </c> representa las _transitividad para acceder un origen
        /// </summary>
        Recipiente<Relacion> IOrigenes.Transitividad
        {
            get { return Consultado.Transitividad; }
        }

        #endregion

        #endregion

        #region metodos

        #region iterador
        /// <summary>
        /// enumerador de campos 
        /// </summary>
        /// <returns></returns>
        public new System.Collections.Generic.IEnumerator<Campos> GetEnumerator()
        {
            if (hayDatos)
            {
                bool b = datos.Read();
                while (b)
                {
                    yield return _campos.Recuperar(datos);
                    b = datos.Read();
                }
            }
        }
        #endregion

        #region MesclarContenido
        /// <summary>
        /// traspasa los valores del consultor a la plantilla
        /// </summary>
        /// <param name="plantilla"></param>
        public override void MesclarContenido(Variables variables)
        {
            base.MesclarContenido(variables);
            foreach (Campos cmp in this)
            {
                cmp.MesclarContenido(variables);
            }

        }
        public override void EnlazarGrafica(Variables variables, HttpRequest request = null)
        {
            Campos.EnlazarGrafica(variables, request);
            base.EnlazarGrafica(variables, request);
        }

        #endregion

        #region origenes
        /// <summary>
        /// origenes de la consulta
        /// </summary>
        /// <param name="campos">set de origenes de acuerdo a los campos especificados</param>
        /// <returns></returns>
        public Recipiente<Registro> Origenes(params Recipiente<Campo>[] campos)
        {
            return Consultado.Origenes(
                _campos,
                _criterios,
                _ordenadores,
                _totalizadores);
        }
        #endregion

        #region recuperar
        /// <summary>
        /// recupera la consulta de la base de datos
        /// </summary>
        /// <returns></returns>
        public bool Recuperar()
        {
            //si el consultor en personalizado
            if (!string.IsNullOrEmpty(SQL))
            {
                //_logger.Debug(string.Concat("antes de BaseDeDatos.OperacionBD ", SQL));
                datos = BaseDeDatos.OperacionBD(SQL);
            }
            else
            {
                //_logger.Debug(string.Concat("antes de BaseDeDatos.Consultar ", SQL));
                datos = BaseDeDatos.Consultar();
            }

            // verificar el reader 
            hayDatos = false;
            if (BaseDeDatos.Conexion.State == System.Data.ConnectionState.Open)
            {
                hayDatos = datos.HasRows;
                if (hayDatos)
                {
                    Ejecutar(Evento.DespuesDeRecuperar);
                }
            }
            return hayDatos;
        }
        public bool Recuperar(object parametros)
        {
            //si el consultor en personalizado
            if (!string.IsNullOrEmpty(SQL))
            {
                datos = BaseDeDatos.OperacionBD(SQL, parametros);
            }
            else
            {
                datos = BaseDeDatos.Consultar();
            }
            // verificar el reader 
            hayDatos = false;
            if (BaseDeDatos.Conexion.State == System.Data.ConnectionState.Open)
            {
                hayDatos = datos.HasRows;
                if (hayDatos)
                {
                    Ejecutar(Evento.DespuesDeRecuperar);
                }
            }
            return hayDatos;
        }
        public bool Recuperar(Recipiente<SQLParam> parametros)
        {
            //si el consultor en personalizado
            if (!string.IsNullOrEmpty(SQL))
            {
                datos = BaseDeDatos.OperacionBD(SQL, parametros);
            }
            else
            {
                datos = BaseDeDatos.Consultar();
            }
            // verificar el reader 
            hayDatos = false;
            if (BaseDeDatos.Conexion.State == System.Data.ConnectionState.Open)
            {
                hayDatos = datos.HasRows;
                if (hayDatos)
                {
                    Ejecutar(Evento.DespuesDeRecuperar);
                }
            }
            return hayDatos;
        }
        #endregion

        #endregion


    }

}
