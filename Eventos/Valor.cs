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
	public class Valor : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
		public Valor()
			: base()
		{
			Id = Clase = Lenguaje.Valor;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
		protected Valor(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Id = Clase = Lenguaje.Valor;
		}
		/// <summary>
		/// Punto de entrada para la ejecución del valor
		/// </summary>
		/// <returns></returns>
		//public override void Ejecutar(Evento ev)
		public override void Ejecutar()
		{
			if (Contexto._valor == null) {
				Valor = string.Empty;
				return;
			}
			Valor = Contexto._valor.ToString();
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