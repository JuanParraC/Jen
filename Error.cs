// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using TextWriter = System.IO.TextWriter;

    /// <summary>
    /// Plantilla de error
    /// </summary>
    [Serializable]
    public class Error: Patron
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Error()
        {
            Id = Clase = Lenguaje.Error;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Error(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Clase = Lenguaje.Error;
        }
        /// <summary>
        /// Pinta el error
        /// </summary>
        /// <param name="salida"></param>
        public override void Pintar(TextWriter salida)
        {
            Registro registro = Contexto;
            if (registro == null) return;
            //Campo campo;

            Jen.IU.Consejos<IEtiquetaIConsejos> consejos = new Jen.IU.Consejos<IEtiquetaIConsejos>();

            Campos campos = registro.Campos;
            //int nCampos = campos.Largo;

            // pinta el consejo del registro
            consejos.Contexto = registro;
            Variables[Lenguaje.Error].Asignar(consejos.Pintar());

            //for (int c = 0; c < nCampos; c++)
			foreach(Campo campo in campos)	
            {
                //campo = campos[c];
                if (!string.IsNullOrEmpty(campo.Consejos))
                {
                    consejos.Contexto = campo;
                    // pinta el consejo del campo
                    Variables[Lenguaje.Error].Asignar(consejos.Pintar());
                }
            }
            base.Pintar(salida);
        }
        #region propiedad contexto
            private Registro _contexto;
            /// <summary>
            /// contexto de la plantilla error
            /// </summary>
            public Registro Contexto 
            {
                get { return _contexto; }
                set 
                    {
                        _contexto = value; 
                    }

            }
        #endregion
    }
}
 