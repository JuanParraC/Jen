
using System.Collections.Generic;

namespace Jen.Json
{
    public interface IJson
    {
        int TamSer { get; set; }
        string Clase { get; }
        int NumItems { get; }
        IEnumerator<object> GetEnumerator();
    }
}
