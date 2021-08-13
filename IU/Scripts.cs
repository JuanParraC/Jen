namespace Jen.IU
{
    using Serializable = System.SerializableAttribute;
    /// <summary>
    /// Contenedor de scripts
    /// </summary>
    [Serializable]
    public class Scripts : Contenedor<IU.Script>, IEjecutarEvento
    {
            // contenedor de objetos graficos disponibles
            #if ApConsola
                internal static Scripts interfaseUsuario = Constructor.deserializar<Scripts>(cadena.nombreArchivoObjetosGraficos);
            #else
                internal static Scripts interfaseUsuario = Constructor.deserializar<Scripts>(cadena.nombreArchivoObjetosGraficos);
            #endif
            #region delegados
                private Delegados<Scripts> _delegados;
                /// <summary>
                /// <c>objetoGrafico : </c>
                /// </summary>   
                public Delegados<Scripts> delegados
                {
                    get 
                    {
                        return _delegados; 
                    }
                    set 
                    { 
                        _delegados = value;
                        _delegados.padre = this;
                        _delegados.setearContexto(this);
                    }
                }
            #endregion

            #region ejecutarEvento
                /// <summary>
                /// <c>ejecutarEvento : </c> Permite ejecutar los eventos del objeto
                /// </summary>  
                public void ejecutarEvento(Evento evento)
                {
                    if (_delegados == null) return;
                    // ejecuta todos los delegados subcritos al evento
                    _delegados.cada(delegate(Metodo<Scripts> m)
                       {
                            if (evento == m.evento)  m.ejecutar();
                       }
                    );
                }
            #endregion
    }
}
