namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    /// <summary>
    /// representa una porcion de codigo fuente en algun lenguaje
    /// </summary>
    [Serializable]
    public class Script : Dato<string>
    {
        #region propiedades

            #region evento
                private string _evento;
                /// <summary>
                /// evento : evento asociado al script
                /// </summary>
                public string evento 
                {
                    get { return _evento; }
                    set 
                    {
                        string ev = _evento;
                        if (!atributos.Contains(claveAtributo.evento))
                            respaldar(claveAtributo.evento, delegate() { _evento = ev; });
                    	_evento = value; 
                    }

                }
            #endregion

        #endregion propiedades
    }
}
