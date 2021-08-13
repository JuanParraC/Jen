// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Web;
    using ISerializable = System.Runtime.Serialization.ISerializable;
    using IXmlSerializable = System.Xml.Serialization.IXmlSerializable;
    /// <summary>
    /// Interface que debe ser implementada por cualquier objeto Jen
    /// </summary>
    public interface ISemilla : IXmlSerializable, ISerializable
    {
        /// <summary>
        /// clase del objeto
        /// </summary>
        string Clase { get; }
        /// <summary>
        /// almacena el estado en que se encuenta el objeto
        /// </summary>
        Estado Estado { get; set; }

#if RuntimeCache
        HttpRequest Request { get; set; }
#endif
        /// <summary>
        /// id es el identificador del objeto
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// almacena quien es el madre del objeto
        /// </summary>
        ISemilla Padre { get; set; }
        /// <summary>
        /// Metodo que deja el objeto en estado inicial listo para ser utilizado
        /// </summary>
        void Restaurar();
        /// <summary>
        /// Método que crea la grafica declarada en las variable, crea el objeto grafico 
        /// y establece el contexto de este con el objeto semilla
        /// </summary>
        /// <param name="variables">colección de variables</param>
        void EnlazarGrafica(Variables variables, HttpRequest request = null);
        /// <summary>
        /// Método que extrae la info mediante la asignación de variables
        /// </summary>
        /// <param name="variables">colección de variables</param>
        void MesclarContenido(Variables variables);
    }
}
