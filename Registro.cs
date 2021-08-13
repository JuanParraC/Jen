// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using System.Web;
    using NameValueCollection = System.Collections.Specialized.NameValueCollection;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Clase para mapear registros de base de datos
    /// </summary>
    [Serializable]
    public class Registro : Eventos<Registro>, IEtiquetaIConsejos, IConsultable, ITabla
    {
        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Registro()
            : base()
        {
            IniRegistro();
            Id = Lenguaje.Registro;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        public Registro(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniRegistro();
            Serie serie = (Serie)context.Context;
            _etiqueta = info.GetString(serie.Valor());
            _consejos = info.GetString(serie.Valor());
            _tabla = info.GetString(serie.Valor());

            Campos = Constructor.Embriones[Lenguaje.Campos].Germinar<Campos>(info, context);
            // setea el parentesco de los campos
            foreach (Campo c in _campos.Contenido)
            {
                c.Padre = this;
            }
            /*for (int i = 0; i < _campos.Largo; i++)
                _campos[i].Padre = this;*/

            if (info.GetBoolean(serie.Valor()))
            {
                _propiedades = Constructor.Embriones[Lenguaje.Propiedades].Germinar<Propiedades>(info, context);
            }

            if (info.GetBoolean(serie.Valor()))
            {
                Relaciones = Constructor.Embriones[Lenguaje.Relaciones].Germinar<Recipiente<Relacion>>(info, context);
                // setea el parentesco de las relaciones
                foreach (Relacion r in _relaciones.Contenido)
                {
                    r.Padre = this;
                }
                /*
                for (int i = 0; i < _relaciones.Largo; i++)
                    _relaciones[i].Padre = this;*/
            }
            if (info.GetBoolean(serie.Valor()))
            {
                Traductores = Constructor.Embriones[Lenguaje.Traductores].Germinar<Recipiente<Traductor>>(info, context);
                // setea el parentesco de los traductores
                foreach (Traductor t in _traductores.Contenido)
                {
                    t.Padre = this;
                }
                /*for (int i = 0; i < _traductores.Largo; i++)
                    _traductores[i].Padre = this;*/
            }

        }
        void IniRegistro()
        {
            Clase = Lenguaje.Registro;
            Genero = Genero.Masculino;
            Numero = Numero.Singular;
            AntesDeEscribirContenedor = escritorXMLRegistro;
            LeerPropiedades = lectorXMLRegistro;
            LeerAtributos = lectorXMLEventos;
        }

        protected override Registro Ambito()
        {
            return this;
        }
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

        #region etiqueta
        private string _etiqueta = string.Empty;
        /// <summary>
        /// <c>etiqueta : </c> representa el titulo de la tabla 
        /// </summary>
        public string Etiqueta
        {
            get { return _etiqueta; }
            set
            {
#if RuntimeCache
                string et = _etiqueta;
                if (Atributos.Respaldable(Atributo.etiqueta))
                    Atributos.Agregar(Atributo.etiqueta, delegate () { _etiqueta = et; });
#endif
                _etiqueta = value;
            }

        }

        #endregion

        #region campos
        private Campos _campos;

        /// <summary>
        /// <c>campos : </c> representa la coleccion de columnas de una tabla
        /// </summary>
        public Campos Campos
        {
            get { return _campos; }
            set
            {
                _campos = value;
                _campos.Padre = this;
                _campos.Adoptar = delegate () { return this; };
            }
        }
        /// <summary>
        /// criterios de cualquier operacion de base de datos
        /// </summary>
        private Recipiente<Campo> _criterios;
        public Recipiente<Campo> Criterios
        {
            get
            {
                if (BaseDeDatos.Recuperacion == Recuperacion.PorConsulta)
                    return _criterios;
                return _campos.Claves();
            }
            set
            {
#if RuntimeCache
                if (Atributos.Respaldable(Atributo.criterios))
                    Atributos.Agregar(Atributo.criterios, delegate () { _criterios = null; });
#endif
                _criterios = value;
            }
        }

        Recipiente<Campo> IConsultable.Ordenadores
        {
            get { return null; }
            set {; }
        }
        Recipiente<Campo> IConsultable.Totalizadores
        {
            get { return null; }
            set {; }
        }

        #endregion

        #region consejos
        private string _consejos = string.Empty;
        /// <summary>
        /// <c>consejos : </c> representa los mensajes que se puedan generar 
        /// </summary>
        public string Consejos
        {
            get { return _consejos; }
            set
            {
#if RuntimeCache
                string csjs = _consejos;
                if (Atributos.Respaldable(Atributo.consejos))
                    Atributos.Agregar(Atributo.consejos, delegate () { _consejos = csjs; });
#endif
                _consejos = value;
            }

        }

        #endregion

        #region distinto
        /// <summary>
        /// todos distindos
        /// </summary>
        public bool Distinto
        {
            get { return false; }
            set {; }

        }
        #endregion

        #region propiedades
        private Propiedades _propiedades;
        /// <summary>
        /// <c>propiedades : </c> Permite acceder a la coleccion de propiedades del objeto
        /// </summary>
        public Propiedades Propiedades
        {
            get { return _propiedades; }
            set
            {
#if RuntimeCache
                Propiedades prop = _propiedades;
                if (Atributos.Respaldable(Atributo.propiedades))
                    Atributos.Agregar(Atributo.propiedades,
                    delegate () { _propiedades = prop; });
#endif
                _propiedades = value;
            }

        }
        #endregion

        #region relaciones
        private Recipiente<Relacion> _relaciones;
        /// <summary>
        /// coleccion de relaciones
        /// </summary>
        public Recipiente<Relacion> Relaciones
        {
            get
            {
                return _relaciones;
            }
            set
            {
                _relaciones = value;
                _relaciones.Padre = this;
                _relaciones.NombreContenido = Lenguaje.Relaciones;
                _relaciones.Adoptar = delegate () { return this; };
            }
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
#if RuntimeCache
                Requerimiento req = _requerimiento;
                if (Atributos.Respaldable(Atributo.requerimiento))
                    Atributos.Agregar(Atributo.requerimiento, delegate () { _requerimiento = req; });
#endif
                _requerimiento = value;
            }

        }
        #endregion

        #region tabla
        private string _tabla = string.Empty;
        /// <summary>
        /// <c>tabla : </c> representa el origen de la tabla en la base de datos
        /// </summary>
        public string Tabla
        {
            get { return _tabla; }
            set
            {
#if RuntimeCache
                string tb = _tabla;
                if (Atributos.Respaldable(Atributo.tabla))
                    Atributos.Agregar(Atributo.tabla, delegate () { _tabla = tb; });
#endif
                _tabla = value;
            }

        }

        #endregion

        #region traductores
        private Recipiente<Traductor> _traductores;
        /// <summary>
        /// <c>traductores : </c> representa la coleccion traductores de un dataRow
        /// </summary>
        public Recipiente<Traductor> Traductores
        {
            get { return _traductores; }
            set
            {
                _traductores = value;
                _traductores.Padre = this;
                _traductores.NombreContenido = Lenguaje.Traductores;
                _traductores.Adoptar = delegate () { return this; };
            }
        }

        #endregion

        #region transitividad
        // recipiente con todas las _transitividad segun los origenes
        Recipiente<Relacion> _transitividad = new Recipiente<Relacion>();
        /// <summary>
        /// definicion de la transitividad
        /// </summary>
        public Recipiente<Relacion> Transitividad
        {
            get { return _transitividad; }
        }

        #endregion


        #endregion

        #region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            Serie serie = (Serie)context.Context;
            info.AddValue(serie.Valor(), _etiqueta);
            info.AddValue(serie.Valor(), _consejos);
            info.AddValue(serie.Valor(), _tabla);

            _campos.GetObjectData(info, context);

            if (_propiedades != null)
            {
                info.AddValue(serie.Valor(), true);
                _propiedades.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false);
            }

            if (_relaciones != null)
            {
                info.AddValue(serie.Valor(), true);
                _relaciones.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false);
            }

            if (_traductores != null)
            {
                info.AddValue(serie.Valor(), true);
                _traductores.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false);
            }

        }
        #endregion

        #region metodos

        #region actualizar
        /// <summary>
        /// actualiza el regPadre en la base de datos
        /// </summary>
        /// <returns>exito de la operación</returns>
        public bool Actualizar()
        {
            BaseDeDatos.Actualizar();
            return !En(Estado, Estado.Error);
        }
        #endregion

        #region entradaUsuario
        /// <summary>
        /// especificacion de campos del usuario en la operación
        /// </summary>
        /// <param name="request">coleccion de nombre valor</param>
        /// <returns>colección de campos</returns>
        public Recipiente<Campo> EntradaUsuario(NameValueCollection request)
        {
            // debe devolver un recipiente de campos
            Recipiente<Campo> condiciones = new Recipiente<Campo>();

            string valor;
            string campo;
            _campos.Limpiar();
            for (int i = 0; i < request.AllKeys.Length; i++)
            {
                campo = request.AllKeys[i];
                if (!string.IsNullOrEmpty(campo))
                {
                    if (_campos.Existe(campo))
                    {
                        valor = request[campo];
                        if (!string.IsNullOrEmpty(valor))
                        {
                            _campos[campo].Valor = valor;
                            condiciones.Agregar(_campos[campo]);
                        }
                    }
                }
            }
            return Criterios = condiciones;
        }
        #endregion

        #region ejecutarEvento
        /// <summary>
        /// <c>ejecutarEvento : </c> Permite ejecutar los eventos del objeto
        /// </summary>  
        public override bool Ejecutar(Evento trigger)
        {
            bool ret;
            if (ret = base.Ejecutar(trigger))
            {
                // repliega el evento en el contenido
                foreach (Campo campo in _campos)
                {
                    if (!(ret = campo.Ejecutar(trigger)))
                    {
                        Estado |= Estado.Error;
                        break;
                    }
                }
            }
            return ret;
        }
        #endregion

        #region eliminar
        /// <summary>
        /// elimina el regPadre en la base de datos
        /// </summary>
        /// <returns></returns>
        public bool Eliminar()
        {
            BaseDeDatos.Eliminar();
            return !En(Estado, Estado.Error);
        }
        #endregion

        #region insertar
        /// <summary>
        /// inserta el regPadre en la base de datos
        /// </summary>
        /// <returns></returns>
        public bool Insertar()
        {
            BaseDeDatos.Insertar();
            return !En(Estado, Estado.Error);
        }
        #endregion

        #region origenes
        /// <summary>
        /// determina los origenes de la coleccion de campos especificada
        /// </summary>
        /// <param name="campos">coleccio de campos</param>
        /// <returns>coleccion de registros</returns>
        public Recipiente<Registro> Origenes(params Recipiente<Campo>[] campos)
        {
            Recipiente<Registro> origen = new Recipiente<Registro>();
            origen.Agregar(this);
            return origen;
        }
        #endregion

        #region recuperar
        /// <summary>
        /// recupera desde la base de datos
        /// </summary>
        /// <returns>exito de la operacion</returns>
        public bool Recuperar()
        {
            bool ret = false;
            using (ILectorBD datos = BaseDeDatos.Consultar())
            {
                if (datos.HasRows)
                {
                    datos.Read();
                    _campos.Recuperar(datos);
                    ret = true;
                }
            }
            return ret;
        }
        #endregion

        #region restaurar
        /// <summary>
        /// <c>restaurar : </c> vuelve el objeto al estado inicial.
        /// </summary>  
#if RuntimeCache
        public override void Restaurar()
        {

            _campos.Restaurar();
            if (_traductores != null)
                _traductores.Restaurar();
            if (_relaciones != null)
                _relaciones.Restaurar();
            base.Restaurar();

        }
#endif
        #endregion

        void lectorXMLRegistro(System.Xml.XmlReader reader)
        {
            string Etiqueta = reader.GetAttribute(Lenguaje.Etiqueta);
            if (!string.IsNullOrEmpty(Etiqueta))
            {
                _etiqueta = Etiqueta;
            }

            string Tabla = reader.GetAttribute(Lenguaje.Tabla);
            if (!string.IsNullOrEmpty(Tabla))
            {
                _tabla = Tabla;
            }

            reader.Read();
            if (reader.Name.Equals(Lenguaje.Campos))
            {
                Campos = new Campos();
                _campos.ReadXml(reader);
                reader.Read();
            }
            if (reader.Name.Equals(Lenguaje.Relaciones))
            {
                Relaciones = new Recipiente<Relacion>();
                //_relaciones.antesDeLeerContenedor = avanzar;
                _relaciones.ReadXml(reader);
            }
            if (reader.Name.Equals(Lenguaje.Traductores))
            {
                Traductores = new Recipiente<Traductor>();
                _traductores.ReadXml(reader);
            }
            //if (reader.IsEmptyElement)
            //    return;

            if (reader.Name.Equals(Lenguaje.Propiedades))
            {
                _propiedades = new Propiedades();
                _propiedades.lectorXMLPropiedades(reader);
                reader.Read();
            }
        }

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorXMLRegistro(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(_etiqueta))
            {
                writer.WriteAttributeString(Lenguaje.Etiqueta, _etiqueta);
            }

            if (!string.IsNullOrEmpty(_tabla))
            {
                writer.WriteAttributeString(Lenguaje.Tabla, _tabla);
            }

            if (_campos != null)
            {
                _campos.WriteXml(writer);
            }

            if (_relaciones != null)
            {
                _relaciones.WriteXml(writer);
            }

            if (_traductores != null)
            {
                _traductores.WriteXml(writer);
            }

            if (_propiedades != null)
            {
                _propiedades.escritorXMLPropiedades(writer);
            }
        }
        #endregion

        public override void EnlazarGrafica(Variables variables, HttpRequest request = null)
        {
            base.EnlazarGrafica(variables, request);
            Campos.EnlazarGrafica(variables, request);
        }
        public override void MesclarContenido(Variables variables)
        {
            base.MesclarContenido(variables);
            Campos.MesclarContenido(variables);
        }

        #endregion

    }
}