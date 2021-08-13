// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using Jen;
    using System.Text;

    [Serializable]
    public class SubPlantillas<T> : ObjetoGrafico<T>
        where T : Semilla
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SubPlantillas()
            : base()
        {
            Id = Clase = "SubPlantillas";
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected SubPlantillas(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = "SubPlantillas";
        }
        /// <summary>
        /// pinta el largo del campo
        /// </summary>
        /// <returns>string con el resultado del pintado</returns>
        public override string Pintar()
        {
            StringBuilder ret = new StringBuilder();
            int largoSeparador = 0;
            if (Contexto.Plantillas != null)
            {
                foreach (Plantilla plt in Contexto.Plantillas)
                {
                    plt.Pintar(ref ret);
                    if (plt.Separadores != null)
                    {
                        if (plt.Separadores.Largo > 0)
                        {
                            largoSeparador = plt.Separadores[0].Valor.Length;
                        }
                    }
                }
            }
            ret.Length -= largoSeparador;
            return ret.ToString();
        }
    }
}
