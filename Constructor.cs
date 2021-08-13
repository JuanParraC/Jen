// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Collections.Concurrent;
    using System.Text;
    using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
    using CacheItemPriority = System.Web.Caching.CacheItemPriority;
    using CredentialCache = System.Net.CredentialCache;
    using DateTime = System.DateTime;
    using File = System.IO.File;
    using FileAccess = System.IO.FileAccess;
    using FileMode = System.IO.FileMode;
    using FileShare = System.IO.FileShare;
    using FileStream = System.IO.FileStream;
    using Formatting = System.Xml.Formatting;
#if AplicacionWeb
    using HttpContext = System.Web.HttpContext;
    using HttpRuntime = System.Web.HttpRuntime;
    using System.Net;
#endif
    using Path = System.IO.Path;
    using Stream = System.IO.Stream;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using StreamingContextStates = System.Runtime.Serialization.StreamingContextStates;
    using UTF8Encoding = System.Text.UTF8Encoding;
    using WebRequest = System.Net.WebRequest;
    using WebResponse = System.Net.WebResponse;
    using XmlSerializer = System.Xml.Serialization.XmlSerializer;
    using System.Web;
    using System;
    using System.Net.Security;
#if LOGGER
    using NLog;
    using System.Collections.Specialized;
#endif

    /// <summary>
    ///Constructor : Clase que permite la creacion de objetos Jen desde una definicion Xml, 
    /// binariza y hostea objetos en runtimecache para su reutilizacion en ejecuciones sucesivas
    /// </summary>
    public static class Constructor
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        #region campos

#if AplicacionWeb
        //private static string claveRuntimeCache = Util.Configuracion(Lenguaje.ClaveRuntime);
#endif
        // ruta base de directorio de sistema
        private static string _directorio = Util.Configuracion(Lenguaje.Directorio);
        // ruta http base del sistema
        private static string _url = Util.Configuracion(Lenguaje.URL);
        // indicador de binarizacion de objetos
        private static bool _binarizar = true;
        // lista de archivos binarizandose error de choque en escritura
        private static System.Collections.Generic.IList<string> escrituraDeArchivos = new System.Collections.Generic.List<string>();
#if RuntimeCache
        // numero de objetos por clase/tipo hosteados en runtimecache
        private static int MaxNumObjInMem = int.Parse(Util.Configuracion(Lenguaje.MaxNumObjInMem));
        // permite el acceso aleatoreo 
        private static System.Random indAccRnd = new System.Random();
#endif
        // lista de embriones de objetos Jen
        static Recipiente<Embrion> _embriones = CrearExtensiones();
        //static Recipiente<Embrion> _embriones = new Recipiente<Embrion>();

        /// para sincronizar el acceso de los objetos concurrentes
        //static int sincObjConc = 0;



#if AplicacionEscritorio
			// semillas usadas en el proceso
			private static System.Collections.Generic.IList<ISemilla> semillas = 
                new System.Collections.Generic.List<ISemilla>();
#endif
        #endregion
        static Constructor()
        {
            _embriones.NombreContenido = "Embriones";
        }

        #region propiedades
        #region propiedad Formato
        //declara el origen _usuario privado para la propiedad
        private static Formatting _formato = Formatting.Indented;

        /// <summary>
        /// Formato del archivo xml
        /// </summary>  
        public static Formatting Formato
        {
            get
            {
                return _formato;

            }
            set
            {
                _formato = value;
            }
        }
        #endregion

        #region propiedad Binarizar
        /// <summary>
        /// Bin indica si binariza los objetos a instanciar
        /// </summary>  
        public static bool Bin
        {
            get
            {
                return _binarizar;

            }
            set
            {
                _binarizar = value;
            }
        }
        #endregion

        #region propiedad Directorio


        /// <summary>
        /// Directorio base del sistema
        /// </summary>  
        public static string Directorio
        {
            get
            {
                return _directorio;

            }
            set
            {
                _directorio = value;
            }
        }
        #endregion

        #region propiedad URL
        /// <summary>
        /// URL base del sistema
        /// </summary>  
        public static string URL
        {
            get
            {
                return _url;

            }
            set
            {
                _url = value;
            }
        }
        #endregion

        #region propiedad Embriones


        /// <summary>
        ///     <c>Embriones : </c> lista de objetos embrion, los que permiten la germinación de objetos de un tipo Semilla
        /// </summary>
        public static Recipiente<Embrion> Embriones
        {
            get
            {
                return _embriones;
            }
        }
        #endregion
        #endregion
        #region metodos

        #region BorrarCache
        public static void BorrarCache(string obj, HttpRequest request = null)
        {
            string clave = arCacheBin(obj);
            string cacheBinario = Path.Combine(string.Concat(_directorio, Lenguaje.CarpetaCache, Lenguaje.SepDir), clave);
            if (File.Exists(cacheBinario))
            {
                File.Delete(cacheBinario);
                Cache.Usuario cache = CacheUsuario(request);
                cache.Clear();
                //HttpRuntime.Cache
            }
        }
        #endregion
        #region crearCacheUsuario

        public static Cache.Usuario CacheUsuario(HttpRequest request = null)
        {

            Cache.Usuario cache;
            //string sesionUsuario;
            string cacheId = "cache";

            if (request != null)
            {
                if (!string.IsNullOrEmpty(request["cache"]))
                {
                    cacheId = request["cache"];
                }
            }
            // accede al sistema de memoria
            object oCacheUsuario = HttpRuntime.Cache.Get(cacheId);
            if (oCacheUsuario != null)
            {
                // si existe lo direcciona
                cache = oCacheUsuario as Cache.Usuario;
            }
            else
            {
                cache = new Cache.Usuario();
                int minSesion = int.Parse(Util.Configuracion("UsuarioSesion"));
                // registra el objeto en el runtimecache                          
                HttpRuntime.Cache.Insert(cacheId, cache, null, DateTime.Now.AddMinutes(minSesion), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);

#if LogConstructor
                    HttpRuntime.Cache.Insert("LogConstructor", new Recipiente<Semilla>(), null, DateTime.Now.AddDays(2), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
#endif
#if LogURL
                    HttpRuntime.Cache.Insert("LogURL", new Recipiente<Dato>(), null, DateTime.Now.AddDays(2), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
#endif
            }
            return cache;

        }
        #endregion
        #region crearCache
        private static T crearCache<T>(T objeto, HttpRequest request = null)
            where T : Semilla
        {
            string clave = objeto.Tag;
            Cache.Usuario cache = CacheUsuario(request);
            ConcurrentBag<Semilla> objetosConcurrentes;

            if (!cache.ContainsKey(clave))
            {
                objetosConcurrentes = new ConcurrentBag<Semilla>();
                cache.Add(clave, objetosConcurrentes);
                objetosConcurrentes.Add(objeto);
            }
            else
            {
                objetosConcurrentes = cache[clave];
                objetosConcurrentes.Add(objeto);
            }
            /*
                    if (!cache.ContainsKey(clave))
                    {
                        if (Interlocked.Exchange(ref sincObjConc, 1) == 0)
                        {
                            objetosConcurrentes = new ConcurrentBag<ISemilla>();
                            cache.Add(clave, objetosConcurrentes);
                            objetosConcurrentes.Add(objeto);
                            Interlocked.Exchange(ref sincObjConc, 0);
                        }
                    }
                    else
                    {
                        objetosConcurrentes = cache[clave];
                        objetosConcurrentes.Add(objeto);
                    }
                */
            //objeto.Tag = clave;
            return objeto;
        }
        #endregion

        #region crearExtensiones
        public static Recipiente<Embrion> CrearExtensiones()
        {
            _embriones = new Recipiente<Embrion>();
            Embrion em = new Embrion();
            em.Tipo = typeof(Embrion).AssemblyQualifiedName;
            _embriones.Agregar(em);

            Recipiente<Embrion> extensiones = crear<Recipiente<Embrion>>(Lenguaje.Clases);
            extensiones.Agregar(em);

            em = new Embrion();
            extensiones.Clase = em.Id = "RecipienteEmbrion";
            em.Tipo = typeof(Recipiente<Embrion>).AssemblyQualifiedName;
            extensiones.Agregar(em);

            return extensiones;
        }
        #endregion
        #region desBin
        internal static T desBin<T>(string archBin)
        {
            // declara la instancia a devolver
            T instancia;
            //declara la serie para el proceso de serializacion del objeto
            Serie serie = new Serie();
            // instancia los objetos para la 
            StreamingContext sc = new StreamingContext(StreamingContextStates.All, serie);
            BinaryFormatter serializardor = new BinaryFormatter(null, sc);
            using (Stream datosStream = new FileStream(archBin, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                instancia = (T)serializardor.Deserialize(datosStream);
            }
            return instancia;
        }
        #endregion
        #region Crear
        /// <summary>
        /// Instancia el objeto de tipo T que reside en el archivo xml
        /// Ej. Campo c = Crea-Campo-("Id.xml");
        /// </summary>
        /// <typeparam name="T">Tipo del objeto</typeparam>
        /// <param name="archivo">archivo que contiene la definición del objeto</param>
        /// <returns>La instancia del objeto</returns>
        public static T Crear<T>(params object[] par)
            where T : Semilla
        {
            string archivo = par[0].ToString();
            string hash = string.Empty;
            string url;
            string archBin = string.Empty;
            string cacheBinario = string.Empty;
            HttpRequest request = null;

#if AplicacionWeb
            url = archivo;
#else
            url = ProcHttpRequest(archivo);
#endif
            if ((par.Length > 1) && (par[1] != null))
            {
                if (par[1].GetType() == typeof(string))
                {
                    hash = par[1].ToString();
                    archBin = hash;
                    cacheBinario = Path.Combine(string.Concat(_directorio, Lenguaje.CarpetaCache, Lenguaje.SepDir), hash);

                }
                else if (par[1].GetType() == typeof(HttpRequest))
                {
                    request = (HttpRequest)par[1];
                    url = ProcHttpRequest(archivo, request);
                    archBin = arCacheBin(archivo, request);
                    cacheBinario = Path.Combine(string.Concat(_directorio, Lenguaje.CarpetaCache, Lenguaje.SepDir), archBin);
                }
            }
            else
            {
                // crea una ruta sin violar la unicidad de nombres de archivo
#if DEBUG
                archBin = arCacheBin(archivo);
#else
                archBin = Jen.Util.MD5(archivo.ToUpper(Util.Cultura));
#endif
                cacheBinario = Path.Combine(string.Concat(_directorio, Lenguaje.CarpetaCache, Lenguaje.SepDir), archBin);
            }


#if !RuntimeCache
            T obj;
            // consulta si el cache binario existe
            if (Bin && File.Exists(cacheBinario))
            {
                // se esta escribiendo el archivo binario
                if (escrituraDeArchivos.Contains(cacheBinario))
                {
                    obj = desXml<T>(url, request);
                    obj.Request = request;
                    return obj;
                }
                obj = desBin<T>(cacheBinario);
                obj.Request = request;
                return obj;
            }
            obj = desXml<T>(url, request);

            //obj = desXml<T>(string.Concat(_url, Lenguaje.CarpetaXml, Lenguaje.Slash, archivo), request);
            obj.Request = request;
            return Binarizar(obj, cacheBinario);
#else
            if (request == null)
            {
                // consulta si el cache binario existe
                if (Bin && File.Exists(cacheBinario))
                {
                    // se esta escribiendo el archivo binario
                    if (escrituraDeArchivos.Contains(cacheBinario))
                        return desXml<T>(url);

                    return desBin<T>(cacheBinario);
                }
                //crear desde la imagen xml 
                return Binarizar(desXml<T>(url), cacheBinario);

            }

            Cache.Usuario cache = CacheUsuario(request);
#if LogConstructor
                    // registra todas las llamadas al constructor
                    Recipiente<Semilla> RS = (Recipiente<Semilla>)HttpRuntime.Cache.Get("LogConstructor");
                    string idSemilla = string.Concat("&#60;", typeof(T).Name, "&#62;(\"", archivo, "\");");
                    if (RS != null)
                        if (!RS.Existe(idSemilla))
                            RS.Agregar(new Semilla() { Id = idSemilla });
#endif
#if LogURL
                    Recipiente<Dato> RD = (Recipiente<Dato>)HttpRuntime.Cache.Get("LogURL");
                    string idURL = HttpContext.Current.Request.Url.AbsoluteUri;
                    if (RD != null)
                        if (!RD.Existe(idURL))
                        {
                            Dato d = new Dato();
                            d.Id = idURL;
                            if (HttpContext.Current.Request.HttpMethod.Equals("POST"))
                            {
                                d.Valor = HttpContext.Current.Request.Form.ToString();
                            }
                            RD.Agregar(d);
                        }                    
#endif
            T obj;
            if (cache.ContainsKey(archBin))
            {
                ConcurrentBag<Semilla> objetos = cache[archBin];
                Semilla ret;
                if ((objetos.TryTake(out ret)))
                {

                    if ((obj = (T)ret) != null)
                    {
                        if (!Semilla.En(ret.Estado, Estado.Ocupado))
                        {
                            ret.Request = request;
                            return InformarRestauracion(obj, obj.Tag);
                        }
                    }

                }
                /*
                        int cantidadObjetos = objetos.Count;
                        if (objetos.Count >= MaxNumObjInMem)
                        {
                            int inicioCiclo = indAccRnd.Next(cantidadObjetos);
                            int finCiclo = objetos.Count;
                            int iesimo = inicioCiclo;
                            int iteraciones = 0;
                            //itera dentro del conjunto de objetos con inicio randomico hasta el fin del conjunto
                            //para luego retomar desde el inicio
                            do
                            {
                                // aceede al objeto
                                obj = (T)objetos[iesimo++];
                                if (Semilla.Sincronice(obj))
                                    return InformarRestauracion(obj);
                                //controla la cantidad de iteraciones
                                if (++iteraciones >= cantidadObjetos)
                                    break;
                                //restablece el inicio y fin
                                if (iesimo == finCiclo)
                                {
                                    iesimo = 0;
                                    finCiclo = inicioCiclo;
                                }
                            } while (true);
                    
                        }*/
            }
            // consulta si el cache binario existe
            if (File.Exists(cacheBinario))
            {
                // se esta escribiendo el archivo binario asi que lee de la imagen xml
                if (escrituraDeArchivos.Contains(cacheBinario))
                {
                    obj = desXml<T>(url, request);
                    obj.Request = request;
                    if (Semilla.sincro(obj))
                    {
                        return crearCache(InformarRestauracion(obj, archBin, request), request);
                    }
                }
                obj = desBin<T>(cacheBinario);
                obj.Request = request;
                if (Semilla.sincro(obj))
                {
                    return crearCache(InformarRestauracion(obj, archBin, request), request);
                }
            }
            //crear desde la imagen xml
            obj = desXml<T>(url, request);
            obj.Request = request;
            Binarizar(obj, cacheBinario);
            Semilla.sincro(obj);
            return crearCache(InformarRestauracion(obj, archBin, request), request);
#endif
        }
        /// <summary>
        /// Permite crear objetos accediendo via SO no pasando por el servidor web, con objeto de eliminar recursion
        /// al instanciar objetos internos como las clases y formatos sql, estos objetos no son hosteados en memoria
        /// </summary>
        /// <typeparam name="T">tipo de objeto</typeparam>
        /// <param name="archivo">archivo que contiene el objeto</param>
        /// <returns></returns>
        public static T crear<T>(string archivo)
        {
#if AplicacionWeb
            string ruta = string.Concat(string.Concat(_directorio, Lenguaje.CarpetaXml, Lenguaje.SepDir), archivo);
#else
			string ruta = string.Concat(string.Concat(_directorio, Lenguaje.CarpetaXml, Lenguaje.SepDir), archivo);
#endif
            string cacheBinario = Path.Combine(string.Concat(_directorio, Lenguaje.CarpetaCache, Lenguaje.SepDir), arCacheBin(archivo));
            // consulta si el cache binario existe y no se está escribiendo
            if (File.Exists(cacheBinario) && !escrituraDeArchivos.Contains(cacheBinario))
            {
                return desBin<T>(cacheBinario);
            }
            return Binarizar(desXml<T>(ruta), cacheBinario);
        }
        #endregion
        #region desXml
        private static T desXml<T>(string archivo, HttpRequest request = null)
        {
            T instancia;
            string ruta = archivo;
            XmlSerializer serializardor = new XmlSerializer(typeof(T));
#if TRACE
            _logger.Debug(string.Concat("Get  ", ruta));
#endif   
#if AplicacionWeb
            string[] header = null; // { "Content-type", "application/xml" };
            using (WebResponse imagenXml = RespuestaHttp(ruta, "GET", string.Empty, header, request))
#else
            using (WebResponse imagenXml = RespuestaHttp(ruta, "GET", string.Empty))
#endif
            {
                //obtiene el stream que apunta a la definicion del objeto
                using (Stream stream = imagenXml.GetResponseStream())
                {
                    // deserializar el objeto creando la instancia
                    instancia = (T)serializardor.Deserialize(stream);
                }
            }
            // retorna la instancia del objeto
            return instancia;
        }
        public static T DesXml<T>(string xml)
        {
            T instancia;
            //string ruta = archivo;
            XmlSerializer serializardor = new XmlSerializer(typeof(T));

            // obtiene la definicion del objeto en formato xml
            //using (WebResponse imagenXml = RespuestaHttp(ruta))
            // {
            //obtiene el stream que apunta a la definicion del objeto
            using (Stream stream = string2Stream(xml))
            {
                // deserializar el objeto creando la instancia
                instancia = (T)serializardor.Deserialize(stream);
            }
            // }
            // retorna la instancia del objeto
            return instancia;
        }
        private static Stream string2Stream(string s)
        {
            var stream = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        #endregion
        #region http2Unc
        public static string ProcHttpRequest(string ruta, HttpRequest request = null)
        {
            bool sqlite = false;
            bool oracle = false;
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request[Lenguaje.SQLite]))
                {
                    sqlite = bool.Parse(request[Lenguaje.SQLite]);
                }

                if (!string.IsNullOrEmpty(request[Lenguaje.Oracle]))
                {
                    oracle = bool.Parse(request[Lenguaje.Oracle]);
                }
            }

            // extension accesible via ruta UNC
            string[] separador = new string[] { ".xml", ".js", ".htm", ".html", ".txt" };
            // ocurrencia de alguna extension
            string[] arrOcurr = ruta.ToLower().Split(separador, StringSplitOptions.RemoveEmptyEntries);
            if (arrOcurr.Length > 0)
            {
                // comprueba el largo de la ocurrencia 
                if (arrOcurr[0].Length < ruta.Length)
                {
                    return string.Concat(_url, Lenguaje.CarpetaXml, Lenguaje.Slash, ruta);//, "&", Lenguaje.Oracle, "=", oracle, "&", Lenguaje.SQLite, "=", sqlite);
                }
            }
            StringBuilder sbUrl = new StringBuilder(string.Concat(_url, Lenguaje.CarpetaXml, Lenguaje.Slash, ruta));
            if (request != null)
            {
                string[] keyQString = { "plataforma", "cache" };

                foreach (string k in keyQString)
                {
                    if (!ruta.Contains(k))
                    {
                        sbUrl.Append("&");
                        sbUrl.Append(k);
                        sbUrl.Append("=");
                        sbUrl.Append(request[k]);
                    }
                }
            }
            if (oracle)
            {
                sbUrl.Append("&");
                sbUrl.Append(Lenguaje.Oracle);
                sbUrl.Append("=");
                sbUrl.Append(oracle);
            }
            if (sqlite)
            {
                sbUrl.Append("&");
                sbUrl.Append(Lenguaje.SQLite);
                sbUrl.Append("=");
                sbUrl.Append(sqlite);
            }

            return sbUrl.ToString();

        }
        #endregion
        #region informarRestauracion
        internal static T InformarRestauracion<T>(T instancia, string contenedor, HttpRequest request = null)
        where T : Semilla
        {

#if AplicacionWeb
            instancia.Tag = contenedor;
            ConcurrentBag<Semilla> semillas;
            string clave;

            if (request != null)
            {
                //ConcurrentBag<Semilla> objetosConcurrentes;
                Cache.Usuario cache = CacheUsuario(request);

                clave = HttpContext.Current.CurrentHandler.GetHashCode().ToString();
                //clave = request["ASP.NET_SessionId"];

                instancia.Tag = clave;
                if (!cache.ContainsKey(clave))
                {
                    semillas = new ConcurrentBag<Semilla>();
                    cache.Add(clave, semillas);
                }
                else
                {
                    semillas = cache[clave];
                }

                semillas.Add(instancia);
            }
            else if (HttpContext.Current.Session != null)
            {
                clave = HttpContext.Current.Session.SessionID.ToString();

                if (request != null)
                {
                    clave = request["ASP.NET_SessionId"];
                }

                semillas = (ConcurrentBag<Semilla>)HttpContext.Current.Session[clave];
                if (semillas == null)
                {
                    semillas = new ConcurrentBag<Semilla>();
                    HttpContext.Current.Session[clave] = semillas;
                }
                semillas.Add(instancia);
            }
            else
            {
                throw new ArgumentException("Constructor 675", "HttpContext.Current.Session nulo");
            }
#endif

            return instancia;
        }
        #endregion
        #region arCacheBin
        internal static string arCacheBin(string archivo, HttpRequest request = null)
        {

            string plataforma = string.Empty;
            if ((request != null) && (!string.IsNullOrEmpty(request["plataforma"])))
            {
                plataforma = request["plataforma"] + "_";
            }
            archivo = archivo.Replace("Web.App?", string.Empty);
            NameValueCollection nvc = HttpUtility.ParseQueryString(archivo);
            if (nvc.AllKeys[0] != null)
            {
                archivo = string.Empty;
                foreach (string nv in nvc)
                {
                    if (!(nv.ToLower().Equals("appid")))
                    {
                        archivo += nvc[nv];
                    }
                }
            }
            string[] blancos = new string[] { "/", "xml", " ", ",", ".", "'", "[", "]", "=", "&", "?" };
            foreach (string b in blancos)
            {
                archivo = archivo.Replace(b, string.Empty);
            }
            return Util.MD5(string.Concat(plataforma, archivo));
        }
        #endregion
        #region respuestaServidor
        public static WebResponse RespuestaHttp(string URL, string metodo = "GET", string postData = "",
                                                string[] header = null, HttpRequest request = null, 
                                                string contentType = "application/x-www-form-urlencoded", bool preAuth = false)
        {
            // establece la petición para la objetoXML.
            WebRequest peticionWeb = WebRequest.Create(URL);
            peticionWeb.PreAuthenticate = preAuth;
            peticionWeb.Method = metodo;
            if (request != null)
            {
                StringBuilder cook = new StringBuilder();
                bool hayCookie = false;
                foreach (string c in request.Cookies.AllKeys)
                {
                    cook.Append(c);
                    cook.Append("=");
                    cook.Append(request.Cookies[c].Value);
                    cook.Append(";");
                    hayCookie = true;
                }
                if (hayCookie)
                {
                    cook.Length--;
                    peticionWeb.Headers["cookie"] = cook.ToString();
                }
            }

            // establece los header de la peticion
            if (header != null)
            {
                for (int i = 0; i < ((int)header.Length / 2); i++)
                {
                    peticionWeb.Headers.Add(header[i], header[i + 1]);
                }
            }
            else
            {
                // si en requerido por el servidor, setea las credenciales por defecto
                peticionWeb.Credentials = CredentialCache.DefaultCredentials;
            }
            if (metodo.Equals("POST"))
            {
                peticionWeb.ContentType = contentType;

                var data = Encoding.ASCII.GetBytes(postData);
                peticionWeb.ContentLength = data.Length;
                using (var stream = peticionWeb.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            WebResponse servidor = peticionWeb.GetResponse();
            // retorna la respuesta web
            return servidor;
        }
        #endregion
        #region Restaurar
        public static void Restaurar(HttpRequest request = null)
        {
#if AplicacionWeb && RuntimeCache

            if (request != null)
            {
                //clave = HttpContext.Current.CurrentHandler.GetHashCode().ToString();
                string clave = HttpContext.Current.CurrentHandler.GetHashCode().ToString();
#if TRACE
                _logger.Debug(string.Concat("ASP.NET_SessionId ", clave));
#endif
                Cache.Usuario cache = CacheUsuario(request);

                ConcurrentBag<Semilla> objetosConcurrentes;

                if (cache.ContainsKey(clave))
                {
                    objetosConcurrentes = cache[clave];
                    foreach (Semilla s in objetosConcurrentes)
                    {
                        s.Restaurar();
                    }

                }
                return;
            }

            if (HttpContext.Current.Session != null)
            {
                string sesion = HttpContext.Current.Session.SessionID.ToString();
#if TRACE
                _logger.Debug(string.Concat("Session ", sesion));
#endif
                // direcciona a la coleccion de semillas por restaurar
                List<Semilla> semillas = HttpContext.Current.Session[sesion] as List<Semilla>;
                // restaura todas las semillas ocupadas en el proceso
                if (semillas != null)
                {
#if TRACE
                    _logger.Debug(string.Concat("Hay ", semillas.Count, "objetos"));
#endif
                    Cache.Usuario cache = CacheUsuario(request);
                    foreach (Semilla semilla in semillas)
                    {
                        if (Semilla.En(semilla.Estado, Estado.Ocupado))
                        {
#if TRACE
                            _logger.Debug(string.Concat("Restaurar ", semilla.Id));
#endif
                            semilla.Restaurar();

                            /*
                                    if (cache.ContainsKey(semilla.Tag))
                                    {
                                        ConcurrentBag<Semilla> objetos = cache[semilla.Tag];
                                        objetos.Add(semilla);
                                    }
                            */
                        }
                    }
                    // elimina lo usado en el handler 
                    semillas.Clear();
                }
            }
            else
            {
                string msh = "ohh";
            }
#endif
#if AplicacionEscritorio && RuntimeCache
                    // restaura todas las semillas ocupadas en el proceso
					foreach (ISemilla semilla in semillas)
                        if (Semilla.En(semilla.Estado, Estado.Ocupado))
                            semilla.Restaurar();
                    // elimina lo usado en el handler 
                    semillas.Clear();
#endif
        }
        #endregion
        #region Binarizar
        public static T Binarizar<T>(T instancia, string archBin)
        {
#if Binarizar
            if (Bin)
            {
                Serie serie = new Serie();
                // instancia los objetos para la 
                StreamingContext sc = new StreamingContext(StreamingContextStates.All, serie);
                BinaryFormatter serializardor = new BinaryFormatter(null, sc);
                if (!escrituraDeArchivos.Contains(archBin))
                {
                    escrituraDeArchivos.Add(archBin);
                    using (Stream datosStream =
                        new FileStream(archBin, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        serializardor.Serialize(datosStream, instancia);
                    }
                    escrituraDeArchivos.Remove(archBin);
                }
            }
#endif
            return instancia;
        }
        #endregion
        #region serializar
        /// <summary>
        /// <c>Serializar : </c> crea una imagen de texto en formato xml la instancia.
        /// </summary>  
        public static string Serializar(object instacia)
        {
            // crea un codificador UTF8
            UTF8Encoding codificadorUTF8 = new UTF8Encoding();
            // retorna un string codificado en  UTF8
            return codificadorUTF8.GetString(Util.toXMLArray(instacia));
        }
        #endregion

        #endregion

    }
}