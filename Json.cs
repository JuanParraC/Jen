// Autor : Juan Parra
// 3Soft
namespace Jen.Json2
{
    using System.Collections.Generic;
    using Jen;
    using Boolean = System.Boolean;
    using Double = System.Double;
    using NumberStyles = System.Globalization.NumberStyles;
    using StringBuilder = System.Text.StringBuilder;

    public class Arreglo : List<object>
    {
        public Arreglo() { }
        public Arreglo(int nItems) : base(nItems) { }
        public static void Serializar(ref StringBuilder sbSer, Arreglo arr)
        {
            // objeto 
            sbSer.Append("[ ");
            // recorre cada propiedad del arreglo
            foreach (object obj in arr)
            {
                // serializa el valor
                if (obj == null)
                {
                    sbSer.Append("null,");
                }
                else if (obj is string)
                {
                    sbSer.Append("\"");
                    sbSer.Append(obj);
                    sbSer.Append("\",");
                }
                else if (obj is bool)
                {
                    sbSer.Append("\"");
                    sbSer.Append(obj.ToString().ToLower());
                    sbSer.Append("\",");
                }
                //else if (t is Tabla)
                else if (obj.GetType() == typeof(Objeto))
                {
                    Objeto.Serializar(ref sbSer, (Objeto)obj);
                    sbSer.Length--;
                    // serializa la entrada del diccionario recursivamente como hashtable
                    //Seralizar(ref sbSer, (Objeto)obj);
                }
                else if (obj.GetType() == typeof(Arreglo))
                {
                    Serializar(ref sbSer, (Arreglo)obj);
                    sbSer.Length--;
                }
                else
                {
                    sbSer.Append(obj.ToString());
                    sbSer.Append(",");
                }

            }
            // elimina ultima coma y finaliza el objeto
            sbSer.Length--;
            sbSer.Append("],");
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.Count * Compilador.TotalByteXEntrada);
            Serializar(ref sb, this);
            sb.Length--;
            return sb.ToString();
        }
    }

    public class Objeto : Dictionary<string, object>
    {
        public Objeto() { }
        public Objeto(int nItems) : base(nItems) {  }
        public static void Serializar(ref StringBuilder sbSer, Objeto obj)
        {
            //determina si es necesario aumentar la capacidad del stringbuilder 
            if ((sbSer.Capacity - sbSer.Length) < (Compilador.TotalByteXEntrada * obj.Count))
            {
                sbSer.Capacity += Compilador.TotalByteXEntrada * obj.Count;
            }
            sbSer.Append("{ ");
            // recorre cada propiedad del hash
            foreach (KeyValuePair<string, object> t in obj)
            {
                //escrible el nombre de la entrada
                sbSer.Append("\""); sbSer.Append(t.Key); sbSer.Append("\":");
                if (t.Value == null)
                {
                    sbSer.Append("null,");
                }
                else if (t.Value is string)
                {
                    sbSer.Append("\"");
                    sbSer.Append(t.Value.ToString());
                    sbSer.Append("\",");
                }
                else if (t.Value is bool)
                {
                    sbSer.Append("\"");
                    sbSer.Append(t.Value.ToString().ToLower());
                    sbSer.Append("\",");
                }
                else if (t.Value.GetType() == typeof(Objeto))
                {
                    Serializar(ref sbSer, (Objeto)t.Value);
                    sbSer.Length--;
                }
                else if (t.Value.GetType() == typeof(Arreglo))
                {
                    Arreglo.Serializar(ref sbSer, (Arreglo)t.Value);
                    sbSer.Length--;
                }
                else
                {
                    sbSer.Append(t.Value.ToString());
                    sbSer.Append(",");
                }
            }
            // elimina ultima coma y finaliza el objeto
            sbSer.Length--;
            sbSer.Append("},");
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Count * Compilador.TotalByteXEntrada);
            Serializar(ref sb, this);
            sb.Length--;
            return sb.ToString();
        }
    }

    struct token
    {
        public const int inicio = 0;
        public const int blanco = 1;
        public const int llaveAbrierta = 2;
        public const int llaveCerrada = 3;
        public const int braketAbierto = 4;
        public const int braketCerrado = 5;
        public const int dosPuntos = 6;
        public const int coma = 7;
        public const int texto = 8;
        public const int numero = 9;
        public const int verdadero = 10;
        public const int falso = 11;
        public const int nulo = 12;
        public const int nan = 13;
    }

    /// <summary>
    /// Decodifica string en formato json
    /// </summary>
    public static class Compilador
    {
        // cantidad de estimada caracteres por entrada de un objeto json
        public static int TotalByteXEntrada = 16;
        public static object Evaluar(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                int indice = 0;
                bool exito = true;
                return evaluar(json, ref indice, ref exito);
            }
            return null;
        }
        // procesa Objeto
        static Objeto objeto(string json, ref int indice)
        {

            Objeto ret = new Objeto();
            int tkn;

            sig(json, ref indice);

            bool salir = false;
            while (!salir)
            {
                tkn = prox(json, indice);
                if (tkn == token.blanco)
                {
                    return null;
                }
                else if (tkn == token.coma)
                {
                    sig(json, ref indice);
                }
                else if (tkn == token.llaveCerrada)
                {
                    sig(json, ref indice);
                    return ret;
                }
                else if (tkn == token.nan)
                {
                    sig(json, ref indice);
                    return ret;
                }
                else
                {
                    string prop = txt(json, ref indice);
                    if (string.IsNullOrEmpty(prop))
                        return null;

                    tkn = sig(json, ref indice);
                    if (tkn != token.dosPuntos)
                        return null;

                    bool exito = true;
                    object val = evaluar(json, ref indice, ref exito);
                    if (!exito)
                        return null;
                    ret[prop] = val;
                }
            }
            return ret;
        }
        // procesa un arreglo
        static Arreglo arr(string json, ref int indice)
        {
            Arreglo ret = new Arreglo();

            sig(json, ref indice);

            bool salir = false;
            while (!salir)
            {
                int tkn = prox(json, indice);
                if (tkn == token.blanco)
                {
                    return null;
                }
                else if (tkn == token.coma)
                {
                    sig(json, ref indice);
                }
                else if (tkn == token.braketCerrado)
                {
                    sig(json, ref indice);
                    break;
                }
                else
                {
                    bool exito = true;
                    object o = evaluar(json, ref indice, ref exito);
                    if (!exito)
                    {
                        return null;
                    }
                    ret.Add(o);
                }
            }
            return ret;
        }
        // compila un objeto
        static object evaluar(string json, ref int indice, ref bool exito)
        {
            switch (prox(json, indice))
            {
                case token.texto:
                    return txt(json, ref indice);
                case token.numero:
                    return num(json, ref indice);
                case token.llaveAbrierta:
                    return objeto(json, ref indice);
                case token.braketAbierto:
                    return arr(json, ref indice);
                case token.verdadero:
                    sig(json, ref indice);
                    return Boolean.Parse(Lenguaje.True);
                case token.falso:
                    sig(json, ref indice);
                    return Boolean.Parse(Lenguaje.False);
                case token.nulo:
                    sig(json, ref indice);
                    return null;
                case token.nan:
                    sig(json, ref indice);
                    return string.Empty;
                case token.blanco:
                    break;
            }
            exito = false;
            return null;
        }
        // procesa el texto
        static string txt(string json, ref int indice)
        {
            StringBuilder sb = new StringBuilder(TotalByteXEntrada);
            char chActual, chSepTexto;
            limpiar(json, ref indice);

            // obtiene el separador de texto usado en el json
            chSepTexto = chActual = json[indice++];
            bool completado = false;
            while (!completado)
            {
                if (indice == json.Length)
                    break;
                chActual = json[indice++];
                // encuentra el separador derecho del texto
                if (chActual == chSepTexto)
                {
                    completado = true;
                    break;
                }
                int largoRestante = json.Length - indice;

                if (chActual == CaracteresJson.backSlashBackSlash)
                {
                    if (indice == json.Length)
                    {
                        break;
                    }
                    chActual = json[indice++];
                    if ((chActual == chSepTexto) ||
                        (chActual == CaracteresJson.backSlashBackSlash) ||
                        (chActual == CaracteresJson.slash))
                    {
                        sb.Append(chActual);
                    }
                    else if (chActual == CaracteresJson.b)
                    {
                        sb.Append(CaracteresJson.backSlashB);
                    }
                    else if (chActual == CaracteresJson.f)
                    {
                        sb.Append(CaracteresJson.backSlashF);
                    }
                    else if (chActual == CaracteresJson.n)
                    {
                        sb.Append(CaracteresJson.backSlashN);
                    }
                    else if (chActual == CaracteresJson.r)
                    {
                        sb.Append(CaracteresJson.backSlashR);
                    }
                    else if (chActual == CaracteresJson.t)
                    {
                        sb.Append(CaracteresJson.backSlashT);
                    }
                    else if (chActual == CaracteresJson.u)
                    {
                        
                        if (largoRestante >= 4)
                        {
                            // parsea un hexadecimal de 32 bit en un entero
                            string sNum = json.Substring(indice, 4);
                            int car = int.Parse(sNum, NumberStyles.HexNumber, Util.Cultura);
                            sb.Append(car);
                            indice += 4;
                        }
                        else
                            break;
                    }
                }
                else
                {
                    sb.Append(chActual);
                }
            }
            if (!completado)
            {
                return null;
            }

            //Util.ToLog(string.Concat(sb.Length.ToString(),"\t",sb.Capacity.ToString(),"\n"));
            return sb.ToString();
        }
        //procesa numeros
        static double num(string json, ref int index)
        {
            limpiar(json, ref index);

            int iFinNum = finNum(json, index);
            int largoTexto = (iFinNum - index) + 1;
            char[] arrChar = new char[largoTexto];
            string sNum = json.Substring(index, largoTexto);

            //Array.Copy(json, index, arrChar, 0, largoTexto);
            index = iFinNum + 1;
            //return Double.Parse(new string(arrChar), Util.Cultura);
            return Double.Parse(sNum, Util.Cultura.NumberFormat);

        }
        // entrega el proximo
        static int prox(string json, int indice)
        {
            int guardarIndice = indice;
            return sig(json, ref guardarIndice);
        }
        // mueve al siguiente 
        static int sig(string json, ref int indice)
        {
            limpiar(json, ref indice);

            if (indice == json.Length)
                return token.blanco;
            char c = json[indice];
            indice++;
            switch (c)
            {
                case CaracteresJson.llaveAbierta:
                    return token.llaveAbrierta;
                case CaracteresJson.llaveCerrada:
                    return token.llaveCerrada;
                case CaracteresJson.braketAbierto:
                    return token.braketAbierto;
                case CaracteresJson.braketCerrado:
                    return token.braketCerrado;
                case CaracteresJson.coma:
                    return token.coma;
                case CaracteresJson.comillaDoble:
                case CaracteresJson.comilla:
                    return token.texto;
                case CaracteresJson.cero:
                case CaracteresJson.uno:
                case CaracteresJson.dos:
                case CaracteresJson.tres:
                case CaracteresJson.cuatro:
                case CaracteresJson.cinco:
                case CaracteresJson.seis:
                case CaracteresJson.siete:
                case CaracteresJson.ocho:
                case CaracteresJson.nueve:
                case CaracteresJson.guion:
                    return token.numero;
                case CaracteresJson.dosPuntos:
                    return token.dosPuntos;
            }
            indice--;
            int largoResto = json.Length - indice;
            if (largoResto >= 5)
            {
                if (json[indice] == CaracteresJson.f &&
                    json[indice + 1] == CaracteresJson.a &&
                    json[indice + 2] == CaracteresJson.l &&
                    json[indice + 3] == CaracteresJson.s &&
                    json[indice + 4] == CaracteresJson.e)
                {
                    indice += 5;
                    return token.falso;
                }
            }
            if (largoResto >= 4)
            {
                if (json[indice] == CaracteresJson.t &&
                    json[indice + 1] == CaracteresJson.r &&
                    json[indice + 2] == CaracteresJson.u &&
                    json[indice + 3] == CaracteresJson.e)
                {
                    indice += 4;
                    return token.verdadero;
                }
            }
            if (largoResto >= 4)
            {
                if (json[indice] == CaracteresJson.n &&
                    json[indice + 1] == CaracteresJson.u &&
                    json[indice + 2] == CaracteresJson.l &&
                    json[indice + 3] == CaracteresJson.l)
                {
                    indice += 4;
                    return token.nulo;
                }
            }
            if (largoResto >= 3)
            {
                if (json[indice] == CaracteresJson.N &&
                    json[indice + 1] == CaracteresJson.a &&
                    json[indice + 2] == CaracteresJson.N)
                {
                    indice += 3;
                    return token.nan;
                }
            }
            return token.blanco;
        }
        // devuelve la posicion final del un numero
        static int finNum(string json, int indice)
        {
            int ind;
            for (ind = indice; ind < json.Length; ind++)
                if (Lenguaje.Numeros.IndexOf(json[ind]) == -1)
                    break;
            return ind - 1;
        }
        //excluye caracteres no necesarios en la secuencia
        static void limpiar(string json, ref int indice)
        {
            for (; indice < json.Length; indice++)
                if (Lenguaje.Blancos.IndexOf(json[indice]) == -1)
                    break;
        }
    }
}