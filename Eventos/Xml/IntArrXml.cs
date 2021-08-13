// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
	using System.Text;
    using Serializable = System.SerializableAttribute;
	using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
	using StreamingContext = System.Runtime.Serialization.StreamingContext;
	/// <summary>
	/// Evento que obtiene el arreglo de int del campo
	/// </summary>
	[Serializable]
	public class IntArrXml : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
        public IntArrXml()
			: base()
		{
            Id = Clase = Lenguaje.IntArrXml;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
        protected IntArrXml(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
            Id = Clase = Lenguaje.IntArrXml;
		}
		/// <summary>
		/// Punto de entrada para la ejecución del valor
		/// </summary>
		/// <returns></returns>
		//public override void Ejecutar(Evento ev)
		public override void Ejecutar()
		{
            string id = Contexto.Id;
            if (Contexto._valor == null)
            {
                Valor = string.Concat("<", id, ">", Lenguaje.Null, "</", id, ">");
                return;
            }
            if (Contexto._valor.GetType() == typeof(int[]))
            {
                int[] arrVal = (int[])Contexto._valor;
                StringBuilder sb = new StringBuilder(arrVal.Length * 10);
                sb.Append("<" + id + ">");
                int ind = 0;
                foreach (int v in arrVal)
                {
                    sb.Append("<");
                    sb.Append(ind);
                    sb.Append(">");
                    sb.Append(v.ToString());
                    sb.Append("</");
                    sb.Append(ind++);
                    sb.Append(">");
                }
                sb.Length--;
                sb.Append("</" + id + ">");
                Valor = sb.ToString();
            }
            if (string.IsNullOrEmpty(Valor))
            {
                Valor = string.Concat("<", id, ">", Lenguaje.Null, "</", id, ">");
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