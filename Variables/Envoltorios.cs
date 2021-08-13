

namespace Jen
{
    using System.Text;
    public class Envoltorios
    {
        string[] _envoltorios;

        public Envoltorios(string[] envoltorios)
        {
            _envoltorios = envoltorios;

        }
        public int Peso
        {
            get
            {
                int peso = 0;
                foreach (string k in _envoltorios)
                {
                    peso += k.Length;
                }
                return peso;
            }
        }

        public System.Collections.Generic.IEnumerator<string> GetEnumerator()
        {
            foreach (string k in _envoltorios)
            {
                yield return k;
            }            
        }
        public int Length
        {
            get
            {
                return _envoltorios.Length;
            }
        }
        public string this[int id]
        {
            set
            {
                _envoltorios[id] = value;
            }
            get
            {
                return _envoltorios[id];
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            foreach (string k in _envoltorios)
            {
                sb.Append(string.Concat("'", k, "',"));
            }
            sb.Length--;
            sb.Append("]");
            return sb.ToString();
        }
    }
}
