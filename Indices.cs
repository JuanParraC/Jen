#if UsarIndices
namespace Jen
{
    using Serializable = System.SerializableAttribute;
    /// <summary>
    /// coleccion de indices de una tabla
    /// </summary>
    [Serializable]
    public class Indices : Contenedor<Tupla>
    {
        #region inicializa
        /// <summary>
        /// <c>inicializa : </c> método que inicializa el objeto
        /// </summary>
        internal override void inicializa(delegado d)
        {
            base.inicializa(d);
            genero = Genero.masculino;
        }
        #endregion
    }
}
#endif