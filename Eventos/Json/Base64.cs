// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
	using System.Text;
    using Serializable = System.SerializableAttribute;
	using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
	using StreamingContext = System.Runtime.Serialization.StreamingContext;
	/// <summary>
	/// Evento que obtiene el "valor" del campo
	/// </summary>
	[Serializable]
	public class Base64 : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
        public Base64()
			: base()
		{
            Id = Clase = Lenguaje.Base64;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
        protected Base64(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
            Id = Clase = Lenguaje.Base64;
		}
		/// <summary>
		/// Punto de entrada para la ejecución del valor
		/// </summary>
		/// <returns></returns>
		//public override void Ejecutar(Evento ev)
		public override void Ejecutar()
		{
            //string valContexto = string.Empty;
			if (Contexto._valor == null) {
                Valor = Lenguaje.Null;
				return;
			}
            if (string.IsNullOrEmpty(Contexto._valor.ToString()))
            {
                Valor = Lenguaje.Null;
                return;
            }
            //string strVar = Contexto._valor.ToString();
            string strVar = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(Contexto._valor.ToString()));
            Valor = string.Concat("\"", strVar, "\"");
		}
		public override Campo Contexto
		{
			set
			{
				base.Contexto = value;
				value._valor_ = Id;
			}
		}
	}
}