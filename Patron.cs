// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
    using TextWriter = System.IO.TextWriter;

    /// <summary>
    /// <c>Patron</c> patrón o esqueleto de codigo fuente, soporta variables  ${variable[:]@@1@...@@n@} siendo
    /// 1..n el numero de repeticiones de valores de la  variable
    /// </summary>
    [Serializable]
    public class Patron : Dato
    {

        #region constructores
        /// <summary>
        /// crea una patron pasando el contenido de este
        /// </summary>
        /// <param name="contenido"></param>
        public Patron(string contenido, string regexp = Lenguaje.RegExpVariables) : this()
        {
            Valor = contenido;
            _sPatronVariables = regexp;
            //Comprimir = false;
            CrearVariables();
        }
        /// <summary>
        /// Constructor vacio
        /// </summary>
        public Patron()
            : base()
        {
            Id = Lenguaje.Patron;
            IniPatron();
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Patron(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniPatron();
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;

            _comprimir = info.GetBoolean(serie.Valor());
            _sPatronVariables = info.GetString(serie.Valor());

            if (info.GetBoolean(serie.Valor()))
            {
                _variables = Constructor.Embriones[Lenguaje.Variables].Germinar<Variables>(info, context);
            }

            if (info.GetBoolean(serie.Valor()))
            {
                Separadores = Constructor.Embriones[Lenguaje.Separadores].Germinar<Recipiente<Dato>>(info, context);
            }

        }
        void IniPatron()
        {
            Clase = Lenguaje.Patron;
            LectorXML = lectorXMLPatron;
            EscritorXML = escritorXMLPatron;
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
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;

            info.AddValue(serie.Valor(), _comprimir);
            info.AddValue(serie.Valor(), _sPatronVariables);

            if (_variables != null)
            {
                info.AddValue(serie.Valor(), true, typeof(bool));
                _variables.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false, typeof(bool));
            }
            if (_separadores != null)
            {
                info.AddValue(serie.Valor(), true, typeof(bool));
                _separadores.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false, typeof(bool));
            }
        }
        #endregion

        #region propiedades

        #region comprimir

        private bool _comprimir = true;
        /// <summary>
        /// permite informar al patrón que debe ajustar su salida sin blancos
        /// </summary>
        /*public bool Comprimir
        {
            get { return _comprimir; }
            set { _comprimir = value; }

        }*/
        #endregion

        #region separadores
        private Recipiente<Dato> _separadores;
        /// <summary>
        /// <c>delegados : </c> 
        /// </summary>   
        public Recipiente<Dato> Separadores
        {
            get
            {
                return _separadores;
            }
            set
            {
                _separadores = value;
                _separadores.NombreContenido = Lenguaje.Separadores;
            }
        }
        #endregion

        #region patronVariables
        //  patron para parsear las variables 
        private Regex _patronVariables;// = new Regex(Lenguaje.analizadorIdentificadores, RegexOptions.Compiled);
        private string _sPatronVariables = Lenguaje.RegExpVariables;
        /// <summary>
        /// expresión regular para el parseo de las variables del documento
        /// </summary>
        public string ExpRegVars
        {
            get { return _sPatronVariables; }
            set
            {
                _sPatronVariables = value;
                //_patronVariables = new Regex(_sPatronVariables, RegexOptions.Compiled);
            }

        }
        #endregion

        #region variables
        private Variables _variables;
        /// <summary>
        /// permite informar al patrón que debe ajustar su salida para soportar 
        /// generacion de codigo com json script de bases de datos
        /// </summary>
        public Variables Variables
        {
            get { return _variables; }
        }
        #endregion
        #endregion

        #region metodos
        #region crearVariables
        /// <summary>
        /// <c>crearVariables</c> crea los identificadores definidos en la plantilla
        /// </summary>
        public Variables CrearVariables(string rex = null, int ncharIzq = 2, int ncharDer = 3)
        {
            if( !string.IsNullOrEmpty(rex))
            {
                _sPatronVariables = rex;
            }
            _patronVariables = new Regex(_sPatronVariables, RegexOptions.Compiled);

            // si esta activado la compresion elimina saltos de carros y tabuladores
#if !DEBUG
            Regex r = new Regex(Lenguaje.RegExpComprimir, RegexOptions.Compiled);
            Valor = r.Replace(Valor, Lenguaje.MayorQue + Lenguaje.MenorQue);
            Valor = Valor.Replace(Lenguaje.BackSlashN, string.Empty).
                Replace(Lenguaje.BackSlashT, string.Empty);
#endif   
            _variables = new Variables(Valor, _patronVariables, ncharIzq, ncharDer);

            return _variables;
        }
        #endregion


        #region pintar
        /// <summary>
        ///  construye la salida del patron en un TextWriter 
        /// </summary>
        /// <param name="salida">resultado del pintado</param>
        public virtual void Pintar(TextWriter salida)
        {
            Variables.Pintar(salida);
            Variables.Borrar();
        }

        /// <summary>
        /// <c>pintar</c> proceso que mescla los valores pasados desde el modelo 
        /// del procesamiento con el texto de la plantilla
        /// </summary>
        /// <returns>retorna el texto resultante </returns>
        public virtual string Pintar()
        {
            StringBuilder sb = new StringBuilder(Valor.Length * 5);
            Pintar(ref sb);
            return sb.ToString();
        }
        public void Pintar(ref StringBuilder sb)
        {
            TextWriter salida = new StringWriter(sb, Util.Cultura);
            if (_variables == null)
            {
                CrearVariables();
            }
            Pintar(salida);
        }
        #endregion
        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorXMLPatron(System.Xml.XmlWriter writer)
        {
            //writer.WriteAttributeString("Valor", valor);
            escritorXMLDato(writer);
            if (_comprimir)
            {
                writer.WriteStartAttribute(Lenguaje.Comprimir);
                writer.WriteValue(_comprimir);
                writer.WriteEndAttribute();
            }
            if (!_sPatronVariables.Equals(Lenguaje.RegExpVariables))
            {
                writer.WriteAttributeString(Lenguaje.Variables, _sPatronVariables);
            }
            // controla la escritura de los separadores 
            if (_separadores != null)
            {
                writer.WriteStartAttribute(Lenguaje.Separadores);
                writer.WriteValue(true);
                writer.WriteEndAttribute();
                _separadores.WriteXml(writer);
            }
        }
        #endregion
        void lectorXMLPatron(System.Xml.XmlReader reader)
        {
            lectorXMLDato(reader);
            string Comprimir = reader.GetAttribute(Lenguaje.Comprimir);
            if (!string.IsNullOrEmpty(Comprimir))
            {
                _comprimir = bool.Parse(Comprimir);
            }

            string Variables = reader.GetAttribute(Lenguaje.Variables);
            if (!string.IsNullOrEmpty(Variables))
            {
                ExpRegVars = Variables;
            }
            CrearVariables();

            string strSeparadores = reader.GetAttribute(Lenguaje.Separadores);
            bool tieneSeparadores = false;
            if (!string.IsNullOrEmpty(strSeparadores))
            {
                tieneSeparadores = bool.Parse(strSeparadores);
            }
            if (tieneSeparadores)
            {
                reader.Read();
                Separadores = new Recipiente<Dato>();
                Separadores.ReadXml(reader);
                if (_separadores.Largo > 0)
                {
                    foreach (Dato separador in _separadores.Contenido)
                    {
                        if (_variables.Existe(separador.Id))
                        {
                            _variables[separador.Id].Separador(separador.Valor);
                        }
                    }
                }
            }
        }

        #endregion
    }
}