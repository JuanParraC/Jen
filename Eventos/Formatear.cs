// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que genera el valor formateado del campo definido en las propiedad formato
    /// </summary>
    [Serializable]
    public class Formatear : Valor
    {
        /// <summary>
        /// Constructor por defecto del objeto
        /// </summary>
        public Formatear()
            : base()
        {
            Id = Clase = Lenguaje.Formatear;
        }
        /// <summary>
        /// Constructor binario del valor
        /// </summary>
        /// <param name="info">Propiedades serializadas del valor</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected Formatear(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.Formatear;
        }

        /// <summary>
        /// Punto de entrada para el cálculo del valor
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // direcciona el campo
            Campo campo = Contexto;
            // obtiene el valor crudo del campo
            string valorCampo = Valor = campo.ToString(); 

            // valida si posee mascara de formato
            string formato = campo.Formato;
            if (string.IsNullOrEmpty(formato)) return; 

            Tipo tipoCampo = campo.Tipo;
            if (tipoCampo == Tipo.Numerico)
                Valor = System.Convert.ToDouble(valorCampo, Util.Cultura).ToString(formato, Util.Cultura);
        }
    }
}
