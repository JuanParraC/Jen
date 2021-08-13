namespace Jen.Cache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    [Serializable]
    public class Usuario : Dictionary<string, ConcurrentBag<Semilla>>
    {

    }
}
