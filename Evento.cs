// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using XmlRoot = System.Xml.Serialization.XmlRootAttribute;

    /// <summary>
    /// metodo en una clase que ejecuta una porcion de codigo sin retornar un valor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable, XmlRoot(Lenguaje.Evento)]
    public abstract class Evento<T> : Semilla, IEvento<T>
        where T : ISemilla
    {

        #region constructor
            /// <summary>
            /// Constructor binario
            /// </summary>
            protected Evento() 
                : base() 
            {
                IniEvento();
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Evento(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                IniEvento();
                // obtiene la serie del contexto de la serializacion
                Serie serie = (Serie)context.Context;
                // lee el consejo del evento
                _consejo = info.GetString(serie.Valor());
            }
            void IniEvento()
            {
                Genero = Genero.Masculino;
                EscritorXML = EscritorXMLEvento;
                LectorXML = lectorXMLEvento;
            }
        #endregion

        /// <summary>
        /// inicio de ejecución
        /// </summary>
        //public abstract void Ejecutar(Evento ev);
            public abstract void Ejecutar();

        private T _contexto;
        /// <summary>
        /// contexto del codigo
        /// </summary>
        public virtual T Contexto 
        {
            get 
            {
                return _contexto; 
            }
            set
            {
                _contexto = value; 
            }
        }

        private string _consejo;
        /// <summary>
        /// mensaje cuando se gatilla el metodo
        /// </summary>
        public string Consejo
        {
            get
            {
                return _consejo;
            }
            set
            {
                _consejo = value;
            }
        }

        #region propiedad valor
        //declara el origen _agrupa privado para la propiedad
        private string _valor = string.Empty;

        /// <summary>
        /// <c>valor : </c> propiedad valor.
        /// </summary>  
        public string Valor
        {
            get
            {
                return _valor;
            }
            set
            {
                _valor = value;
            }
        }
        #endregion

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        public void EscritorXMLEvento(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString(Lenguaje.Clase, Clase);
            if (!string.IsNullOrEmpty(_consejo))
                writer.WriteAttributeString(Lenguaje.Consejo, _consejo);
        }
        #endregion
        internal void lectorXMLEvento(System.Xml.XmlReader reader)
        {
            _consejo = reader.GetAttribute(Lenguaje.Consejo);
        }

        #region GetObjectData
            /// <summary>
            /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
            /// </summary> 
            [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                if (!En(Estado, Jen.Estado.Excluido))
                {
                    // obtiene la serie del contexto de la serializacion
                    Serie serie = (Serie)context.Context;
                    info.AddValue(serie.Valor(), _consejo);
                }
            }
        #endregion
    }
}