// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using XmlRoot = System.Xml.Serialization.XmlRootAttribute;
    using XmlWriter = System.Xml.XmlWriter;
    using XmlReader = System.Xml.XmlReader;
    using XmlSerializer = System.Xml.Serialization.XmlSerializer;
    using System.Collections.Generic;
    using System.Web;
    using Jen.Json;
    using System;

    /// <summary>
    /// <c>Recipiente : </c> clase base para la manipulación de objetos 
    /// dentro de un contenedor genérico, los objetos serán almacenados 
    /// por una clave del tipo string 
    /// </summary>
    /// <typeparam name="TContenido">Es el tipo del objeto a manipular</typeparam>
    [Serializable, XmlRoot(Lenguaje.Recipiente)]
    public class Recipiente<TContenido> : Semilla
        where TContenido : ISemilla
    {
        // hooks para leer  el archivo xml
        public Proc<XmlWriter> AntesDeEscribirContenedor;
        public Proc<XmlWriter> DespuesDeEscribirContenedor;
        public Proc<XmlReader> LeerPropiedades;
        public Proc<XmlReader> LeerAtributos;

        // hook para asignar el padre del objeto
        public Func<ISemilla> Adoptar;

        private System.Collections.Generic.List<string> Indices;


        #region constructor
        /// <summary>
        /// Constructor binario
        /// </summary>
        public Recipiente()
            : base()
        {
            IniRecipiente();
            Id = Lenguaje.Recipiente;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Recipiente(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniRecipiente();
            Serie serie = (Serie)context.Context;

            int _largo = info.GetInt32(serie.Valor());
            string clsObj;
            for (int i = 0; i < _largo; i++)
            {
                clsObj = info.GetString(serie.Valor());
                TContenido item = Constructor.Embriones[clsObj].Germinar<TContenido>(info, context);
                Agregar(item);
            }
        }

        void IniRecipiente()
        {
            Clase = Lenguaje.Recipiente;
            Genero = Genero.Masculino;
            EscritorXML = escritorRecipiente;
            LectorXML = LectorXMLRecipiente;
            AntesDeEscribirContenedor = EscritorNulo;
            DespuesDeEscribirContenedor = EscritorNulo;
            LeerPropiedades = LectoNulo;
            LeerAtributos = LectoNulo;
            Adoptar = delegate () { return this; };
            _contenido = new Dictionary<string, TContenido>();
            Indices = new List<string>();
        }

        #endregion

        #region propiedades

        #region contenido
        private System.Collections.Generic.Dictionary<string, TContenido> _contenido;
        #endregion

        #region NombreContenido
        /// <summary>
        /// <c>NombreContenido : </c> permite acceder al numero de elementos del contenedor 
        /// </summary>
        private string nombreContenido;
        public string NombreContenido
        {
            get
            {
                return nombreContenido;
            }
            set
            {
                nombreContenido = value;
            }
        }
        #endregion

        #region largo
        /// <summary>
        /// <c>largo : </c> permite acceder al numero de elementos del contenedor 
        /// </summary>
        public int Largo
        {
            get { return _contenido.Count; }
        }
        #endregion

        #endregion propiedades

        #region métodos

        #region agrega(Recipiente<Contenido> obj)
        /// <summary>
        /// <c>agrega : </c> agrega los elementos dentro del contenido
        /// </summary>
        /// <mr name="obj"> recipiente de objetos a agregar en el contenido</mr>
        public void Agregar(Recipiente<TContenido> rc)
        {
            // itera en todos los elementos a agregar
            foreach (TContenido c in rc)
            {
                Agregar(c);
            }
        }
        #endregion

        #region agrega(Contenido[] obj)
        /// <summary>
        /// <c>agrega : </c> agrega los elementos dentro del contenido
        /// </summary>
        /// <mr name="obj"> objetos a agregar en el contenido</mr>
        public virtual void Agregar(params TContenido[] obj)
        {
            TContenido c;
            // itera en todos los elementos a agregar
            for (int i = 0; i < obj.Length; i++)
            {
                c = obj[i];
                if (c.Padre == null)
                {
                    c.Padre = Adoptar();
                }

                string id = c.Id;
                // valida que no exista el elemento
                if (!_contenido.ContainsKey(id))
                {
#if RuntimeCache
                            // esta agregando programaticamente 
                            if (Semilla.Ocupado(this))
                                // agrega el restaurador
                                Atributos.Restauradores.Add(delegate()
                                {
                                    _contenido.Remove(id);
                                    Indices.Remove(id);
                                });
#endif
                    // agrega el elemento al contenido
                    _contenido.Add(id, c);
                    Indices.Add(id);
                }
            }
        }
        #endregion

        #region Borrar
        /// <summary>
        /// permite borrar el contenedor de objetos
        /// </summary>
        public void Borrar()
        {
            _contenido.Clear();
            Indices.Clear();
        }
        #endregion

        #region eliminar
        /// <summary>
        /// permite eliminar un elemento del contenedor
        /// </summary>
        /// <param name="obj">ob</param>jeto a eliminar
        public void Eliminar(string obj)
        {
#if RuntimeCache
                    if (Semilla.Ocupado(this))
                    {
                        TContenido elemento = _contenido[obj];
                        Proc restauraEliminarElemento = delegate() 
                        { 
                            _contenido.Add(obj, elemento); 
                        };
                        Proc restauraEliminarIndice = delegate() 
                        {
                            Indices.Add(obj); 
                        };
                        Proc restauraRestauradores = delegate() 
                        { 
                            Atributos.Restauradores.Remove(restauraEliminarElemento);
                            Atributos.Restauradores.Remove(restauraEliminarIndice); 
                        };
                        // agrega los restauradores
                        Atributos.Restauradores.Add(restauraRestauradores);
                        Atributos.Restauradores.Add(restauraEliminarElemento);
                        Atributos.Restauradores.Add(restauraEliminarIndice);
                    }
#endif
            _contenido.Remove(obj);
            Indices.Remove(obj);
        }
        #endregion

        #region existe
        /// <summary>
        /// <c>existe : </c> permite verificar si existe el elemento en el contenido
        /// </summary>
        /// <mr name="id"></mr>
        /// <returns></returns>
        public bool Existe(string id)
        {
            return _contenido.ContainsKey(id);
        }
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
            info.AddValue(serie.Valor(), Largo);
            foreach (TContenido c in _contenido.Values)
            {
                info.AddValue(serie.Valor(), c.Clase);
                c.GetObjectData(info, context);
            }
        }
        #endregion

        #region indexador
        /// <summary>
        /// <c>indexador : </c> permite acceder a los elementos del Recipiente mediante brackets
        /// </summary>
        /// <mr name="id">nombreId del objeto </mr>
        /// <returns>retorna el objeto direccionado</returns>
        public TContenido this[string id]
        {
            get
            {
                TContenido tc;
                if (_contenido.TryGetValue(id, out tc))
                {
                    return tc;
                }
                throw new System.Exception(id + " no existe en " + NombreContenido);
                //return null;
            }
        }
        /// <summary>
        /// indexador numerico
        /// </summary>
        /// <param name="id">posicion del objeto</param>
        /// <returns>objeto a retornar</returns>
        public TContenido this[int id]
        {
            get
            {
                return this[Indices[id]];
                //return _contenido[Indices[id]];
            }
        }
        #endregion
        #region Excluir - Incluir
        public void Excluir(string[] elementos)
        {
            TContenido tc;
            foreach (string e in elementos)
            {
                if (_contenido.TryGetValue(e, out tc))
                {
                    tc.Estado |= Estado.Excluido;
                }
            }
        }
        public void Incluir(string[] elementos)
        {
            TContenido tc;
            foreach (string e in elementos)
            {
                if (_contenido.TryGetValue(e, out tc))
                {
                    tc.Estado -= Estado.Excluido;
                }
            }
        }
        public void Excluir(Arreglo elementos)
        {
            TContenido tc;
            foreach (string e in elementos)
            {
                if (_contenido.TryGetValue(e, out tc))
                {
                    tc.Estado |= Estado.Excluido;
                }
            }
        }
        public void Incluir(Arreglo elementos)
        {
            TContenido tc;
            foreach (string e in elementos)
            {
                if (_contenido.TryGetValue(e, out tc))
                {
                    tc.Estado -= Estado.Excluido;
                }
            }
        }

        #endregion
        #region iterador
        /// <summary>
        /// enumerador del contenido 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TContenido> GetEnumerator()
        {
            TContenido item;
            foreach (string k in _contenido.Keys)
            {
                if (_contenido.TryGetValue(k, out item))
                {
                    if (!En(item.Estado, Estado.Excluido))
                    {
                        yield return item;
                    }
                }
            }
        }
        public IEnumerable<TContenido> Contenido
        {
            get
            {
                TContenido item;
                foreach (string k in _contenido.Keys)
                {
                    if (_contenido.TryGetValue(k, out item))
                    {
                        yield return item;
                    }
                }
            }
        }
        #endregion

        #region ReadXml
        /// <summary>
        /// <c>ReadXml : </c>Genera el objeto desde su representación XML.
        /// </summary>
        /// <mr name="reader">Tipo que permite leer el Archivo xml</mr>
        public void LectorXMLRecipiente(System.Xml.XmlReader lectorXml)
        {
            // clase por defecto del recipiente obtenida del nombre de la etiqueta
            string clsInstancia;
            // clase declarada en la propiedad
            string clsElemento;
            // clase del elemento tipo del recipiente
            string clsTipo;
            // engancha el delegado que le atributos del recipiente
            LeerPropiedades(lectorXml);
            // declara un tipo para el iesimo elemento del recipiente
            TContenido elemento;
            // verifica que el contenedor no esté vacío ".../>"
            if ((lectorXml.IsEmptyElement) || (lectorXml.NodeType == System.Xml.XmlNodeType.EndElement))
            {
                return;
            }
            // avanza en la lectura del documento
            lectorXml.Read();
            // engancha el delegado que lee elementos definidos despues de los atributos
            LeerAtributos(lectorXml);
            // comprueba que no esté vacío
            if (lectorXml.NodeType == System.Xml.XmlNodeType.EndElement)
            {
                return;
            }
            // obtiene la clase del elemento contenido en la etiqueta
            clsTipo = clsInstancia = lectorXml.Name;
            do
            {
                // obtiene la clase del objeto en la propiedad, esta tiene prioridad
                if ((clsElemento = lectorXml.GetAttribute(Lenguaje.Clase)) != null)
                {
                    clsInstancia = clsElemento;
                }
                /*try
                 {*/
                // instancia de acuerdo a la clase declarada en el atributo del objeto
                elemento = (TContenido)Constructor.Embriones[clsInstancia].Germinar();

                // lee las propiedades del objeto desde el documento XML
                elemento.ReadXml(lectorXml);
                // agrega el elemento al contenedor
                Agregar(elemento);
                /*}
                catch (System.Exception e)
                {
                    e.ToString();
                }*/
                // avanza en la lectura del documento
                lectorXml.Read();
            } while (clsTipo.Equals(lectorXml.Name) &&
                    (lectorXml.NodeType != System.Xml.XmlNodeType.EndElement));

            if (!Clase.Equals(lectorXml.Name) && (lectorXml.NodeType == System.Xml.XmlNodeType.EndElement))
            {
                lectorXml.Read();
            }
        }
        #endregion

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorRecipiente(System.Xml.XmlWriter writer)
        {
            AntesDeEscribirContenedor(writer);
            if (Largo > 0)
            {
                if (NombreContenido != null)
                {
                    writer.WriteStartElement(NombreContenido);
                }

                DespuesDeEscribirContenedor(writer);

                XmlSerializer serializardorDeElemento = new XmlSerializer(typeof(TContenido));
                //foreach (TContenido c in this)
                foreach (TContenido c in _contenido.Values)
                {
                    serializardorDeElemento.Serialize(writer, c);
                }

                if (NombreContenido != null)
                {
                    writer.WriteEndElement();
                }
            }
        }
        #endregion

        #region restaurar
        /// <summary>
        /// <c>restaurar : </c> vuelve el objeto al estado inicial.
        /// </summary>  
#if RuntimeCache
                public override void Restaurar() 
                {
                    // restaura el contenido
                    foreach (TContenido c in _contenido.Values)
                        c.Restaurar();
                    // restaura el contenedor
                    base.Restaurar();

                }
#endif
        #endregion

        #region IndexOf
        /// <summary>
        /// Busca la primera ocurrencia de la clave y retorna la posicion donde se encuentra
        /// </summary>
        /// <param name="Evento">Objeto a buscar</param>
        /// <returns>Posición</returns>
        public int IndexOf(string clave)
        {
            if (!_contenido.ContainsKey(clave))
            {
                return -1;
            }
            return Indices.IndexOf(clave);
        }
        #endregion
        public override void EnlazarGrafica(Variables variables, HttpRequest request = null)
        {
            base.EnlazarGrafica(variables, request);
            string _id = Id;
            string clsObjGraf;
            if (Padre != null)
            {
                _id = Padre.Id;
            }

            string nVar = string.Concat(_id, Lenguaje.Punto, NombreContenido);
            Variable vContenido;
            System.Reflection.PropertyInfo pContexto;
            //object oGraf;
            if (variables.Existe(nVar))
            {
                vContenido = variables[nVar];
                foreach (Declaracion s in vContenido)
                {
                    if (Existe(s.Identificadores[0]))
                    {
                        if (s.ObjetoGrafico == null)
                        {
                            TContenido c = _contenido[s.Identificadores[0]];
                            clsObjGraf = s.Identificadores[1] + c.Clase;

                            Embrion emb = Constructor.Embriones[clsObjGraf];
                            if (emb.ConstructorJson)
                            {
                                Objeto cfg = new Objeto(1);
                                cfg.Add("Clase", s.Id);
                                s.ObjetoGrafico = Constructor.Embriones[clsObjGraf].Germinar<IU.ObjetoGrafico>(cfg);
                                s.ObjetoGrafico.Request = request;
                                pContexto = s.ObjetoGrafico.GetType().GetProperty(Lenguaje.Contexto);
                                pContexto.SetValue(s.ObjetoGrafico, c, null);
                            }
                            else
                            {
                                s.ObjetoGrafico = (IU.ObjetoGrafico)Constructor.Embriones[clsObjGraf].Germinar();
                                s.ObjetoGrafico.Request = request;
                                pContexto = s.ObjetoGrafico.GetType().GetProperty(Lenguaje.Contexto);
                                pContexto.SetValue(s.ObjetoGrafico, c, null);
                            }
                        }
                        else
                        {
                            TContenido c = _contenido[s.Identificadores[0]];
                            pContexto = s.ObjetoGrafico.GetType().GetProperty(Lenguaje.Contexto);
                            s.ObjetoGrafico.Request = request;
                            pContexto.SetValue(s.ObjetoGrafico, c, null);
                        }
                    }
                }
            }
        }
        public override void MesclarContenido(Variables variables)
        {
            base.MesclarContenido(variables);
            string _id = Id;
            if (Padre != null)
            {
                _id = Padre.Id;
            }
            string nVar = _id + Lenguaje.Punto + NombreContenido;
            if (variables.Existe(nVar))
            {
                variables[nVar].Asignar();
            }
        }

        public static implicit operator Recipiente<TContenido>(Recipiente<Patron> v)
        {
            throw new NotImplementedException();
        }

        #endregion métodos
    }
}