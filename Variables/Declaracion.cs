// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Representa la declaracion de una variable dentro de un texto
    /// </summary>
    [Serializable]
    public class Declaracion : Semilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Declaracion()
            : base()
        {
            Clase = Lenguaje.Declaracion;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Declaracion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Clase = Lenguaje.Declaracion;
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            _posicion = info.GetInt32(serie.Valor());
            // instancia el objeto grafico
            if (info.GetBoolean(serie.Valor()))
            {
                string s = info.GetString(serie.Valor());
                ObjetoGrafico = (Jen.IU.ObjetoGrafico)Constructor.Embriones[s].Germinar();
                //ObjetoGrafico.InicializarID(s);
            }
            int tIds = info.GetInt32(serie.Valor());
            Identificadores = new string[tIds];
            for (int i = 0; i < tIds; i++)
            {
                Identificadores[i] = info.GetString(serie.Valor());
            }
        }
        // posicion declariativa de la instancia
        int _posicion;
        public int Posicion
        {
            get
            {
                return _posicion;
            }
            set
            {
                _posicion = value;
            }
        }
        // identificadores
        string[] _identificadores;
        /// <summary>
        /// Colección de identificadores de la declaración
        /// </summary>
        public string[] Identificadores
        {
            get
            {
                return _identificadores;
            }
            set
            {
                _identificadores = value;
            }
        }
        //enlace al modelo de procesamiento para extraer los valores
        internal Jen.IU.ObjetoGrafico _objGraf;
        /// <summary>
        /// Objeto grafico que implementa la funcionalidad de la declaración
        /// </summary>
        public Jen.IU.ObjetoGrafico ObjetoGrafico
        {
            get
            {
                return _objGraf;
            }
            set
            {
                _objGraf = value;
            }
        }
        #region GetObjectData
            /// <summary>
            //// Devuelve las propiedades serializadas del objeto.
            /// </summary> 
            [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                Serie serie = (Serie)context.Context;
                info.AddValue(serie.Valor(), Posicion);
                // si existe objeto grafico
                if (ObjetoGrafico != null)
                {
                    info.AddValue(serie.Valor(), true);
                    info.AddValue(serie.Valor(), ObjetoGrafico.Id);
                }
                else
                    info.AddValue(serie.Valor(), false);
                info.AddValue(serie.Valor(), Identificadores.Length);
                foreach (string k in Identificadores)
                    info.AddValue(serie.Valor(), k);
            }
        #endregion
    }
}