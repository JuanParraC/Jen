// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento para la validación de campos con digito verificador 
    /// </summary>
    [Serializable]
    public class ValidarDigitoVerificador : Evento<Campo>
    {
        /// <summary>
        /// Constructo por defecto del evento
        /// </summary>
        public ValidarDigitoVerificador()
            : base()
        {
            Id = Clase = Lenguaje.ValidarDigitoVerificador;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Lista de propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de deserialización</param>
        protected ValidarDigitoVerificador(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarDigitoVerificador;
        }
        /// <summary>
        /// Punto de entrada de la validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            // declara una variable para el calculo del digito verificador
            string campoDv = null;
            // direcciona el campo contexto de la varidación 
            Campo campo = Contexto;
            // obtiene el valor almacenado en el campo
            string valor = campo.ToString();
            // controla el blanco
            if (string.IsNullOrEmpty(valor))
                return;

            // comienza el calculo del digito verificador
            int multiplicador = 2;
            int sumatoria = 0;
            for (int i = valor.Length - 1; i >= 0; i--)
            {
                sumatoria = sumatoria + multiplicador++ * System.Convert.ToInt32(valor.Substring(i,1), Util.Cultura);
                // reinicia el multiplicador
                if (multiplicador == 8) multiplicador = 2;
            }
            int mod11 = (sumatoria % 11);
            int iDv = mod11 == 0 ? 0 : 11 - mod11;
            string sDv = iDv == 10 ? Lenguaje.k : iDv.ToString(Util.Cultura);

            if (campo.Propiedades != null)
                campoDv = campo.Propiedades[Lenguaje.dv];
            // compara el resultado con el valor almacenado en el campo
            if (!string.IsNullOrEmpty(campoDv))
            {
                Registro registro = (Registro)campo.Padre;
                string valorDv = registro.Campos[campoDv].Valor.ToLower(Util.Cultura);
                if (sDv.Equals(valorDv))
                    return;
            }
            // establece el consejo para el despliegue en la validación
            Contexto.Consejos = string.IsNullOrEmpty(Consejo) ? string.Concat(Contexto.Articulo, Lenguaje.Espacio,
                Contexto.Clase, Lenguaje.Espacio,
                Contexto.Etiqueta, Lenguaje.Espacio,
                Lenguaje.DvNoConcuerda) : Consejo;
            // informa el estado de error en el campo
            campo.Estado |= Estado.Error;
        }
    }
}