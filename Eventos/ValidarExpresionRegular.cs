// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using System.Security.Permissions;
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que permite validar campos con expresión regular
    /// </summary>
    [Serializable]
    public class ValidarExpresionRegular : Evento<Campo>
    {
        /// <summary>
        /// Constructor por defecto del evento
        /// </summary>
        public ValidarExpresionRegular()
            : base()
        {
            IniValidarExpresionRegular();
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarExpresionRegular(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniValidarExpresionRegular();

            Serie serie = (Serie)context.Context;
            _expReg = info.GetString(serie.Valor());
        }

        void IniValidarExpresionRegular()
        {
            Id = Clase = Lenguaje.ValidarExpresionRegular;
            Genero = Genero.Femenino;
            EscritorXML = escritorXMLValidarExpresionRegular;
            LectorXML = lectorXMLValidarExpresionRegular;
        }
        internal void lectorXMLValidarExpresionRegular(System.Xml.XmlReader lectorXml)
        {
            lectorXMLEvento(lectorXml);
            _expReg = lectorXml.GetAttribute(Lenguaje.Expresion);
        }

        void escritorXMLValidarExpresionRegular(System.Xml.XmlWriter writer)
        {
            EscritorXMLEvento(writer);
            if (!string.IsNullOrEmpty(_expReg))
                writer.WriteAttributeString(Lenguaje.Expresion, _expReg);

        }

        #region propiedad expresionRegular
        //declara el origen _expresionRegular privado para la propiedad
        private string _expReg = string.Empty;
        /// <summary>
        /// <c>expresionRegular : </c> propiedad expresionRegular.
        /// </summary>  
        public string ExpReg
        {
            get { return _expReg; }
            set
            {
#if RuntimeCache  
                string exR = _expReg;
                if (Atributos.Respaldable(Atributo.expresionRegular))
                    Atributos.Agregar(Atributo.expresionRegular, delegate() { _expReg = exR; });
#endif
                _expReg = value;
            }
        }
        #endregion
        /// <summary>
        /// Punto de entrada de la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            //direcciona al campo
            Campo campo = Contexto;
            //Obtiene el valor crudo del campo
            string valor = campo.ToString();
            //si el campo tiene valor  
            if (!string.IsNullOrEmpty(valor))
            {
                //Crea una expresión regular compilada
                Regex valRegex = new Regex(_expReg, RegexOptions.Compiled);
                // si no concuerda  informa el error de foramato según el formato especificado
                if (!valRegex.IsMatch(valor))
                {
                    campo.Consejos = string.IsNullOrEmpty(Consejo) ? 
                        string.Concat(campo.Articulo, Lenguaje.Espacio,
                        campo.Clase, Lenguaje.Espacio,
                        campo.Nombre,
                        Lenguaje.CampoNoConcuerdaConValidacion) : Consejo;
                    campo.Estado |= Estado.Error;
                }
            }
        }
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
                info.AddValue(serie.Valor(), _expReg);
            }
        #endregion
    }
}
