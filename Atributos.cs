// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using Serializable = System.SerializableAttribute;
    /// <summary>
    /// Atributos tiene el proposito de obtener un estado inicial y deseable de un semilla 
    /// para su posterior restauración, asumiendo que el semilla pasa por una serie de 
    /// modificaciones 
    /// </summary>
    [Serializable]
    internal sealed class Atributos : System.Collections.Generic.List<Atributo>
    {
        #region Campos
        
        /// <summary>
        /// Lista de delegados que realizaran la restauración de los objetos
        /// </summary>
        internal System.Collections.Generic.IList<Proc> Restauradores = new System.Collections.Generic.List<Proc>();
        #endregion

        #region Propiedades
        /// <summary>
        /// Propiedad que enlaza el padre de los atributos
        /// </summary>
        ISemilla _padre;
        public ISemilla Padre
        {
            set
            {
                _padre = value;
            }
        }
        #endregion
        #region Metodos
        /// <summary>
        /// Permite incorporar un delegado de restauración para una propiedad en particular
        /// </summary>
        /// <param name="atributo">identificador del atributo</param>
        /// <param name="restaurador">proceso que realiza la reparación del objeto</param>
        public void Agregar(Atributo atributo, Proc restaurador)
        {
            base.Add(atributo);
            Restauradores.Add(restaurador);
        }
        /// <summary>
        /// Comprobación si el objeto está en un estado respaldable
        /// </summary>
        /// <param name="Atributo">atributo a respaldar</param>
        /// <returns></returns>
        public bool Respaldable(Atributo Atributo)
        {
            if (!base.Contains(Atributo))
                if (_padre != null)
                    if (Semilla.Ocupado(_padre))
                        return true;
            return false;
        }
        /// <summary>
        /// ejecuta la restauración del objeto
        /// </summary>
        public void Restaurar()
        {
            // restaura lo ultimo hasta el final asegurando que el estado sea el ultimo atributos a restaurar
            for (int i = Restauradores.Count - 1; i >= 0; i--)
				Restauradores[i]();
			/*foreach (Proc proc in Restauradores)
				proc ();
              */  
            // limpia los restauradores
            Restauradores.Clear();
            // limpia los atributosin 
            Clear();
        }
        #endregion
    }
}