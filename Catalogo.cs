// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using Json;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;
    using NameValueCollection = System.Collections.Specialized.NameValueCollection;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// <c>Catalogo : </c> Permite el mapeo global de una base de datos
    /// </summary>
    [Serializable]
    public class Catalogo : Eventos<Catalogo>, IOrigenes, IEntradaUsuario
    {
        #region campos
        // recipiente con la transitividad segun los origenes
        Recipiente<Relacion> _transitividad = null;
        #endregion

        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Catalogo()
            : base()
        {
            IniCatalogo();
            Id = Lenguaje.Catalogo;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Catalogo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniCatalogo();
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            // obtiene el motor de base de datos
            string bd = info.GetString(serie.Valor());

            _baseDeDatos = Constructor.Embriones[bd].Germinar<BaseDatos>(info, context);
            _baseDeDatos.catalogo = this;
            _baseDeDatos.Atributos = this.Atributos;

            if (info.GetBoolean(serie.Valor()))
                _propiedades = Constructor.Embriones[Lenguaje.Propiedades].Germinar<Propiedades>(info, context);


            if (info.GetBoolean(serie.Valor()))
            {
                Registros = Constructor.Embriones[Lenguaje.Registros].Germinar<Recipiente<Registro>>(info, context);
                _registros.Padre = this;
                foreach (Registro r in _registros)
                {
                    r.Padre = this;
                    r.BaseDeDatos = _baseDeDatos;
                }
            }


            if (info.GetBoolean(serie.Valor()))
            {
                _funcBD = Constructor.Embriones[Lenguaje.Funciones].Germinar<Recipiente<Dato>>(info, context);
                //_funcBD = Constructor.Embriones[Lenguaje.Funciones].Germinar<Recipiente<Campos>>(info, context);
                _funcBD.Padre = this;
            }
#if BDMD
                if (info.GetBoolean(serie.Valor()))
                    Dimensiones = Constructor.Embriones[Lenguaje.Dimensiones].Germinar<Recipiente<Dimension>>(info, context);

                if (info.GetBoolean(serie.Valor()))
                    Cubos = Constructor.Embriones[Lenguaje.Cubos].Germinar<Recipiente<Cubo>>(info, context);
#endif
        }
        void IniCatalogo()
        {
            Clase = Lenguaje.Catalogo;
            NombreContenido = Lenguaje.Eventos;
            AntesDeEscribirContenedor = escritorXmlCatalogo;
            LeerPropiedades = lectorXMLCatalogo;
            LeerAtributos = lectorXMLEventos;
            Registros = new Recipiente<Registro>();
            //FuncionesBD = new Recipiente<Campos>();
            FuncionesBD = new Recipiente<Dato>();
        }
        protected override Catalogo Ambito()
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
            get { return _baseDeDatos; }
            set
            {
#if RuntimeCache
                        BaseDatos bdd = _baseDeDatos;
                        if (Atributos.Respaldable(Atributo.baseDeDatos))
                            Atributos.Agregar(Atributo.baseDeDatos, delegate() { _baseDeDatos = bdd; });
#endif
                _baseDeDatos = value;
            }

        }
        #endregion

        #region Funciones
        //declara la variable local que almacena las dimensiones
        private Recipiente<Dato> _funcBD;
        //private Recipiente<Campos> _funcBD;
        /// <summary>
        /// Dimensiones existentes en el catalogo.
        /// </summary>  
        public Recipiente<Dato> FuncionesBD
        //public Recipiente<Campos> FuncionesBD
        {
            get { return _funcBD; }
            set
            {
                
                _funcBD = value;
                if (_funcBD != null)
                {
                    //_funcBD.Padre = this;
                    _funcBD.NombreContenido = Lenguaje.Funciones;
                    //_funcBD.Adoptar = delegate () { return this; };
                }

            }
        }
        #endregion

#if BDMD
        #region Cubos
                //declara la variable local que almacena lo s cubos
                private Recipiente<Cubo> _cubos;
            /// <summary>
                /// Cubos existentes en el catalogo.
            /// </summary>  
                public Recipiente<Cubo> Cubos
                {
                    get { return _cubos; }
                    set 
                    { 
                        _cubos = value;
                        _cubos.NombreContenido = Lenguaje.Cubos;
                    }
                }
        #endregion

        #region Dimensiones
                //declara la variable local que almacena las dimensiones
                private Recipiente<Dimension> _dimensiones;
                /// <summary>
                /// Dimensiones existentes en el catalogo.
                /// </summary>  
                public Recipiente<Dimension> Dimensiones
                {
                    get { return _dimensiones; }
                    set 
                    { 
                        _dimensiones = value;
                        _dimensiones.NombreContenido = Lenguaje.Dimensiones;
                    }
                }
        #endregion
#endif
        #region indexador
        /// <summary>
        /// <c>indexador : </c> permite acceder a los registros del catalogo mediante brackets
        /// </summary>
        /// <mr name="id">nombreId del registro </mr>
        /// <returns>retorna el registro direccionado</returns>
        public new Registro this[string id]
        {
            get
            {
                return _registros[id];
            }
        }
        /// <summary>
        /// indexador numerico
        /// </summary>
        /// <param name="id">posicion del registro</param>
        /// <returns>resgistro a retornar</returns>
        public new Registro this[int id]
        {
            get
            {
                return _registros[id];
            }
        }
        #endregion

        #region iterador
        /// <summary>
        /// enumerador del registros 
        /// </summary>
        /// <returns></returns>
        public new System.Collections.Generic.IEnumerator<Registro> GetEnumerator()
        {
            foreach (Registro r in _registros)
            {
                if (!Semilla.En(r.Estado, Estado.Excluido))
                {
                    yield return r;
                }
            }
        }
        #endregion

        #region Propiedades
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
                        Atributos.Agregar(Atributo.propiedades, delegate() { _propiedades = prop; });
#endif
                _propiedades = value;
            }

        }
        #endregion

        #region Registros
        private Recipiente<Registro> _registros;
        /// <summary>
        /// <c>propiedades : </c> Permite acceder a la coleccion de propiedades del objeto
        /// </summary>
        public Recipiente<Registro> Registros
        {
            get { return _registros; }
            set
            {
                _registros = value;
                _registros.Padre = this;
                _registros.NombreContenido = Lenguaje.Registros;
                _registros.Adoptar = delegate () { return this; };
            }
        }
        #endregion

        #region Requerimiento
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
                // bug : no asigna el requerimiento a los registros porque esta excluido y no fue restaurado
                //foreach(Registro r in this)
                //    r.Requerimiento = value;

                if (_registros != null)
                {
                    foreach (Registro r in _registros)
                    {
                        r.Requerimiento = value;
                    }
                }
            }
        }
        #endregion

        #endregion

        #region metodos
        public void Agregar(params Registro[] obj)
        {
            _registros.Agregar(obj);
            foreach (Registro r in obj)
            {
                r.BaseDeDatos = _baseDeDatos;
            }
        }

        #region buscarTransitividad
        private Recipiente<Relacion> buscarTransitividad(Registro origen, Registro destino)
        {
            Recipiente<Relacion> relaciones = new Recipiente<Relacion>();
            Relacion relacion = null;
            bool terminar = false;
            // itera sobre el los registros
            //foreach(Registro r in this)
            foreach (Registro r in _registros)
            {

                // condición de termino
                if (terminar) break;

                if (origen.Id.Equals(r.Id))
                {
                    break;
                }
                if (destino.Id.Equals(r.Id))
                {
                    break;
                }

                // existe rel directa entre origen y el iesimo elemento del catalogo
                if ((relacion = existeRelacion(origen, r)) == null) break;

                // agrega la rel
                relaciones.Agregar(relacion);

                //ubica el origen en la rel y continua buscando 
                /*> if (_registros[relacion.Madre.Id] == origen) */
                if (_registros[relacion.Madre.Id] == origen)
                /*>relaciones.Agregar(ruta(_registros[relacion.Hijo.Id], destino));*/
                {
                    relaciones.Agregar(ruta(_registros[relacion.Hijo.Id], destino));
                }

                // comprueba si llegó a destino
                if (existeTransitividad(origen, destino, relaciones))
                // se encontró la ruta, terminar la iteracion
                {
                    terminar = true;
                }
                else
                // la ruta elegida no llega a destino así las eliminar
                {
                    relaciones.Borrar();
                }
            }
            return relaciones;
        }
        #endregion

        #region estanRelacionados
        private static Relacion existeRelacion(Registro origen, Registro destino)
        {
            Relacion relacion = null;

            if (origen.Relaciones != null)
            // ve la relaciones de origen a destino
            {
                foreach (Relacion r in origen.Relaciones)
                {
                    // verifica si las relaciones 
                    if ((r.Madre.Id.Equals(origen.Id)) && (r.Hijo.Id.Equals(destino.Id)))
                    {
                        relacion = r;
                        break;
                    }
                }
            }
            return relacion;
        }
        #endregion

        #region entradaUsuario
        /// <summary>
        /// lo que el cliente selecciona en el modelo (campos)
        /// </summary>
        /// <param name="request">coleccion campo valor</param>
        /// <returns>contenedor de campos</returns>
        public Recipiente<Campo> EntradaUsuario(NameValueCollection request)
        {
            Recipiente<Campo> ret = new Recipiente<Campo>();
            // debe devolver un recipiente de campos
            foreach (Registro registro in _registros)
            //foreach(Registro registro in this)
            {
                // se detiene cuando el catalogo retorna los campos indicados en el request
                if (ret.Largo == request.Count)
                {
                    break;
                }
                ret.Agregar(registro.EntradaUsuario(request));
            }
            return ret;
        }
        /// <summary>
        /// campos seleccioandos por el cliente previemente
        /// </summary>
        /// <returns></returns>
        public Recipiente<Campo> EntradaUsuario()
        {
            return EntradaUsuario(Requerimiento.Campos);
        }
        #endregion

        #region existeTransitividad
        private static bool existeTransitividad(Registro origen, Registro destino, Recipiente<Relacion> relaciones)
        {
            bool existeOrigen = false;
            bool existeDestino = false;
            //Relacion relacion;

            //for (int i = 0; i < relaciones.Largo; i++)
            foreach (Relacion relacion in relaciones)
            {
                //relacion = relaciones[i];
                if (relacion.Madre.Id.Equals(origen))
                {
                    existeOrigen = true;
                    break;
                }
                if (relacion.Hijo.Id.Equals(origen.Id))
                {
                    existeOrigen = true;
                    break;
                }

            }
            //for (int i = 0; i < relaciones.Largo; i++)
            foreach (Relacion relacion in relaciones)
            {
                //relacion = relaciones[i];
                if (relacion.Madre.Id.Equals(destino.Id))
                {
                    existeDestino = true;
                    break;
                }
                if (relacion.Hijo.Id.Equals(destino))
                {
                    existeDestino = true;
                    break;
                }
            }
            return existeOrigen && existeDestino;
        }
        #endregion

        #region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Ejecutar(Evento.AntesDeSerializarBinario);
            // invoca a GetObjectData de la base
            base.GetObjectData(info, context);
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            // agrega la clase del motor de base de datos
            info.AddValue(serie.Valor(), _baseDeDatos.Clase);
            // invoca a GetObjectData de la base de datos
            _baseDeDatos.GetObjectData(info, context);
            _baseDeDatos.Atributos = this.Atributos;
            // verifica si hay propiedades
            if (_propiedades != null)
            {
                info.AddValue(serie.Valor(), true, typeof(bool));
                _propiedades.GetObjectData(info, context);
            }
            else
                info.AddValue(serie.Valor(), false, typeof(bool));

            if (_registros != null)
            {
                info.AddValue(serie.Valor(), true, typeof(bool));
                _registros.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false, typeof(bool));
            }

            if (_funcBD != null)
            {
                info.AddValue(serie.Valor(), true, typeof(bool));
                _funcBD.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false, typeof(bool));
            }

#if BDMD
                    if (_dimensiones != null)
                    {
                        info.AddValue(serie.Valor(), true, typeof(bool));
                        _dimensiones.GetObjectData(info, context);
                    }
                    else
                        info.AddValue(serie.Valor(), false, typeof(bool));

                    if (_cubos != null)
                    {
                        info.AddValue(serie.Valor(), true, typeof(bool));
                        _cubos.GetObjectData(info, context);
                        /*> inicializar Dimienciones-> registros -> campos, Cubos->Dimensiones usadas */
                    }
                    else
                        info.AddValue(serie.Valor(), false, typeof(bool));
#endif
        }
        #endregion

        #region Modelo
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        public static Catalogo Modelo(string strCatatogo, string strSubModelo, HttpRequest request = null)
        {

            string nomArchBin = Constructor.arCacheBin(string.Concat(strCatatogo, "?m=", strSubModelo));

            string rutaArchBin = Path.Combine(string.Concat(Constructor.Directorio, Lenguaje.CarpetaCache, Lenguaje.SepDir), nomArchBin);

            if (File.Exists(rutaArchBin))
            {
                return Constructor.desBin<Catalogo>(rutaArchBin);
            }

            Catalogo catModelo = Constructor.Crear<Catalogo>(strCatatogo, request);

            // parsea el json con la definicion del modelo
            Arreglo lstEntidades = Arreglo.Ref;
            Compilador<Arreglo>.Crear(ref lstEntidades, strSubModelo);
            // listas de objetos a eliminar     
            System.Collections.Generic.IList<string> eliminaRelacion = new System.Collections.Generic.List<string>();
            System.Collections.Generic.IList<string> registrosAEliminar = new System.Collections.Generic.List<string>();

            // determina cuales tablas va a eliminar
            foreach (Registro registro in catModelo.Registros)
            {
                if (!lstEntidades.Contains(registro.Id))
                {
                    registrosAEliminar.Add(registro.Id);
                }
            }


            // ejecuta la eliminacion de tablas
            foreach (string reg in registrosAEliminar)
                catModelo.Registros.Eliminar(reg);

            // elimina las Relaciones que no se ocuparan
            foreach (Registro registro in catModelo)
            {
                if (registro.Relaciones != null)
                {
                    foreach (Relacion relacion in registro.Relaciones)
                    {
                        if (!catModelo.Registros.Existe(relacion.Hijo.Id))
                        {
                            eliminaRelacion.Add(registro.Id + "." + relacion.Id);
                        }
                    }
                }
            }

            // ejecuta la eliminacion de las Relaciones
            string[] ids;
            foreach (string s in eliminaRelacion)
            {
                ids = s.Split('.');
                catModelo[ids[0]].Relaciones.Eliminar(ids[1]);
            }
            return Constructor.Binarizar(catModelo, rutaArchBin);
        }
        #endregion

        #region origenes
        /// <summary>
        /// <c>origenes : </c> retorna los origenes involucrados junto con la 
        /// transitividad involucradas en los campos
        /// </summary>
        /// <mr name="campos"></mr>
        /// <returns></returns>
        public Recipiente<Registro> Origenes(params Recipiente<Campo>[] campos)
        {
            Recipiente<Registro> origenes = new Recipiente<Registro>();
            _transitividad = new Recipiente<Relacion>();

            // obtiene todos los origenes segun la especificacion de campos
            Recipiente<Campo> rc;
            for (int i = 0; i < campos.Length; i++)
            {
                rc = campos[i];
                if (rc != null)
                {
                    for (int j = 0; j < rc.Largo; j++)
                    {
                        origenes.Agregar((Registro)rc[j].Padre);
                    }
                }
            }
            Recipiente<Relacion> rutaEntreDosOrigenes;
            int totalRegistros = origenes.Largo;
            for (int i = 0; i < totalRegistros - 1; i++)
            {
                for (int j = i + 1; j < totalRegistros; j++)
                {  // obtiene la ruta entre tablas, comprueba en ambos sentidos
                    rutaEntreDosOrigenes = ruta(origenes[i], origenes[j]);
                    if (rutaEntreDosOrigenes.Largo > 0)
                    {
                        _transitividad.Agregar(rutaEntreDosOrigenes);
                    }
                    else
                    {
                        _transitividad.Agregar(ruta(origenes[j], origenes[i]));
                    }
                }
            }

            // agrega los posibles nuevos origenes de acuerdo a las relaciones
            //for (int i = 0; i < _transitividad.Largo; i++)
            foreach (Relacion r in _transitividad.Contenido)
            {
                origenes.Agregar(_registros[r.Madre.Id]);
                origenes.Agregar(_registros[r.Hijo.Id]);
            }
            return origenes;
        }
        #endregion

        #region restaurar
        /// <summary>
        /// <c>restaurar : </c> vuelve el objeto al estado inicial.
        /// </summary>  

        public override void Restaurar()
        {
#if RuntimeCache
                    base.Restaurar();
                    _registros.Restaurar();
                    _requerimiento = null;
#endif
            BaseDeDatos.Restaurar();
        }
        #endregion

        #region ruta
        private Recipiente<Relacion> ruta(Registro origen, Registro destino)
        {
            Recipiente<Relacion> relaciones = new Recipiente<Relacion>();
            Relacion relacion = existeRelacion(origen, destino);
            if (relacion != null)
            {
                relaciones.Agregar(relacion);
                return relaciones;
            }
            return buscarTransitividad(origen, destino);
            //return buscarTransitividad(destino, origen);
        }
        #endregion

        #region transitividad
        /// <summary>
        /// devuelve la transitividad actual
        /// </summary>
        public Recipiente<Relacion> Transitividad
        {
            get { return _transitividad; }
        }
        #endregion

        #region lectorXMLCatalogo
        /// <summary>
        /// Genera el objeto desde su representación XML.
        /// </summary>
        /// <mr name="reader">Tipo que permite leer el Archivo xml</mr>
        void lectorXMLCatalogo(System.Xml.XmlReader reader)
        {
            string motor = reader.GetAttribute(Lenguaje.Motor);
            string conexion = reader.GetAttribute(Lenguaje.Conexion);

            //if (!string.IsNullOrEmpty(conexion))
            //{
            _baseDeDatos = (BaseDatos)Constructor.Embriones[motor].Germinar();
            _baseDeDatos.Driver = conexion;
            _baseDeDatos.catalogo = this;
            _baseDeDatos.Atributos = this.Atributos;
            //}
            reader.Read();
            if (reader.Name.Equals(Lenguaje.Propiedades))
            {
                _propiedades = new Propiedades();
                _propiedades.lectorXMLPropiedades(reader);
                reader.Read();
            }
            if (reader.Name.Equals(Lenguaje.Registros))
            {
                _registros.LectorXMLRecipiente(reader);
                foreach (Registro r in _registros.Contenido)
                {
                    r.BaseDeDatos = _baseDeDatos;
                }
            }
            if (reader.Name.Equals(Lenguaje.Funciones))
            {
                _funcBD.LectorXMLRecipiente(reader);
            }
#if BDMD
                    if (reader.Name.Equals(Lenguaje.Dimensiones))
                    {
                        Dimensiones = new Recipiente<Dimension>();
                        _dimensiones.lectorXMLRecipiente(reader);
                    }
                    if (reader.Name.Equals(Lenguaje.Cubos))
                    {
                        Cubos = new Recipiente<Cubo>();
                        _cubos.lectorXMLRecipiente(reader);
                        /*> inicializar Dimienciones-> registros -> campos, Cubos->Dimensiones usadas */
                    }
#endif
        }
        #endregion

        #region escritorXmlCatalogo
        /// <summary>
        /// Convierte el objecto a su representación XML.
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorXmlCatalogo(System.Xml.XmlWriter writer)
        {
            if (BaseDeDatos != null)
            {
                writer.WriteAttributeString(Lenguaje.Motor, BaseDeDatos.Clase);
                writer.WriteAttributeString(Lenguaje.Conexion, BaseDeDatos.Driver);
            }
            if (_propiedades != null)
            {
                _propiedades.escritorXMLPropiedades(writer);
            }

            if (_registros != null)
            {
                if (_registros.Largo > 0)
                {
                    _registros.WriteXml(writer);
                }
            }


            if (_funcBD != null)
            {
                if (_funcBD.Largo > 0)
                {
                    _funcBD.WriteXml(writer);
                }
            }

#if BDMD
                    if (_dimensiones != null)
                    {
                        _dimensiones.WriteXml(writer);
                        if (_cubos != null)
                            _cubos.WriteXml(writer); 
                    }
#endif
        }
        #endregion
        #endregion
    }
}