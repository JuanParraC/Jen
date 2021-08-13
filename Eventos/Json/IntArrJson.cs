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
	public class IntArrJson : Evento<Campo>
	{
		/// <summary>
		/// Constructo por defecto del evento
		/// </summary>
        public IntArrJson()
			: base()
		{
            Id = Clase = Lenguaje.IntArrJson;
		}
		/// <summary>
		///  Constructor binario del evento
		/// </summary>
		/// <param name="info">Lista de propiedades serializadas</param>
		/// <param name="context">Contexto del proceso de deserialización</param>
        protected IntArrJson(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
            Id = Clase = Lenguaje.IntArrJson;
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
            if (Contexto._valor.GetType() == typeof(int[]))
            {
                int[] arrVal = (int[])Contexto._valor;
                StringBuilder sb = new StringBuilder(arrVal.Length * 10);
                sb.Append("[");
                foreach (int v in arrVal)
                {
                    sb.Append(v.ToString());
                    sb.Append(",");
                }
                sb.Length--;
                sb.Append("]");
                Valor = sb.ToString();
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