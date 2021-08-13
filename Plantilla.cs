// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Web;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using TextWriter = System.IO.TextWriter;

    /// <summary>
    /// <c>Plantilla</c> extiende un patron otorgandole un contexo quien proporciona los 
    /// datos de las variables
    /// </summary>
    [Serializable]
    public class Plantilla : Patron
    {
        System.Collections.Generic.IList<ISemilla> contexts = new System.Collections.Generic.List<ISemilla>();
        #region constructores
            /// <summary>
            /// Constructor vacio
            /// </summary>
            public Plantilla() 
                : base() 
            {
                IniPlantilla();
            }
            /// <summary>
            /// crea una plantilla pasando el contenido de esta
            /// </summary>
            /// <param name="contenido"></param>
            public Plantilla(string contenido, string regexp = Lenguaje.RegExpVariables)
            : base(contenido, regexp)
            {
                IniPlantilla();
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Plantilla(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                IniPlantilla();
            }
            void IniPlantilla()
            {
                Clase = Lenguaje.Plantilla;
            }
        #endregion

        /// <summary>
        /// proceso que mescla los valores pasados desde el modelo 
        /// del procesamiento con el texto de la plantilla
        /// </summary>
        /// <returns>retorna el texto resultante </returns>
        public override void Pintar(TextWriter salida)
        {
            //_contexto.mesclarContenido(this);
            foreach (ISemilla c in contexts)
            {
                c.MesclarContenido(Variables);
            }
			
            base.Pintar(salida);
            contexts.Clear(); 
        }

        public void Enlazar(ISemilla semilla)
        {
            contexts.Add(semilla);
            semilla.EnlazarGrafica(Variables, Request);
        }
    }
}