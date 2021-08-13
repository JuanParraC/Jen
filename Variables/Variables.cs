// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Collections.Generic;
    using Jen.Json;
    using Regex = System.Text.RegularExpressions.Regex;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Implementa una colección de variables dentro de un documento de texto
    /// </summary>
    [Serializable]
    public class Variables : Declaraciones<Variable, Variable>
    {
        /// <summary>
        /// Constructor de variables
        /// </summary>
        /// <param name="code">codigo fuente donde se encuentran las definiciones</param>
        /// <param name="regex">expresión que permite parsear las definiciones</param>
        /// 
        public Variables(string code, Regex regex, int nCharIzq = 2, int nCharDer = 3)
            : base(code, regex, nCharIzq, nCharDer)
        {
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Variables(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Crea la declaracion adecuada para el contenedor
        /// </summary>
        /// <param name="statement">cuerpo de la variable</param>
        /// <returns>Variable</returns>
        public override Variable CrearDeclaracion(string statement)
        {
            return new Variable(statement);
        }
        /// <summary>
        /// Crea el tipo de la variable
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override Variable CrearDeclaracionTipo(Variable statement)
        {
            return statement;
        }
        /// <summary>
        /// Devuelve el contenido mesclado con los elementos variables en un textwriter
        /// </summary>
        /// <param name="tw"></param>
        public void Pintar(System.IO.TextWriter tw)
        {
            // imprime el primer envoltorio de texto de las variables

            tw.Write(Envoltorios[0]);
            // itera sobre el envoltorio de texto de las instancias
            for (int k = 1; k < Envoltorios.Length; k++)
            {
                // direcciona el valor atravez de las iesima instancia 
                Valores[this.ListaDeclaraciones[k - 1].Posicion].Pintar(tw);
                // imprime el iesimo envoltorio de la iesima variable
                tw.Write(Envoltorios[k]);
            }
        }

        /// <summary>
        /// Indexador de variables
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Variable this[string id]
        {
            get { return Valores[Mapa[id]]; }
        }
        /// <summary>
        /// Indica se existe la variable consultada
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Existe(string id)
        {
            return Mapa.ContainsKey(id);
        }
        /// <summary>
        /// Libera memoria
        /// </summary>
        /// <param name="t"></param>
        internal override void Liberar(Variable t)
        {
            t.Borrar();
        }
        public IEnumerator<Variable> GetEnumerator()
        {
            foreach (string k in Mapa.Keys)
            {
                yield return Valores[Mapa[k]];
            }
        }
        public void Persistentes(params string[] ids)
        {
            foreach(string id in ids)
            {
                if (Mapa.ContainsKey(id))
                {
                    this[id].Persistente = true;
                }
            }
        }
    }
}