namespace Jen.Json
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Objeto : Dictionary<string, object>, IJson
    {

        public int TamSer { get; set; }
        public string Clase { get { return "Objeto"; } }
        public int NumItems { get { return Count; } }
        public Objeto()
        {
        }
        public Objeto(int nItems) : base(nItems)
        {
        }
        public string Encontrar(string clave, string valor)
        {
            foreach (var pItem in this)
            {
                Objeto p = this.Obtener<Objeto>(pItem.Key);
                if (p[clave].Equals(valor))
                {
                    return pItem.Key;
                }
            }
            return string.Empty;
        }
        public bool Encontrar(ref Objeto o, string clave, string valor)
        {
            foreach (var pItem in this)
            {
                Objeto p = this.Obtener<Objeto>(pItem.Key);
                if (p[clave].Equals(valor))
                {
                    o = p;
                    return true;
                }
            }
            return false;
        }

        public T Obtener<T>(params string[] ruta)
        {
            int iteracion = 0;
            Objeto obj = this;
            foreach (string id in ruta)
            {
                iteracion++;
                if ((obj.ContainsKey(id)))
                {
                    object objVal = obj[id];
                    if (objVal != null)
                    {
                        if (objVal.GetType() == typeof(Objeto))
                        {
                            if (iteracion == ruta.Length)
                            {
                                return (T)obj[id];
                            }
                            obj = (Objeto)obj[id];
                        }
                        else if (iteracion == ruta.Length)
                        {
                            return (T)obj[id];
                        }
                    }
                    else
                    {
                        return default(T);
                    }
                }
                else
                {
                    break;
                }
            }
            throw new System.ArgumentException(
                string.Concat("Error al obtener ", string.Join(".", ruta), " en el objeto"));
        }
        public bool Existe<T>(ref T t, params string[] ruta)
        {
            int iteracion = 0;
            Objeto obj = this;

            foreach (string id in ruta)
            {
                iteracion++;
                if ((obj.ContainsKey(id)) && (obj[id] != null))
                {
                    if (obj[id].GetType() == typeof(Objeto))
                    {
                        if (iteracion == ruta.Length)
                        {
                            t = (T)obj[id];
                            return true;
                        }
                        obj = (Objeto)obj[id];
                    }
                    else if (iteracion == ruta.Length)
                    {
                        t = (T)obj[id];
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            t = default(T);
            return false;
        }
        public bool Existe(params string[] ruta)
        {
            int iteracion = 0;
            Objeto obj = this;
            foreach (string id in ruta)
            {
                iteracion++;
                if ((obj.ContainsKey(id)) && (obj[id] != null))
                {
                    if (obj[id].GetType() == typeof(Objeto))
                    {
                        obj = (Objeto)obj[id];
                    }
                    else if (iteracion == ruta.Length)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        public string[] Claves
        {
            get
            {
                string[] claves = new string [Keys.Count];
                Keys.CopyTo(claves,0);
                return claves;
            }
        }
        IEnumerator<object> IJson.GetEnumerator()
        {
            //for (int i = Count-1; i >= 0; i--)
            foreach (object o in this)
            {
                yield return o;
            }
        }
        public new object this[string id]
        {
            get
            {
                object tc;
                if (TryGetValue(id, out tc))
                {
                    return tc;
                }
                return null;
            }
            set
            {
                base[id] = value;
            }
        }

        //Objeto inst = null;
        public static Objeto Ref
        {
            get { return null; }
        }

        public static void Serializar(ref StringBuilder sbSer, Objeto obj, int indent = 0, string dcma = "\"")
        {
            string sIndent = string.Empty;
            string sFinLinea = string.Empty;
            if (indent > 0)
            {
                sIndent = new string('\t', indent++);
                sFinLinea = "\n";
                indent++;
            }
            sbSer.Append("{ ");
            sbSer.Append(sFinLinea);

            foreach (KeyValuePair<string, object> t in obj)
            {
                //escrible el nombre de la entrada
                sbSer.Append(sIndent);
                sbSer.Append(dcma); 
                sbSer.Append(t.Key);
                sbSer.Append(dcma);
                sbSer.Append(":");
                if (t.Value == null)
                {
                    sbSer.Append("null,");
                    sbSer.Append(sFinLinea);
                }
                else if (t.Value.GetType() == typeof(string))
                {
                    sbSer.Append(Util.JavaScriptStringLiteral(t.Value.ToString(),dcma));
                    sbSer.Append(",");
                    sbSer.Append(sFinLinea);
                }
                else if (t.Value.GetType() == typeof(decimal))
                {
                    sbSer.Append(t.Value.ToString());
                    sbSer.Append(",");
                    sbSer.Append(sFinLinea);
                }
                else if (t.Value.GetType() == typeof(bool))
                {
                    sbSer.Append(t.Value.ToString().ToLower());
                    sbSer.Append(",");
                    sbSer.Append(sFinLinea);
                }
                else if (t.Value.GetType() == typeof(Objeto))
                {
                    Serializar(ref sbSer, (Objeto)t.Value, indent);
                }
                else if (t.Value.GetType() == typeof(Arreglo))
                {
                    Arreglo.Serializar(ref sbSer, (Arreglo)t.Value, indent);
                }
                else if (t.Value.GetType() == typeof(StringBuilder))
                {
                    //sbSer.Append(Util.JavaScriptStringLiteral(t.Value.ToString()));
                    sbSer.Append(t.Value.ToString());
                    sbSer.Append(",");
                    sbSer.Append(sFinLinea);
                }
                else
                {
                    sbSer.Append(Util.JavaScriptStringLiteral(t.Value.ToString(), dcma));
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
            sbSer.Append("},");
            sbSer.Append(sFinLinea);
        }
        public override string ToString()
        {
            return ToString(0);
        }
        public string ToString(int indent, string dcma = "\"")
        {
            int l1 = Count * Compilador<IJson>.TotalByteXEntrada;
            StringBuilder sb = new StringBuilder(l1 > TamSer ? l1 : TamSer);
            Serializar(ref sb, this, indent, dcma);
            sb.Length--;
            if (indent > 0)
            {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
}
