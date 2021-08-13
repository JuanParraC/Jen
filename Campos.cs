// Autor : Juan Parra
// 3Soft



namespace Jen
{
    using DbDataReader = System.Data.Common.DbDataReader;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
	using System.Collections;
#if LOGGER
    using NLog;
    using System.Text;
#endif

    /// <summary>
    /// coleccion de campos
    /// </summary>
    [Serializable]
    public class Campos : Recipiente<Campo>
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif        
        #region constructor
            /// <summary>
            /// Constructor por defecto
            /// </summary>
            public Campos() 
                : base()
            {
                Id = Clase = NombreContenido = Lenguaje.Campos;
                Numero = Numero.Plural;
            }
            /// <summary>
            /// Constructor binario
            /// </summary>
            /// <param name="info">información de la serialización</param>
            /// <param name="context">Contexto del proceso de serialización</param>
            protected Campos(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                Clase = NombreContenido = Lenguaje.Campos;
                Numero = Numero.Plural;
            }
        #endregion      


        /// <summary>
        /// crea contenedor de campos claves, necesariamente el campo debe contener un valor
        /// </summary>
        /// <returns>contenedor de campos claves</returns>
        public Recipiente<Campo> Claves(bool valVal = true)
        {
            Recipiente<Campo> _claves = new Recipiente<Campo>();
            //Campo campo;
            //for (int i = 0; i < Largo; i++)
			foreach(Campo campo in Contenido)
			{
                //campo = this[i];
                if (campo.Clave)
                {
                    if (!string.IsNullOrEmpty(campo.Valor) && valVal)
                    {
                        _claves.Agregar(campo);
                    }
                    else
                    {
                        _claves.Agregar(campo);
                    }
                }
            }
            return _claves;

        }
        /// <summary>
        /// Limpia la coleccion de campos
        /// </summary>
        public void Limpiar()
        {
            //limpia los campos
            foreach (Campo c in this.Contenido)
            {
                c.Limpiar();
            }
        }

        /// <summary>
        /// traspasa el valor de datarow a la coleccion de campos
        /// </summary>
        /// <param name="dataRow">Datos</param>
        /// <returns>Campos recuperados</returns>
        internal Campos Recuperar(ILectorBD datos)
        {

            for (int i = 0; i < datos.FieldCount; i++)
            {
#if TRY
                //Jen.Util.ToLog ("pg.txt", "Recuperar", string.Concat(datos.GetName (i), "=", datos.GetValue (i).ToString ()));
                try
                {
#endif
                    string nCmp = datos.GetName(i);
                    //if (this.Existe(nCmp)){
                    object val = datos.GetValue(i);
                    Campo cmp = this[nCmp];
                    //Campo cmp = this[i];
                    if (cmp.Tipo == Tipo.Objeto)
                    {
                        cmp._valor = val;

                    }
                    else
                    {
                        cmp.Valor = val.ToString();
                    }
                    cmp.Ejecutar(Evento.AlCambiarValor);
                //}
#if TRY
                }
                catch (System.Exception e)
                {
                    //Jen.Util.ToLog ("pg.txt", "Error", datos.GetName (i) + e.ToString());
                    _logger.Error(string.Concat(" ", datos.GetName(i), e.ToString()));
                }
#endif
            }
            return this;
        }
    }
}
