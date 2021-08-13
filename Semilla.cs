// Autor : Juan Parra
// 3Soft
[assembly: System.CLSCompliant(true)]
namespace Jen
{
    using System.Security.Permissions;
    using System.Web;
    using Json;
    using Interlocked = System.Threading.Interlocked;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Semilla : Clase base que implementa la interface para la identificación 
    ///  y manipulación de los objetos del componente.
    /// </summary>
    [Serializable]
    public class Semilla : ISemilla
    {
        #region campos
        // Contenedor de los valores iniciales de los atributos
        internal Atributos Atributos;
        /// propiedad para sincronizar el acceso del objeto in modo runtime
#if RuntimeCache
        private int sinc = 0;
#endif
        #endregion
        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Semilla()
        {
            // establece el estado de la semilla
            _estado = Estado.Instancia | Estado.Inicializado;
            // setea las propiedades para determinar el articulo del objeto
            _genero = Genero.Femenino;
            _numero = Numero.Singular;
            _id = _clase = Lenguaje.Semilla;

#if RuntimeCache
            // crea el objeto para respaldar propiedades
            Atributos = new Atributos();
            Atributos.Padre = this;
#endif
            EscritorXML = EscritorNulo;
            LectorXML = LectoNulo;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Semilla(SerializationInfo info, StreamingContext context)
            : this()
        {
            Serie serie = (Serie)context.Context;
            // obtiene su identificador
            _id = info.GetString(serie.Valor());

            // verifica si el proximo valor indica estado excluido
            string strEstado = info.GetString(serie.Proximo());
            if (!string.IsNullOrEmpty(strEstado))
                if (strEstado.Equals(Estado.Excluido.ToString()))
                    _estado |= Estado.Excluido;
        }
        #endregion
        #region propiedades
        #region articulo
        /// <summary>
        /// <c>articulo : </c> permite determinar la denominacion de objeto vía el genero.
        /// <returns>Devuelve la denominación del objeto.</returns>
        /// </summary>       
        public string Articulo
        {
            get
            {
                if (_genero == Genero.Femenino)
                {
                    if (_numero == Numero.Singular)
                    {
                        return Lenguaje.La;
                    }
                    return Lenguaje.Las;
                }
                if (_genero == Genero.Masculino)
                {
                    if (_numero == Numero.Singular)
                    {
                        return Lenguaje.El;
                    }
                    return Lenguaje.Los;

                }
                //caso indeterminado
                if (_numero == Numero.Singular)
                {
                    return Lenguaje.Lo;
                }
                return Lenguaje.Los;
            }
        }
        #endregion
        #region clase
        /// <summary>
        /// Almacena el valor string del tipo del objeto
        /// </summary>
        private string _clase;
        /// <summary>
        /// <c>clase : </c> Identificador o nombre de la clase del objeto, se obtiene via reflección.
        /// </summary>       
        public string Clase
        {
            get
            {
                return _clase;
            }
            set
            {
                _clase = value;
            }
        }
        #endregion
        #region estado
        private Estado _estado;
        /// <summary>
        /// <c>estado : </c> estado actual del objeto
        /// </summary>
        public Estado Estado
        {
            get
            {
                return _estado;
            }
            set
            {
#if RuntimeCache
                if (!Atributos.Contains(Atributo.estado))
                    Atributos.Agregar(Atributo.estado, delegate ()
                    {
                        _estado = Estado.Instancia | Estado.Inicializado;
                    });
#endif
                _estado = value;

            }
        }
        #endregion
        #region EscritorXML
        Proc<System.Xml.XmlWriter> _escritorXML;
        public Proc<System.Xml.XmlWriter> EscritorXML
        {
            get
            {
                return _escritorXML;
            }
            set
            {
                _escritorXML = value;
            }
        }
        #endregion
        #region genero
        private Genero _genero;
        /// <summary>
        /// <c>genero : </c> Calificación de genero del objeto.
        /// </summary>       
        internal Genero Genero
        {
            set { _genero = value; }
        }
        #endregion
        #region numero
        private Numero _numero;
        /// <summary>
        ///     <c>Numero</c> Calificación de numero del objeto(singular, plural).
        /// </summary>
        internal Numero Numero
        {
            set { _numero = value; }
        }
        #endregion
        #region id
        private string _id;
        /// <summary>
        /// id : nombreId del objeto
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
#if RuntimeCache
                string iden = _id;
                if (Atributos.Respaldable(Atributo.id))
                    Atributos.Agregar(Atributo.id, delegate () { _id = iden; });
#endif
                _id = value;
            }

        }
        #endregion
        #region LectorXML
        Proc<System.Xml.XmlReader> _lectorXML;
        public Proc<System.Xml.XmlReader> LectorXML
        {
            get
            {
                return _lectorXML;
            }
            set
            {
                _lectorXML = value;
            }
        }
        #endregion

        #region Plantillas
        Recipiente<Plantilla> _plantilla;
        public Recipiente<Plantilla> Plantillas
        {
            get
            {
                return _plantilla;
            }
            set
            {
                _plantilla = value;
            }
        }
        #endregion

        #region Request
        HttpRequest _request;
        public HttpRequest Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
            }
        }
        #endregion

        #region padre
        private ISemilla _padre;
        /// <summary>
        ///     <c>Padre : </c> Referencia del objeto padre.
        /// </summary>
        public ISemilla Padre
        {
            get { return _padre; }
            set
            {
#if RuntimeCache
                ISemilla pad = _padre;

                if (Atributos.Respaldable(Atributo.padre))
                    Atributos.Agregar(Atributo.padre, delegate () { _padre = pad; });
#endif
                _padre = value;
            }

        }
        #endregion
        #region tag
        private string _tag;
        /// <summary>
        ///     <c>Tag : </c> info adicional.
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set
            {
#if RuntimeCache
                /*
                string tag = _tag;

                if (Atributos.Respaldable(Atributo.tag))
                    Atributos.Agregar(Atributo.padre, delegate () { _tag = tag; });
                    */
#endif
                _tag = value;
            }

        }

        #endregion
        #endregion propiedades
        #region métodos

        #region En
        /// <summary>
        ///     <c>En : </c>  Metodo estatico que reporta si el objeto esta en el estado especificado.
        /// </summary>
        public static bool En(Estado propiedad, Estado estado)
        {
            return ((propiedad & estado) == estado);
        }
        #endregion

        #region Ocupado
        /// <summary>
        ///     <c>Ocupado : </c>  Reporta si el objeto está ocupado o uno en la jerarquia enlazada por el padre.
        /// superior está ocupado
        /// </summary>

        internal static bool Ocupado(ISemilla obj)
        {
            if (Semilla.En(obj.Estado, Estado.Ocupado))
            {
                return true;
            }

            ISemilla predecesor = obj.Padre;
            while (predecesor != null)
            {
                if (Semilla.En(predecesor.Estado, Estado.Ocupado))
                {
                    return true;
                }
                predecesor = predecesor.Padre;
            }
            return false;
        }
        #endregion

        #region Restaurar
        /// <summary>
        /// <c>restaurar : </c> vuelve el objeto al estado inicial.
        /// </summary>  

        public virtual void Restaurar()
        {
#if RuntimeCache
            Atributos.Restaurar();
#endif
        }
        #endregion

        #region GetSchema
        /// <summary>
        /// <c>GetSchema : </c> Este método en reservado y no en usado, pero debe ser por contrato 
        /// retornando null. 
        /// Si especificara un esquema personalizado en requerido aplicando un XmlSchemaProviderAttribute
        /// para lla clase.
        /// </summary>
        /// <returns>Retorna nulo</returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        #endregion

        #region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            // agrega la propiedad Id
            info.AddValue(serie.Valor(), _id);
            if (En(_estado, Jen.Estado.Excluido))
                info.AddValue(serie.Valor(), Jen.Estado.Excluido.ToString());
        }
        #endregion

        #region Sincronice
        /// <summary>
        ///Sincronice : Permite el acceso y escritura de la propiedad estado del objeto en modo seguro, marcandola como Ocupado, mediante acceso sincronizado.
        /// </summary>
#if RuntimeCache
        internal static bool sincro(Semilla obj)
        {
            bool ret = false;
            if (0 == Interlocked.Exchange(ref obj.sinc, 1))
            {
                if (!Semilla.En(obj.Estado, Estado.Ocupado))
                {
                    obj.Estado |= Estado.Ocupado;
                    ret = true;
                }
                Interlocked.Exchange(ref obj.sinc, 0);
            }
            return ret;
        }
#endif
        #endregion

        #region ReadXml
        /// <summary>
        /// <c>ReadXml : </c>Genera el objeto desde su representación XML.
        /// </summary>
        /// <mr name="reader">Tipo que permite leer el Archivo xml</mr>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            string idObj = reader.GetAttribute(Lenguaje.Id);
            if (!string.IsNullOrEmpty(idObj))
            {
                _id = idObj;
            }
            string estadoObj = reader.GetAttribute(Lenguaje.Estado);
            if (!string.IsNullOrEmpty(estadoObj))
            {
                if (estadoObj.Equals(Estado.Excluido.ToString()))
                {
                    _estado |= Estado.Excluido;
                }
            }
            LectorXML(reader);
        }

        #endregion

        #region ToString
        /// <summary>
        /// <c>serializar : </c> Indicador para serializar un objeto.
        /// </summary>  
        public override string ToString() { return _id; }
        #endregion

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!_id.Equals(_clase))
            {
                writer.WriteAttributeString(Lenguaje.Id, _id);
            }

            if (Semilla.En(_estado, Estado.Excluido))
            {
                writer.WriteAttributeString(Lenguaje.Estado, Jen.Estado.Excluido.ToString());
            }
            // hook que escribe el resto de las clases derivadas
            EscritorXML(writer);
        }
        #endregion

        static internal void EscritorNulo(System.Xml.XmlWriter writer) { }
        static internal void LectoNulo(System.Xml.XmlReader reader) { }
        static internal void LeerXML(System.Xml.XmlReader reader)
        {
            reader.Read();
        }
        public virtual void EnlazarGrafica(Variables variables, HttpRequest request = null)
        {
            System.Reflection.PropertyInfo pContexto;
            string clsObjGraf;
            if (variables.Existe(_id))
            {
                foreach (Declaracion s in variables[_id])
                {
                    if (s.ObjetoGrafico == null)
                    {
                        clsObjGraf = s.Id + _clase;
                        if (Constructor.Embriones.Existe(clsObjGraf))
                        {
                            Embrion emb = Constructor.Embriones[clsObjGraf];
                            s.ObjetoGrafico = (IU.ObjetoGrafico)emb.Germinar();
                            s.ObjetoGrafico.Request = request;
                            s.ObjetoGrafico.Padre = this;
                        }
                        else
                        {

                            Objeto cfg = new Objeto(1);

                            if (s.Id.Contains(".Id"))
                            {
                                cfg.Add("Clase", string.Concat(Id, s.Id.Replace(".Id", string.Empty)));
                            }
                            else
                            {
                                cfg.Add("Clase", s.Id);
                            }
                            s.ObjetoGrafico = new IU.Generacion.VarPass<Semilla>(cfg);
                            s.ObjetoGrafico.Request = request;
                        }
                        pContexto = s.ObjetoGrafico.GetType().GetProperty(Lenguaje.Contexto);
                        pContexto.SetValue(s.ObjetoGrafico, this, null);
                    }
                    else
                    {
                        pContexto = s.ObjetoGrafico.GetType().GetProperty(Lenguaje.Contexto);
                        s.ObjetoGrafico.Request = request;
                        pContexto.SetValue(s.ObjetoGrafico, this, null);
                    }
                }
            }
        }
        public virtual void MesclarContenido(Variables variables)
        {
            if (variables.Existe(Id))
            {
                variables[Id].Asignar();
            }
        }
        #endregion métodos
    }
}