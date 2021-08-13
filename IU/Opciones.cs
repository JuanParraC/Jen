// Autor : Juan Parra
// 3Soft
namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using StringBuilder = System.Text.StringBuilder;
    /// <summary>
    /// Represneta el objeto Select de html
    /// </summary>
    [Serializable]
    public class Opciones : ObjetoGrafico<Campo>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Opciones()
            : base()
        {
            Id = Clase = Lenguaje.Opciones;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Opciones(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            Id = Clase = Lenguaje.Opciones;
        }
        /// <summary>
        /// pintado del select
        /// </summary>
        /// <returns></returns>
        public override string Pintar()
        {

            // identificador del traductor
            string traductor = Contexto.Traductor;
            // si no posee traductor no devuelve items de opcion
            if (string.IsNullOrEmpty(traductor))
            {
                return string.Empty;
            }

            // valor del campo
            string valorCampo = Contexto.ToString();
            // direcciona al regPadre
            Registro registro = (Registro)Contexto.Padre;


            // direcciona al traductor de campo
            Traductor trad = registro.Traductores[traductor];
            return PintarTraductor(trad, valorCampo);

        }
        protected string PintarTraductor(Traductor trad, string valorCampo)
        {

            // si el traductor está excluido no devuelve items de opción
            if (Semilla.En(trad.Estado, Estado.Excluido))
            {
                return string.Empty;
            }
            int posSeleccion = -1;
            if (trad.Existe(valorCampo))
            {
                posSeleccion = trad.IndexOf(valorCampo);
            }
            StringBuilder sb = new StringBuilder();
            if (posSeleccion >= 0)
            {
                for (int i = 0; i < posSeleccion; i++)
                {
                    sb.Append(Lenguaje.OptionValue);
                    sb.Append(trad[i].Id);
                    sb.Append(Lenguaje.Comilla);
                    sb.Append(Lenguaje.MayorQue);
                    sb.Append(trad[i].Valor);
                    sb.Append(Lenguaje.Option);
                }
                sb.Append(Lenguaje.OptionValue);
                sb.Append(trad[posSeleccion].Id);
                sb.Append(Lenguaje.Comilla);
                sb.Append(Lenguaje.Selected);
                sb.Append(Lenguaje.MayorQue);
                sb.Append(trad[posSeleccion].Valor);
                sb.Append(Lenguaje.Option);

                Dato d;
                for (int i = posSeleccion + 1; i < trad.Largo; i++)
                //foreach (Dato d in trad.Contenido)
                {
                    d = trad[i];
                    sb.Append(Lenguaje.OptionValue);
                    sb.Append(d.Id);
                    sb.Append(Lenguaje.Comilla);
                    sb.Append(Lenguaje.MayorQue);
                    sb.Append(d.Valor);
                    sb.Append(Lenguaje.Option);
                }
            }
            else
            {
                //for (int i = 0; i < trad.Largo; i++)
                foreach (Dato d in trad.Contenido)
                {
                    {
                        sb.Append(Lenguaje.OptionValue);
                        sb.Append(d.Id);
                        sb.Append(Lenguaje.Comilla);
                        sb.Append(Lenguaje.MayorQue);
                        sb.Append(d.Valor);
                        sb.Append(Lenguaje.Option);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
