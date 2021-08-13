// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// <c>Dato</c> Semilla con un valor de un tipo string
    /// </summary>
    [Serializable]
    public class Dato : Semilla, IValor
    {

        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Dato()
        {
            Id = Lenguaje.Dato;
            IniDato();
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Dato(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniDato();
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            _valor = info.GetString(serie.Valor());
        }
        void IniDato()
        {
            Clase = Lenguaje.Dato;
            Genero = Genero.Masculino;
            EscritorXML = escritorXMLDato;
            LectorXML = lectorXMLDato;
        }
        #endregion


        #region valor
        string _valor = string.Empty;
        /// <summary>
        /// Tipo del dato
        /// </summary>
        public string Valor
        {
            get
            {
                return _valor;
            }
            set
            {
#if RuntimeCache
                    string val = _valor;
                    if (Atributos.Respaldable(Atributo.valor))
                        Atributos.Agregar(Atributo.valor, delegate() { _valor = val; });
#endif
                _valor = value;
            }
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
            info.AddValue(serie.Valor(), _valor);
        }
        #endregion

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        internal void escritorXMLDato(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString(Lenguaje.Valor, _valor.ToString());
        }
        internal void lectorXMLDato(System.Xml.XmlReader reader)
        {
            _valor = reader.GetAttribute(Lenguaje.Valor);
        }
        #endregion
    }
    public class Dato<T> : Semilla, IValor<T>
    {
        public T Valor { get; set; }
        public Dato() : base()
        {
            Id = Lenguaje.Dato;
            IniDato();
        }
        void IniDato()
        {
            Clase = Lenguaje.Dato;
            Genero = Genero.Masculino;
            //EscritorXML = escritorXMLDato;
            //LectorXML = lectorXMLDato;
        }

    }
}