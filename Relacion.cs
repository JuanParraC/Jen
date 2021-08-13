// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// representa una relacion entre dos tablas
    /// </summary>
    [Serializable]
    public class Relacion : Semilla 
    {
        #region constructor
            /// <summary>
            /// Constructor binario
            /// </summary>
            public Relacion() 
                : base() 
            {
                IniRelacion();
                Id = Lenguaje.Relacion;
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Relacion(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                IniRelacion();
                string cls = Lenguaje.Recipiente + Lenguaje.Semilla;
                Madre = Constructor.Embriones[cls].Germinar<Recipiente<Semilla>>(info, context);
                Hijo = Constructor.Embriones[cls].Germinar<Recipiente<Semilla>>(info, context);

                Serie serie = (Serie)context.Context;
                _relacion = (TipoRelacion)info.GetValue(serie.Valor(), typeof(TipoRelacion));
            }
            void IniRelacion()
            {
                Clase = Lenguaje.Relacion;
                EscritorXML = escritorXMLRelacion;
                LectorXML = lectorXMLRelacion;
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
                _madre.GetObjectData(info, context);
                _hijo.GetObjectData(info, context);
                Serie serie = (Serie)context.Context;
                info.AddValue(serie.Valor(), _relacion, typeof(TipoRelacion));

            }
        #endregion

        private Recipiente<Semilla> _madre;
        /// <summary>
        /// relacion madre
        /// </summary>
        public Recipiente<Semilla> Madre 
        {
            get { return _madre; }
            set 
                    {
#if RuntimeCache  
                        Recipiente<Semilla> r1 = _madre;
                        if (Atributos.Respaldable(Atributo.rel1))
                            Atributos.Agregar(Atributo.rel1, delegate() 
                            { 
                                _madre = r1; 
                            });
#endif
                        _madre = value;
                        _madre.NombreContenido = Lenguaje.Campos;
                        _madre.LeerPropiedades = LeerXML;
                    }
        }

        private Recipiente<Semilla> _hijo;
        /// <summary>
        /// relacion hija
        /// </summary>
        public Recipiente<Semilla> Hijo 
        {
            get { return _hijo; }
            set 
                    {
#if RuntimeCache  
                        Recipiente<Semilla> r2 = _hijo;
                        if (Atributos.Respaldable(Atributo.rel2))
                            Atributos.Agregar(Atributo.rel2, delegate() 
                            { 
                                _hijo = r2;
                            });
#endif
                        _hijo = value;
                        _hijo.NombreContenido = Lenguaje.Campos;
                        _hijo.LeerPropiedades = LeerXML;
                    }

        }
        /// <summary>
        /// especifica como viaja desde madre a hijo
        /// </summary>
        private TipoRelacion _relacion = TipoRelacion.Interna;
        /// <summary>
        /// Tipo de relacion
        /// </summary>
        public TipoRelacion TipoRelacion
        {
            get { return _relacion; }
            set
            {
#if RuntimeCache  
                TipoRelacion rel = _relacion;
                if (Atributos.Respaldable(Atributo.relacion))
                    Atributos.Agregar(Atributo.relacion, delegate() { _relacion = rel; });
#endif
                _relacion = value;
            }
        }

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorXMLRelacion(System.Xml.XmlWriter writer)
        {
            if (_madre != null)
            {
                writer.WriteStartElement(Lenguaje.Madre);
                _madre.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (_hijo != null)
            {
                writer.WriteStartElement(Lenguaje.Hijo);
                _hijo.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion
        void lectorXMLRelacion(System.Xml.XmlReader reader)
        {
            reader.Read();
            Madre = new Recipiente<Semilla>();
            _madre.ReadXml(reader);
            reader.Read();
            Hijo = new Recipiente<Semilla>();
            _hijo.ReadXml(reader);
            reader.Read();
        }

    }
}
