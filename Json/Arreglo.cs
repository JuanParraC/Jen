namespace Jen.Json
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Arreglo : List<object>, IJson
    {
        public int TamSer { get; set; }
        public string Clase { get { return "Arreglo"; } }
        public int NumItems { get { return Count; } }

        public static Arreglo Ref
        {
            get { return null; }
        }


        public Arreglo() { }
        public Arreglo(object contenido, char separador = ',')
        {
            if (contenido != null)
            {
                string[] lista = contenido.ToString().Split(separador);
                foreach (string l in lista)
                {
                    Add(l);
                }
            }
        }
        public Arreglo(int nItems) : base(nItems)
        {
        }

        public new IEnumerator<object> GetEnumerator()
        {
            //for (int i = Count-1; i >= 0; i--)
            for (int i = 0; i < this.Count; i++)
            //foreach(object o in this)
            {
                yield return this[i];
            }
        }
        /*public new object this[int id]
        {
            get
            {
                return this[Count - id];
            }
        }
*/
        public static void Serializar(ref StringBuilder sbSer, Arreglo arr, int indent = 0)
        {
            string sIndent = string.Empty;
            string sFinLinea = string.Empty;
            if (indent > 0)
            {
                
                sIndent = new String('\t', indent++);
                sFinLinea = "\n";
                indent++;
            }
            sbSer.Append("[ ");
            sbSer.Append(sFinLinea);

            // recorre cada elemento del arreglo
            foreach (object obj in arr)
            {
                sbSer.Append(sIndent);

                //obj = arr[i];
                // serializa el valor
                if (obj == null)
                {
                    sbSer.Append("null,");
                    sbSer.Append(sFinLinea);
                }
                else if (obj is string)
                {
                    sbSer.Append("\"");
                    sbSer.Append(obj);
                    sbSer.Append("\",");
                    sbSer.Append(sFinLinea);
                }
                else if (obj is bool)
                {
                    sbSer.Append(obj.ToString().ToLower());
                    sbSer.Append(",");
                    sbSer.Append(sFinLinea);
                }
                else if (obj.GetType() == typeof(Objeto))
                {
                    Objeto.Serializar(ref sbSer, (Objeto)obj, indent);
                }
                else if (obj.GetType() == typeof(Arreglo))
                {
                    Serializar(ref sbSer, (Arreglo)obj, indent);
                }
                else
                {
                    sbSer.Append(obj.ToString());
                    sbSer.Append(",");
                    sbSer.Append(sFinLinea);
                }
            }
            // elimina ultima coma y finaliza el objeto
            sbSer.Length--;
            if (indent > 0)
            {
                sbSer.Length--;
            }
            sbSer.Append("],");
            sbSer.Append(sFinLinea);
        }
        public override string ToString()
        {
            return ToString(0);
        }
        public string ToString(int indent)
        {
            int l1 = Count * Compilador<IJson>.TotalByteXEntrada;
            StringBuilder sb = new StringBuilder(l1 > TamSer ? l1 : TamSer);

            Serializar(ref sb, this, indent);
            sb.Length--;
            if (indent > 0)
            {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
}
