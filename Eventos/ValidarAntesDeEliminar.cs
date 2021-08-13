// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Evento que valida la integridad referencial al momento de eliminar un registro
    /// </summary>
    [Serializable]
    public class ValidarAntesDeEliminar : Evento<Registro>
    {
        /// <summary>
        /// Constructor del evento
        /// </summary>
        public ValidarAntesDeEliminar()
            : base()
        {
            Id = Clase = Lenguaje.ValidarAntesDeEliminar;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">Propiedades serializadas</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected ValidarAntesDeEliminar(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ValidarAntesDeEliminar;
        }

        /// <summary>
        /// Punto de entrada para el evento de validación
        /// </summary>
        /// <returns></returns>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {

            if (Contexto.Relaciones == null)
            {
                return;
            }
            // direcciona al registro
            Registro regPadre = Contexto;
            // direcciona al catalogo
            Catalogo cat = (Catalogo)regPadre.Padre;
            // instancia un StringBuilder
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // Instancia un consultor para obtener las posibles referencias del registro
            Consultor existenReferencias = new Consultor();
            // recupera el contexto de la base de datos
            //IConsultable contextoBD = regPadre.BaseDeDatos.Contexto;
            // accede a la base de datos definida en el registro

            BaseDatos bd;
#if MultipleActiveResultSets
            bd = regPadre.BaseDeDatos;
#else
            bd = (Jen.BaseDatos)Constructor.Embriones[regPadre.BaseDeDatos.Clase].Germinar();
            bd.Driver = regPadre.BaseDeDatos.Driver;
#endif 
            existenReferencias.BaseDeDatos = bd;
            // define los eventos del registro que obtiene las posibles referencias
            existenReferencias.Agregar(new Jen.Eventos.CargarPrimerRegistro(), Evento.DespuesDeRecuperar);
            // instancia un campo para obtener las posibles referencias del registro padre
            Campo cuentaReferencias = new Campo()
            {
                Id = Lenguaje.Cuenta,
                ExpresionSql = true
            };
            // declara un valor para obtener las posibles referencias del registro padre
            Valor vc = new Valor();
            vc.Contexto = cuentaReferencias;
            cuentaReferencias._valor_ = vc.Id;
            cuentaReferencias.Agregar(vc, Evento.AlCambiarValor);
            existenReferencias.Campos.Agregar(cuentaReferencias);

            // recorre cada relacion de la tabla
            foreach (Relacion rel in regPadre.Relaciones)
            {
                Registro regHijo = cat.Registros[rel.Hijo.Id];
                // setea el campo para obtener el numero de registros hijos
                cuentaReferencias.Nombre = Lenguaje.Count + Lenguaje.AbreParentesis + regHijo.Campos[0].Nombre + Lenguaje.CierreParentesis;
                cuentaReferencias.Valor = Lenguaje.Cero;
                //Compone el sql para obtener las posibles referencias hijas
                sb.Length = 0;
                sb.Append(Lenguaje.SelectCount);
                sb.Append(regPadre.BaseDeDatos.SeparadorAlias);
                sb.Append(Lenguaje.CampoCuenta);
                sb.Append(Lenguaje.From);
                sb.Append(regHijo.Tabla);
                sb.Append(Lenguaje.Where);
                int iCampo = 0;
                // compone el sql para obtener las referencias
                foreach (Semilla cm in rel.Hijo)
                {
                    sb.Append(Lenguaje.AbreParentesis);
                    sb.Append(regHijo.Campos[cm.Id].Nombre);
                    sb.Append(Lenguaje.Igual);
                    sb.Append(regPadre.Campos[rel.Madre[iCampo++].Id].ToString());
                    sb.Append(Lenguaje.CierreParentesis);
                    sb.Append(Lenguaje.And);
                }
                // elimina el ultimo and de la consulta
                sb.Length -= Lenguaje.And.Length;
                existenReferencias.SQL = sb.ToString();
                // recupera conforme al sql generado
                if (existenReferencias.Recuperar())
                {
                    if (!existenReferencias.Campos[0].Valor.Equals(Lenguaje.Cero))
                    {
                        Contexto.Estado |= Estado.Error;
                        regPadre.Consejos = string.IsNullOrEmpty(Consejo) ? string.Concat(Lenguaje.RegistroPadreConReferencias,
                                regHijo.Etiqueta, Lenguaje.NoFuePosibleEliminar, Lenguaje.En,
                                regPadre.Articulo.ToLower(Util.Cultura), Lenguaje.Espacio, regPadre.Clase.ToLower(Util.Cultura), Lenguaje.Espacio, regPadre.Etiqueta) : Consejo;
                        break;
                    }
                }
                existenReferencias.BaseDeDatos.CerrarConexion();
            }
            // restaura el contexto de la base de datos
            //regPadre.BaseDeDatos.Contexto = contextoBD;
        }
    }
}