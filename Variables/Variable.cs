// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using System.Security.Permissions;
    /// <summary>
    /// Representa una variable dentro de un documento de texto
    /// ejemplo 
    /// mensaje[:]<td>hola mundo @@1@</td>
    /// en donde "mensaje" es el identificador de la variable y @@1@ 
    /// es el lugar donde va el valor proporcionado en tiempo de ejecución
    /// </summary>
    [Serializable]
    public class Variable : Declaracion
    {
        // contador de instancias
        int count = 0;
        // Instancias de la variable
        Instancias instancias;

        // separador de instancias
        string _separador;

        /// <summary>
        /// Constructor de la variable
        /// </summary>
        /// <param name="Var">es el texto que define la variable</param>
        public Variable(string Var)
        {
            Clase = Lenguaje.Variable;
            string[] arrVarInst = Var.Split(new string[] { Lenguaje.SeparadorInstancias }, System.StringSplitOptions.None);
            // asigna el identificador
            Id = arrVarInst[0];
            Identificadores = Id.Split(new string[] { Lenguaje.Punto }, System.StringSplitOptions.None);
            // crea las instancias 
            instancias = new Instancias(arrVarInst[1]);
        }
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Variable()
            : base()
        {
            Clase = Lenguaje.Variable;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Variable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Clase = Lenguaje.Variable;
            instancias = Constructor.Embriones[Lenguaje.Instancias].Germinar<Instancias>(info, context);
        }
        public Instancias Instancias
        {
            get
            {
                return instancias;
            }
        }
        string[] fmtVar = { "${", "[:]@@1@}" };
        public string[] FmtVar
        {
            get
            {
                return fmtVar;
            }
            set
            {
                fmtVar = value;
            }
        }
        // establece persistenacia de la variable aun que no se halla asignado 
        // permimte generacion parcial de contenido
        public bool Persistente
        {
            get;
            set;
        }
        /// <summary>
        /// Devuelve las propiedades serializadas del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            instancias.GetObjectData(info, context);
        }
        /// <summary>
        /// permite setear un valor a la instancia especificada
        /// </summary>
        /// <param name="id">identificador de la instancia</param>
        /// <param name="value">valor de la instancia</param>
        public void Asignar(string id, string value)
        {
            instancias.Valores[instancias.Mapa[id]].Add(value);
        }
        /// <summary>
        /// Asigna valores de acuerdo a la definicion de instancias
        /// </summary>
        /// <param name="value">valor a asignar</param>
        public void Asignar(string value)
        {
            int indice = count++ % instancias.Valores.Count;
            instancias.Valores[indice].Add(value);
        }
        /// <summary>
        /// Imprime en el TextWriter el resultado de la operacion de asignación
        /// </summary>
        /// <param name="tw">Fuente donde se debe escribir</param>
        internal void Pintar(System.IO.TextWriter tw)
        {
            int totalData = instancias.Valores[0].Count - 1;
            // itera sobre el contenido de las instancias
            for (int i = 0; i < totalData; i++)
            {
                // imprime el primer envoltorio de texto de las instancias
                tw.Write(instancias.Envoltorios[0]);
                // itera sobre el envoltorio de texto de las instancias
                for (int k = 1; k < instancias.Envoltorios.Length; k++)
                {
                    // direcciona el valor atravez de las iesima instancia 
                    tw.Write(instancias.Valores[instancias.ListaDeclaraciones[k - 1].Posicion][i]);
                    // imprime el iseimo envoltorio de la instancia
                    tw.Write(instancias.Envoltorios[k]);
                }
            }
            // obtiene el ultimo texto
            string separator = instancias.Envoltorios[instancias.Envoltorios.Length - 1];
            if (!string.IsNullOrEmpty(_separador))
            {
                // limpia el ultimo texto
                string strWrapper = instancias.Envoltorios[instancias.Envoltorios.Length - 1];
                instancias.Envoltorios[instancias.Envoltorios.Length - 1] = strWrapper.Remove(strWrapper.Length - _separador.Length);
            }
            // controla la existencia de datos
            if (totalData < 0)
            {
                if (Persistente)
                {
                    foreach (int i in instancias.Valores.Keys)
                    {
                        instancias.Valores[i].Add(string.Concat(fmtVar[0],Id, fmtVar[1]));
                    }
                }
                else
                {
                    // si no hay datos agrega vacios para poder imprimir el texto entre valores
                    foreach (int i in instancias.Valores.Keys)
                    {
                        instancias.Valores[i].Add(string.Empty);
                    }
                }
                totalData = 0;
            }
            // imprime el primer envoltorio de texto de las instancias
            tw.Write(instancias.Envoltorios[0]);
            // itera sobre el envoltorio de texto de las instancias
            for (int k = 1; k < instancias.Envoltorios.Length; k++)
            {
                tw.Write(instancias.Valores[instancias.ListaDeclaraciones[k - 1].Posicion][totalData]);
                // imprime el iseimo envoltorio de la instancia
                tw.Write(instancias.Envoltorios[k]);
            }
            instancias.Envoltorios[instancias.Envoltorios.Length - 1] = separator;
        }
        /// <summary>
        /// Enumerador de las instancias de la variable
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerator<Declaracion> GetEnumerator()
        {
            foreach (Declaracion s in instancias.DeclaracionesUnicas)
            {
                yield return s;
            }
        }
        /// <summary>
        /// Indexador de instancias de la variable
        /// </summary>
        /// <param name="iesimo">item</param>
        /// <returns>iesima instancia de la variable</returns>
        public Declaracion this[int iesimo]
        {
            get
            {
                return instancias.DeclaracionesUnicas[iesimo];

            }
        }
        /// <summary>
        /// Asigna el valor retornado por los objetos graficos a las instancias de la variable
        /// </summary>
        internal void Asignar()
        {
            foreach (Declaracion s in instancias.DeclaracionesUnicas)
            {

                if (s.ObjetoGrafico != null)
                {
                    Asignar(s.Id, s.ObjetoGrafico.Pintar());
                }
                /*try
                {
                }
                catch (System.Exception e)
                {
                    e.ToString();
                }*/
            }
        }
        // auto completa la variable
        public void AutoCompletar()
        {
            int maxTVal = 0;
            // itera por la cantidad de valores de cada instancia y determina el mas alto
            foreach(string id in instancias.Mapa.Keys)
            {
                if (maxTVal < instancias.Valores[instancias.Mapa[id]].Count)
                {
                    maxTVal = instancias.Valores[instancias.Mapa[id]].Count;
                }
            }
            // autocompleta
            foreach (string id in instancias.Mapa.Keys)
            {
                if (maxTVal > instancias.Valores[instancias.Mapa[id]].Count)
                {
                    int valfal = maxTVal - instancias.Valores[instancias.Mapa[id]].Count;
                    for (int i = 1; i <= valfal; i++)
                    {

                    }
                }
            }
        }
        /// <summary>
        /// Establece el separador entre instancias de la variable
        /// </summary>
        /// <param name="sep"></param>
        public void Separador(string sep)
        {
            _separador = sep;
        }
        /// <summary>
        /// libera memoria
        /// </summary>
        internal void Borrar()
        {
            count = 0;
            instancias.Borrar();
        }
    }
}