#if BDMD
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// </summary>
    [Serializable]
    public class Dimension : Recipiente<Semilla>
    {
        #region constructor
            /// <summary>
            /// Constructor por defecto
            /// </summary>
            public Dimension()
                : base()
            {
                IniDimension();
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Dimension(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                IniDimension();
                Serie serie = (Serie)context.Context;
                _tieneTodos = info.GetBoolean(serie.Valor());

            }
            void IniDimension()
            {
                Clase = Lenguaje.Dimension;
                Genero = Genero.Femenino;
                AntesDeEscribirContenedor = escritorXMLDimension;
                LeerPropiedades = lectorXMLDimension;
            }

            #endregion
        #region propiedades
            #region propiedad tieneTodos
                //declara el origen _clave privado para la propiedad
                private bool _tieneTodos;

                /// <summary>
                /// Propiedad tieneTodos.
                /// </summary>  
                public bool TieneTodos 
                {
                get { return _tieneTodos; }
                set 
                    {
#if RuntimeCache 
                        bool tTodos = _tieneTodos;
                        if (Atributos.Respaldable(Atributo.tieneTodos))
                            Atributos.Agregar(Atributo.tieneTodos, delegate() { tTodos = _tieneTodos; });
#endif
                        _tieneTodos = value;
                    }
                }
            #endregion
        #endregion
        #region metodos
            #region escritorXMLDimension
                /// <summary>
                /// Convierte el objecto a su representación XML .
                /// </summary>
                /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
                void escritorXMLDimension(System.Xml.XmlWriter writer)
                {
                    if (_tieneTodos)
                    {
                        writer.WriteStartAttribute(Lenguaje.TieneTodos);
                        writer.WriteValue(_tieneTodos);
                        writer.WriteEndAttribute();
                    }
                }
            #endregion
            #region escritorXMLDimension
                /// <summary>
                /// Inicializa el objeto a partir de su representacion xml
                /// </summary>
                /// <param name="reader"></param>
                void lectorXMLDimension(System.Xml.XmlReader reader)
                {
                    string tieneTodos = reader.GetAttribute(Lenguaje.TieneTodos);
                    if (!string.IsNullOrEmpty(tieneTodos))
                        _tieneTodos = bool.Parse(tieneTodos);
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
                    info.AddValue(serie.Valor(), _tieneTodos);
                }
            #endregion
        #endregion
    }
}
#endif