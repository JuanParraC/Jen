// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
	using Serializable = System.SerializableAttribute;
	using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
	using StreamingContext = System.Runtime.Serialization.StreamingContext;
	/// <summary>
	/// Evento que obtiene el "valor" del campo
	/// </summary>
	[Serializable]
	public class ValorJson : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
        public ValorJson()
			: base()
		{
            Id = Clase = Lenguaje.ValorJson;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
        protected ValorJson(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Id = Clase = Lenguaje.ValorJson;
		}
		/// <summary>
		/// Punto de entrada para la ejecución del valor
		/// </summary>
		/// <returns></returns>
		//public override void Ejecutar(Evento ev)
		public override void Ejecutar()
		{
			if (Contexto._valor == null) {
				Valor = Lenguaje.Null;
				return;
			}
            switch (Contexto.Tipo)
            {
                case Tipo.Numerico:
                    Valor = Contexto._valor.ToString();
                    break;
                case Tipo.Texto:
                    string strVar = Contexto._valor.ToString().Replace("\"", @"\" + "\"");
                    Valor = string.Concat("\"", strVar, "\"");
                    break;
                case Tipo.Fecha:
                    Valor = string.Concat("\"", Contexto._valor.ToString(), "\"");
                    break;
                default:
                    Valor = Contexto._valor.ToString();
                    break;
            }
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