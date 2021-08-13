// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// Contenedor de propiedades
    /// </summary>
    [Serializable]
    public class Propiedades : System.Collections.Generic.Dictionary<string, string> 
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Propiedades()
            : base()
        {
        }

        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Propiedades(SerializationInfo info, StreamingContext context)
            : base()
        {
            Serie serie = (Serie)context.Context;
            int Count = info.GetInt32(serie.Valor());
            for (int i = 0; i < Count; i++)
                Add(info.GetString(serie.Valor()), info.GetString(serie.Valor()));
        }

        #region GetObjectData
            /// <summary>
            /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
            /// </summary> 
            [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]            
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                Serie serie = (Serie)context.Context;
                info.AddValue(serie.Valor(), Count);
                foreach(string key in Keys)
                {
                    info.AddValue(serie.Valor(), key);
                    info.AddValue(serie.Valor(), this[key]);
                }
            }
        #endregion

        #region ReadXml
            /// <summary>
            /// <c>ReadXml : </c>Genera el objeto desde su representación XML.
            /// </summary>
            /// <mr name="reader">Tipo que permite leer el Archivo xml</mr>
            internal void lectorXMLPropiedades(System.Xml.XmlReader reader)
            {
                reader.Read();
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {

                    Add(reader.GetAttribute(Lenguaje.Id), reader.GetAttribute(Lenguaje.Valor));
                    reader.Read();
                }
            }
        #endregion
        #region WriteXml
            /// <summary>
            /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
            /// </summary>
            /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
            internal void escritorXMLPropiedades(System.Xml.XmlWriter writer)
            {
                writer.WriteStartElement(Lenguaje.Propiedades);
                foreach (string key in Keys)
                {
                    writer.WriteStartElement(Lenguaje.Propiedad);
                    writer.WriteAttributeString(Lenguaje.Id, key);
                    writer.WriteAttributeString(Lenguaje.Valor, this[key]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
         #endregion
    }
}