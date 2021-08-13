// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Instancias representa una lista de instancias de una variable de tipo string
    /// </summary>
    [Serializable]
    public class Instancias : Declaraciones<System.Collections.Generic.List<string>, Declaracion>
    {
        public Instancias(string code)
            : base(code, new Regex(Lenguaje.RegExpInstancias, RegexOptions.Compiled))
        {
        }
        public Instancias(string code, Regex regex)
            : base(code, regex)
        {
        }
        protected Instancias(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public override Declaracion CrearDeclaracion(string statement)
        {
            return new Declaracion() { Id = statement, Identificadores = statement.Split(new string[] { Lenguaje.Punto }, System.StringSplitOptions.None) };
        }
        public override System.Collections.Generic.List<string> CrearDeclaracionTipo(Declaracion statement)
        {
            return new System.Collections.Generic.List<string>();
        }
        internal override void Liberar(System.Collections.Generic.List<string> t)
        {
            t.Clear();
        }
    }
}