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
	public class ValorXml : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
        public ValorXml()
			: base()
		{
            Id = Clase = Lenguaje.ValorXml;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
        protected ValorXml(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Id = Clase = Lenguaje.ValorXml;
		}
		/// <summary>
		/// Punto de entrada para la ejecución del valor
		/// </summary>
		/// <returns></returns>
		//public override void Ejecutar(Evento ev)
		public override void Ejecutar()
		{
            //StringBuilder sb = new StringBuilder(30);
            string id = Contexto.Id;
            if (Contexto._valor == null)
            {
                Valor = string.Concat("<", id, ">", Lenguaje.Null, "</", id, ">");
                return;
            }

            //Valor = Contexto._valor.ToString().ToLower();
            Valor = string.Concat("<", id, ">", Contexto._valor.ToString().ToLower(), "</", id, ">");
        

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