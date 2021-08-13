#if BDMD
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    [Serializable]
    public class Cubo : Semilla
    {
        #region constructor
            /// <summary>
            /// Constructor por defecto
            /// </summary>
            public Cubo()
                : base()
            {
                IniCubo();
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Cubo(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                IniCubo();
                Serie serie = (Serie)context.Context;
                Metricas = Constructor.Embriones[Lenguaje.Metricas].Germinar<Recipiente<Metrica>>(info, context);
                Dimensiones = Constructor.Embriones[Lenguaje.DimensionesUsadas].Germinar<Recipiente<Semilla>>(info, context);
            }
            void IniCubo()
            {
                Clase = Lenguaje.Cubo;
                Genero = Genero.Masculino;
                EscritorXML = escritorXMLCubo;
                LectorXML = lectorXMLCubo;
            }

        #endregion
        #region propiedades
            #region Metricas
                Recipiente<Metrica> _metricas;
                /// <summary>
                /// Contenedor de metricas del cubo
                /// </summary>  
                public Recipiente<Metrica> Metricas
                {
                    get { return _metricas; }
                    set 
                    { 
                        _metricas = value;
                        _metricas.NombreContenido = Lenguaje.Metricas;
                    }
                }
            #endregion
            #region Dimensiones
                Recipiente<Semilla> _dimensiones;
                /// <summary>
                /// Contenedor de dimensiones usadas en el cubo
                /// </summary>  
                public Recipiente<Semilla> Dimensiones
                {
                    get { return _dimensiones; }
                    set 
                    { 
                        _dimensiones = value;
                        _dimensiones.NombreContenido = Lenguaje.DimensionesUsadas;
                    }
                }
            #endregion
        #endregion
        #region metodos
            #region escritorXMLCubo
                /// <summary>
                /// Convierte el objecto a su representación XML .
                /// </summary>
                /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
                void escritorXMLCubo(System.Xml.XmlWriter writer)
                {
                    if (_metricas != null)
                        _metricas.WriteXml(writer);

                    if (_dimensiones != null)
                        _dimensiones.WriteXml(writer);
                }
            #endregion
                #region lectorXMLCubo
                /// <summary>
                /// Inicializa el objeto a partir de su representacion xml
                /// </summary>
                /// <param name="reader"></param>
                void lectorXMLCubo(System.Xml.XmlReader reader)
                {
                    reader.Read();
                    if (reader.Name.Equals(Lenguaje.Metricas))
                    {
                        Metricas = new Recipiente<Metrica>();
                        _metricas.ReadXml(reader);
                    }
                    if (reader.Name.Equals(Lenguaje.DimensionesUsadas))
                    {
                        Dimensiones = new Recipiente<Semilla>();
                        _dimensiones.ReadXml(reader);
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
                    _metricas.GetObjectData(info, context);
                    _dimensiones.GetObjectData(info, context);

                }
            #endregion
        #endregion
    }
}
#endif