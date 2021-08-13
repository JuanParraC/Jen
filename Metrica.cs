#if BDMD
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    [Serializable]
    public class Metrica : Semilla
    {
        #region constructor
            /// <summary>
            /// Constructor por defecto
            /// </summary>
            public Metrica()
                : base()
            {
                IniMetrica();
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Metrica(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                IniMetrica();
                Serie serie = (Serie)context.Context;
                _funcion = info.GetString(serie.Valor());
            }
            void IniMetrica()
            {
                Clase = Lenguaje.Metrica;
                Genero = Genero.Femenino;
                EscritorXML = escritorXMLMetrica;
                LectorXML = lectorXMLMetrica;
            }

        #endregion
        #region propiedades
            #region propiedad funcion
            //declara el origen _usuario privado para la propiedad
            private string _funcion = string.Empty;

            /// <summary>
            /// funcion de agregado de la metrica
            /// </summary>  
            public string Funcion
            {
                get { return _funcion; }
                set
                {
#if RuntimeCache                         
                    string fn = _funcion;

                    if (Atributos.Respaldable(Atributo.ayuda))
                        Atributos.Agregar(Atributo.ayuda, delegate() { _funcion = fn; });
#endif
                    _funcion = value;
                }

            }
            #endregion
        #endregion
        #region metodos
            #region escritorXMLMetrica
                /// <summary>
                /// Convierte el objecto a su representación XML .
                /// </summary>
                /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
                void escritorXMLMetrica(System.Xml.XmlWriter writer)
                {
                    if (!string.IsNullOrEmpty(_funcion))
                        writer.WriteAttributeString(Lenguaje.Funcion, _funcion);
                }
            #endregion
            #region escritorXMLMetrica
                /// <summary>
                /// Inicializa el objeto a partir de su representacion xml
                /// </summary>
                /// <param name="reader"></param>
                void lectorXMLMetrica(System.Xml.XmlReader reader)
                {
                    _funcion = reader.GetAttribute(Lenguaje.Funcion);
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
                    info.AddValue(serie.Valor(), _funcion);
                }
            #endregion
        #endregion

    }
}
#endif