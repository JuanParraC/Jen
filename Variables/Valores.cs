namespace Jen
{
    using System.Collections.Generic;
    public class Valores<TipoValores> : Dictionary<int, TipoValores>
    {
        int peso;
        public Valores()
        {
        }
        public new void Add(int key, TipoValores item)
        {
            base.Add(key, item);
        }
    }
}
