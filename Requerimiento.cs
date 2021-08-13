// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Collections;
    using Jen.Json;
    using HttpRequest = System.Web.HttpRequest;
    using NameValueCollection = System.Collections.Specialized.NameValueCollection;
    using Serializable = System.SerializableAttribute;

    /// <summary>
    /// Clase para preprocesar request en parametros y campos
    /// </summary>
    [Serializable]
    public class Requerimiento
    {
        private static string[] pJen = { Lenguaje.Pagina,Lenguaje.Inicio, Lenguaje.Limite, Lenguaje.SeparadorDecimal, 
            Lenguaje.Sort, Lenguaje.Dir, Lenguaje.SQLite, Lenguaje.PostgreSQL };
        NameValueCollection nvcRequest = null;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="HttpRequest"></param>
        public Requerimiento(HttpRequest HttpRequest)
        {
            nvcRequest = new NameValueCollection();

            // determina donde vienen los datos de la solicitud
            if (HttpRequest.QueryString.Count > 0)
            {
                nvcRequest = HttpRequest.QueryString;
            }
            if (HttpRequest.Form.Count > 0)
            {
                nvcRequest = HttpRequest.Form;
            }

            Inicializa(nvcRequest);
        }
        
        public Requerimiento(string strRequest)
        {
            nvcRequest = new NameValueCollection();
            if (!string.IsNullOrEmpty(strRequest))
            {

                string[] arrItemRequest = strRequest.Split(Lenguaje.Arroba.ToCharArray());
                foreach (string strIR in arrItemRequest)
                {
                    string[] nvItem = strIR.Split(Lenguaje.Igual.ToCharArray());
                    nvcRequest.Add(nvItem[0], nvItem[1]);
                }
            }

            Inicializa(nvcRequest);
        }
        void Inicializa(NameValueCollection nvcRequest)
        {
            // crea las colección de parametros
            _parametros = new NameValueCollection(pJen.Length);

            // carga los parametros Jen
            string p;
            for (int i = 0; i < pJen.Length; i++)
            {
                p = pJen[i];
                if (nvcRequest[p] != null)
                {
                    _parametros.Add(p, nvcRequest[p]);
                }
            }
            // crea las colección de campos
            _campos = new NameValueCollection(nvcRequest);

            // elimina los parametros
            for (int i = 0; i < pJen.Length; i++)
            {
                p = pJen[i];
                _campos.Remove(p);
            }
        }

        public string this[string id]
        {
            get
            {
                return nvcRequest[id];
            }
        }
        public IEnumerator GetEnumerator()
        {
            return nvcRequest.GetEnumerator();
        }
        #region propiedades

        #region parametros
        private NameValueCollection _parametros;
                /// <summary>
                /// coleccion de parametros
                /// </summary>
                public NameValueCollection Parametros
                {
                    get { return _parametros;}
                }
            #endregion

            #region campos
                private NameValueCollection _campos;
                /// <summary>
                /// coleccion de campos
                /// </summary>
                public NameValueCollection Campos
                {
                    get { return _campos; }
                }
        #endregion

        #endregion propiedades

        public Objeto Json
        {
            get
            {
                Objeto ret = new Objeto(_campos.Count + _parametros.Count);
                if (_campos.Count > 0)
                {
                    //Objeto campos = new Objeto(_campos.Count);
                    //ret.Add("Campos", campos);
                    foreach (string o in _campos.AllKeys)
                    {
                        if (!string.IsNullOrEmpty(o))
                        {
                            ret.Add(o, _campos[o]);
                        }
                    }

                }
                if (_parametros.Count > 0)
                {
                    //Objeto parametros = new Objeto(_parametros.Count);
                    //ret.Add("Parametros", parametros);
                    foreach (string o in _parametros.AllKeys)
                    {
                        if (!string.IsNullOrEmpty(o))
                        {
                            ret.Add(o, _parametros[o]);
                        }
                    }
                }
                return ret;                
/*
                Objeto ret = new Objeto(2);
                if (_campos.Count > 0)
                {
                    Objeto campos = new Objeto(_campos.Count);
                    ret.Add("Campos", campos);
                    foreach (string o in _campos.AllKeys)
                    {
                        campos.Add(o, _campos[o]);
                    }

                }
                if (_parametros.Count > 0)
                {
                    Objeto parametros = new Objeto(_parametros.Count);
                    ret.Add("Parametros", parametros);
                    foreach (string o in _parametros.AllKeys)
                    {
                        parametros.Add(o, _parametros[o]);
                    }
                }
                return ret;
*/
            }
        }
    }
}
