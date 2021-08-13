// Autor : Juan Parra
// 3Soft

namespace Jen.Json
{
    using System.Collections.Generic;
    using Boolean = System.Boolean;
    using Double = System.Double;
    using NumberStyles = System.Globalization.NumberStyles;
    using StringBuilder = System.Text.StringBuilder;


    class RefDefCont{
        public int Items { get; set; }
        public bool ExistenItems { get; set; }
    }
    public static class Compilador<T> where T:IJson 
    {
        public static int TotalByteXEntrada = 24;
        public static void Crear(ref object ret, string json)
        {
            // valida que no sea nulo
            if (!string.IsNullOrEmpty(json))
            {
                // pila para crear la instanciacion de los diferentes objetos que componene el json
                Stack<object> crearJson = new Stack<object>(20);
                // pila para almacenar el dimensionamiento de los diferentes contenedores json
                Stack<RefDefCont> dimRecipientes = new Stack<RefDefCont>(10);
                // indice del iesimo caracter
                int iesimoChar = -1;

                RefDefCont defCont;

                // caracter actual
                int charActual = 0;
                // mientar no sea fin

                while (++iesimoChar < json.Length)
                {
                    // limpia los posibles blancos qe vengan en la cadena
                    Limpiar(json, ref iesimoChar);
                    switch (charActual = JsonToken(json, ref iesimoChar))
                    {
                        // es un objeto
                        case Json.JsonToken.LlaveAbrierta:
                            if (dimRecipientes.Count > 0)
                            {
                                defCont = dimRecipientes.Peek();
                                defCont.ExistenItems = true;
                            }
                            // empila un contador de items para el recipiente
                            dimRecipientes.Push(new RefDefCont() { Items = 1, ExistenItems = false});
                            break;
                        case Json.JsonToken.LlaveCerrada:
                            if (dimRecipientes.Count > 0)
                            {
                                // obtine la dimension del objeto
                                defCont = dimRecipientes.Pop();
                                if (defCont.ExistenItems)
                                {
                                    // verifica q la pila sea mayor a la dimension del arreglo
                                    if (crearJson.Count * 2 >= defCont.Items)
                                    {
                                        // instancia un objeto json
                                        Objeto oJson = new Objeto(defCont.Items);
                                        object[][] elem = new object[defCont.Items][];
                                        // obtiene de la pila todos los elementos y los agrega al objeto
                                        for (int item = 0; item < defCont.Items; item++)
                                        {
                                            //object val = crearJson.Pop();
                                            //object clave = crearJson.Pop();
                                            object[] ojs = new object[2];
                                            ojs[1] = crearJson.Pop();
                                            ojs[0] = crearJson.Pop();

                                            elem[defCont.Items - item - 1] = ojs;

                                            //elem[defCont.Items - item -1][0] = crearJson.Pop();
                                            //oJson.Add(clave.ToString(), val);
                                        }
                                        // revierte el objeto para mantener el orden establecido 
                                        for (int item = 0; item < defCont.Items; item++)
                                        {
                                            oJson.Add(elem[item][0].ToString(), elem[item][1]);
                                        }
                                        // empila el objeto creado
                                        crearJson.Push(oJson);
                                    }
                                    else
                                    {
                                        // error de json
                                        ret = null;
                                        return;
                                    }
                                }
                                else
                                {
                                    crearJson.Push(new Objeto());
                                }
                            }
                            else
                            {
                                // error de json
                                ret = null;
                                return;
                            }
                            break;
                        case Json.JsonToken.BraketAbierto:
                            if (dimRecipientes.Count > 0)
                            {
                                defCont = dimRecipientes.Peek();
                                defCont.ExistenItems = true;
                            }                            
                            // empila un contador de items para el recipiente
                            dimRecipientes.Push(new RefDefCont() { Items = 1, ExistenItems = false });
                            break;
                        case Json.JsonToken.BraketCerrado:
                            if (dimRecipientes.Count > 0)
                            {   
                                // obtine la dimension del arreglo
                                defCont = dimRecipientes.Pop();
                                if (defCont.ExistenItems)
                                {
                                    // verifica q la pila sea mayor o igual a la dimension del arreglo
                                    if (crearJson.Count >= defCont.Items)
                                    {
                                        // crea el arreglo json
                                        Arreglo arrJson = new Arreglo(defCont.Items);
                                        // agrega los elemento al arreglo de la pila
                                        for (int item = 0; item < defCont.Items; item++)
                                        {
                                            arrJson.Add(crearJson.Pop());
                                        }
                                        // revierte el arreglo para mantener el orden establecido 
                                        arrJson.Reverse();
                                        crearJson.Push(arrJson);
                                    }
                                    else
                                    {
                                        // error de json
                                        ret = null;
                                        return;
                                    }
                                }
                                else
                                {
                                    crearJson.Push(new Arreglo());
                                }
                            }
                            else
                            {
                                // error de json
                                ret = null;
                                return;
                            }
                            break;
                        case Json.JsonToken.Coma:
                            if (dimRecipientes.Count > 0)
                            {
								defCont = dimRecipientes.Peek();
                                defCont.Items++;
                                defCont.ExistenItems = true;
                            }
                            else
                            {
                                // error de json
                                ret = null;
                                return;
                            }
                            break;
                        case Json.JsonToken.Texto:
                            crearJson.Push(txt(json, ref iesimoChar));
                            defCont = dimRecipientes.Peek();
                            defCont.ExistenItems = true;
                            break;
                        case Json.JsonToken.Numero:
                            crearJson.Push(num(json, ref iesimoChar));
                            defCont = dimRecipientes.Peek();
                            defCont.ExistenItems = true;
                            break;
                        case Json.JsonToken.Verdadero:
                            crearJson.Push(Boolean.Parse(Lenguaje.True));
                            defCont = dimRecipientes.Peek();
                            defCont.ExistenItems = true;
                            break;
                        case Json.JsonToken.Falso:
                            crearJson.Push(Boolean.Parse(Lenguaje.False));
                            defCont = dimRecipientes.Peek();
                            defCont.ExistenItems = true;
                            break;
                        case Json.JsonToken.Nulo:
                            crearJson.Push(null);
                            defCont = dimRecipientes.Peek();
                            defCont.ExistenItems = true;
                            break;
                        case Json.JsonToken.Nan:
                            crearJson.Push(string.Empty);
                            defCont = dimRecipientes.Peek();
                            defCont.ExistenItems = true;
                            break;
                    }
                }
                if (crearJson.Count == 1)
                {
                    ret = crearJson.Pop();
                    return;
                }
            }
            ret = null;
        }
        public static void Crear(ref T ret, string json)
        {
            object o = new object();
            Crear(ref o, json);
            if (o == null)
            {
                ret = default(T);
                return;
            }

            if (o.GetType() == typeof(T))
            {
                ret = (T)o;
                ret.TamSer = json.Length;
                return;
            }
            ret = default(T);
        }

        public static string Serializar()
        {
            return string.Empty;
        }

        static string txt(string json, ref int indice)
        {
            StringBuilder sb = new StringBuilder(TotalByteXEntrada);
            char chActual, chSepTexto;

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
            indice--;
            //Util.ToLog(string.Concat(sb.Length.ToString(),"\t",sb.Capacity.ToString(),"\n"));
            return sb.ToString();
        }

        static decimal num(string json, ref int index)
        {
            Limpiar(json, ref index);

            int iFinNum = finNum(json, index);
            int largoTexto = (iFinNum - index) + 1;
            char[] arrChar = new char[largoTexto];
            string sNum = json.Substring(index, largoTexto);

            //Array.Copy(json, index, arrChar, 0, largoTexto);
            index = iFinNum;
            decimal dbl;
            decimal.TryParse(sNum, out dbl);
            return dbl;
            //return Double.Parse(new string(arrChar), Util.Cultura);
            //return Double.Parse(sNum, Util.Cultura.NumberFormat);
            
        }

        public static int JsonToken(string json, ref int indice)
        {
            if (json.Length == indice)
            {
                return Json.JsonToken.Blanco;
            }
            char c = json[indice];
            switch (c)
            {
                case CaracteresJson.llaveAbierta:
                    return Json.JsonToken.LlaveAbrierta;
                case CaracteresJson.llaveCerrada:
                    return Json.JsonToken.LlaveCerrada;
                case CaracteresJson.braketAbierto:
                    return Json.JsonToken.BraketAbierto;
                case CaracteresJson.braketCerrado:
                    return Json.JsonToken.BraketCerrado;
                case CaracteresJson.coma:
                    return Json.JsonToken.Coma;
                case CaracteresJson.comillaDoble:
                case CaracteresJson.comilla:
                    return Json.JsonToken.Texto;
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
                    return Json.JsonToken.Numero;
                case CaracteresJson.dosPuntos:
                    return Json.JsonToken.DosPuntos;
            }
            //indice--;
            int largoResto = json.Length - indice;
            if (largoResto >= 5)
            {
                if (json[indice] == CaracteresJson.f &&
                    json[indice + 1] == CaracteresJson.a &&
                    json[indice + 2] == CaracteresJson.l &&
                    json[indice + 3] == CaracteresJson.s &&
                    json[indice + 4] == CaracteresJson.e)
                {
                    indice += 4;
                    return Json.JsonToken.Falso;
                }
            }
            if (largoResto >= 4)
            {
                if (json[indice] == CaracteresJson.t &&
                    json[indice + 1] == CaracteresJson.r &&
                    json[indice + 2] == CaracteresJson.u &&
                    json[indice + 3] == CaracteresJson.e)
                {
                    indice += 3;
                    return Json.JsonToken.Verdadero;
                }
            }
            if (largoResto >= 4)
            {
                if (json[indice] == CaracteresJson.n &&
                    json[indice + 1] == CaracteresJson.u &&
                    json[indice + 2] == CaracteresJson.l &&
                    json[indice + 3] == CaracteresJson.l)
                {
                    indice += 3;
                    return Json.JsonToken.Nulo;
                }
            }
            if (largoResto >= 3)
            {
                if (json[indice] == CaracteresJson.N &&
                    json[indice + 1] == CaracteresJson.a &&
                    json[indice + 2] == CaracteresJson.N)
                {
                    indice += 2;
                    return Json.JsonToken. Nan;
                }
            }
            return Json.JsonToken.Blanco;
        }

        static int finNum(string json, int indice)
        {
            int ind;
            for (ind = indice; ind < json.Length; ind++)
                if (Lenguaje.Numeros.IndexOf(json[ind]) == -1)
                    break;
            return ind - 1;
        }

        public static void Limpiar(string json, ref int indice)
        {
            for (; indice < json.Length; indice++)
                if (Lenguaje.Blancos.IndexOf(json[indice]) == -1)
                    break;
        }
    }
}