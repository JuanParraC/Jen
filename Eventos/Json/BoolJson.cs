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
	public class BoolJson : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
        public BoolJson()
			: base()
		{
            Id = Clase = Lenguaje.BoolJson;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
        protected BoolJson(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
            Id = Clase = Lenguaje.BoolJson;
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
            if (Contexto._valor is bool)
            {
                Valor = Contexto._valor.ToString().ToLower();
            }

            if (string.IsNullOrEmpty(Valor))
            {
                Valor = Lenguaje.Null;
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