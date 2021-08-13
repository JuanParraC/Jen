// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using MapStringInt = System.Collections.Generic.Dictionary<string, int>;
    using MatchCollection = System.Text.RegularExpressions.MatchCollection;
    using Regex = System.Text.RegularExpressions.Regex;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Declaraciones de tipo y tipo de valor
    /// </summary>
    /// <typeparam name="TipoValores">Tipo de variable</typeparam>
    /// <typeparam name="Tipo">Tipo de estamento</typeparam>
    [Serializable]
    public abstract class Declaraciones<TipoValores, Tipo>
        where Tipo : Declaracion, new()
    {
        /// <summary>
        /// Crea la declaración
        /// </summary>
        /// <param name="statement">codigo fuente</param>
        /// <returns></returns>
        public abstract Tipo CrearDeclaracion(string statement);
        /// <summary>
        /// Crea el tipo de valores de la declaración
        /// </summary>
        /// <param name="statement">estamento original</param>
        /// <returns></returns>
        public abstract TipoValores CrearDeclaracionTipo(Tipo statement);
        // numero total de estamentos
        int totalDeclaraciones;
        //lista de estamentos dentro del documento
        internal System.Collections.Generic.List<Tipo> ListaDeclaraciones = new System.Collections.Generic.List<Tipo>();
        internal System.Collections.Generic.List<Tipo> DeclaracionesUnicas = new System.Collections.Generic.List<Tipo>();
        /// <summary>
        /// mapa para aceder a la lista de estamentos via claves string
        /// </summary>
        MapStringInt mapa;
        /// <summary>
        /// valores que almacena el estamento
        /// </summary>
        //internal System.Collections.Generic.Dictionary<int, TipoValores> Valores = new System.Collections.Generic.Dictionary<int, TipoValores>();
        public Valores<TipoValores> Valores = new Valores<TipoValores>();
        /// <summary>
        /// Envoltorio de texto de la lista de estamentos
        /// </summary>
        //internal string[] Envoltorios;
        internal Envoltorios Envoltorios;
        /// <summary>
        /// Limpia las variables del documento
        /// </summary>
        internal abstract void Liberar(TipoValores t);
        /// <summary>
        /// Constructor, recive el codigo fuente y la expresion regular con que parsea los estamentos
        /// </summary>
        /// <param name="texto">fuente</param>
        /// <param name="er">parseador</param>
        int numCharIzq;
        int numCharDer;
        protected Declaraciones(string texto, Regex er, int nCharIzq = 2, int nCharDer = 3)
        {
            numCharIzq = nCharIzq;
            numCharDer = nCharDer;

            // crea el mapa
            mapa = new MapStringInt();
            // obtiene la declaración de las instancias en una coleccion
            MatchCollection mcDeclaraciones = er.Matches(texto);
            totalDeclaraciones = mcDeclaraciones.Count;
            string[] arrDeclaraciones = new string[totalDeclaraciones];
            string declaracion;

            for (int i = 0; i < totalDeclaraciones; i++)
            {
                declaracion = removerEnvoltorio(mcDeclaraciones[i].Value);
                Tipo ithStatement = CrearDeclaracion(declaracion);
                Tipo statementThere = ListaDeclaraciones.Find(
                    delegate(Tipo k)
                    {
                        return k.Id.Equals(ithStatement.Id);
                    }
                );

                if (statementThere != null)
                {
                    ithStatement.Posicion = statementThere.Posicion;
                }
                else
                {
                    int position;
                    // obtiene y setea la ultima posición
                    ithStatement.Posicion = position = Valores.Count;
                    // mapea el identificador con su area de datos
                    mapa.Add(ithStatement.Id, position);
                    // define el area de datos de la instancia
                    Valores.Add(position, CrearDeclaracionTipo(ithStatement));
                    // agrega el estamento a la lista
                    DeclaracionesUnicas.Add(ithStatement);
                }
                ListaDeclaraciones.Add(ithStatement);
                // almacena la iesima instancia en el arreglo
                arrDeclaraciones[i] = mcDeclaraciones[i].Value;
            }
            // controla que existan Statement para crear el envoltorio de textos
            if (arrDeclaraciones.Length > 0)
            {
                Envoltorios = new Envoltorios(texto.Split(arrDeclaraciones, System.StringSplitOptions.None));
            }
            else
            {
                Envoltorios = new Envoltorios(new string[1] { texto });
            }
        }
        public MapStringInt Mapa
        {
            get
            {
                return mapa;
            }
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Declaraciones(SerializationInfo info, StreamingContext context)
        {
            Serie serie = (Serie)context.Context;
            // obtiene el total de declaraciones
            totalDeclaraciones = info.GetInt32(serie.Valor());
            int total = info.GetInt32(serie.Valor());
            // dimensiona el mapa
            mapa = new MapStringInt(total);
            for (int i = 0; i < total; i++)
            {
                mapa.Add(info.GetString(serie.Valor()), info.GetInt32(serie.Valor()));
            }
            // obtiene el total de envoltorios
            total = info.GetInt32(serie.Valor());
            Envoltorios = new Envoltorios(new string[total]);
            for (int i = 0; i < total; i++)
            {
                Envoltorios[i] = info.GetString(serie.Valor());
            }
            // guarda la lista de declaraciones
            total = info.GetInt32(serie.Valor());
            ListaDeclaraciones = new System.Collections.Generic.List<Tipo>(total);
            string clase;
            Tipo declaracion;
            for (int i = 0; i < total; i++)
            {
                clase = info.GetString(serie.Valor());
                // crea la declaracion
                declaracion = Constructor.Embriones[clase].Germinar<Tipo>(info, context);
                // busca si existe en la lista de declaraciones
                if (ListaDeclaraciones.Find(delegate(Tipo k) { return k.Id.Equals(declaracion.Id); }) == null)
                {
                    DeclaracionesUnicas.Add(declaracion);
                    // define el area de datos de la instancia
                    Valores.Add(Valores.Count, CrearDeclaracionTipo(declaracion));
                }
                ListaDeclaraciones.Add(declaracion);
            }
        }
        /// <summary>
        /// Enlaza un delegado para que reciba cada variable por parametro
        /// </summary>
        /// <param name="accion">delegado</param>
        public void Enlazar(Proc<Tipo> accion)
        {
            foreach (Tipo k in DeclaracionesUnicas)
            {
                accion(k);
            }
        }
        /// <summary>
        /// Indexador de declaraciones
        /// </summary>
        /// <param name="iesimo"></param>
        /// <returns></returns>
        public Tipo this[int iesimo]
        {
            get
            {
                return DeclaracionesUnicas[iesimo];
            }
        }
        /// <summary>
        /// Total de elementos 
        /// </summary>
        public int Largo
        {
            get
            {
                return DeclaracionesUnicas.Count;
            }
        }
        /// <summary>
        /// Devuelve las propiedades serializadas del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serie serie = (Serie)context.Context;
            // guarda el total de declaraciones
            info.AddValue(serie.Valor(), totalDeclaraciones);
            // guarda las entradas del mapa
            info.AddValue(serie.Valor(), mapa.Count);
            foreach (string k in mapa.Keys)
            {
                info.AddValue(serie.Valor(), k);
                info.AddValue(serie.Valor(), mapa[k]);
            }
            // guarda los envoltorios
            info.AddValue(serie.Valor(), Envoltorios.Length);
            foreach (string k in Envoltorios)
            {
                info.AddValue(serie.Valor(), k);
            }
            // guarda la lista de declaraciones
            info.AddValue(serie.Valor(), ListaDeclaraciones.Count);
            foreach (Tipo k in ListaDeclaraciones)
            {
                info.AddValue(serie.Valor(), k.Clase);
                k.GetObjectData(info, context);
            }
        }
        /// <summary>
        /// Descascara el envoltorio de la variable
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        string removerEnvoltorio(string statement)
        {
            //FIXME no soporta envoltorios variables
            return statement.Substring(numCharIzq, statement.Length - numCharDer);
        }
        /// <summary>
        /// Libera memoria
        /// </summary>
        internal void Borrar()
        {
            foreach (TipoValores t in Valores.Values)
            {
                Liberar(t);
            }
        }
    }
}