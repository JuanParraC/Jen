// Autor : Juan Parra
// 3Soft
namespace Jen.Eventos
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    /// <summary>
    /// Determina el numero de registros recuperador por el consultor
    /// </summary>
    [Serializable]
    public class ContarRegistros : Evento<Consultor>
    {
        /// <summary>
        /// Constructor del evento
        /// </summary>
        public ContarRegistros()
        {
            Id = Clase = Lenguaje.ContarRegistros;
        }
        /// <summary>
        /// Constructor binario del evento
        /// </summary>
        /// <param name="info">propiedades serializadas</param>
        /// <param name="context">contexto del proceso de deserialización</param>
        protected ContarRegistros(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.ContarRegistros;
        }
        /// <summary>
        /// Punto de entrada del evento
        /// </summary>
        //public override void Ejecutar(Evento ev)
        public override void Ejecutar()
        {
            //direcciona el consultor
            Consultor consultor = Contexto;
            // direcciona el consultor que provee la consulta para determinar la cantidad de registros
            //recuperados
            Consultor contador = consultor._contador;
            // verifica si el contador es provisto por el framework o programaticamente
            if (consultor._contador == null)
                // determina el numero total de registros de la consulta
                contador = consultor._contador = new Consultor(consultor.BaseDeDatos.SqlContador);
            //agrega el evento para que se poble el valor despues de recuperar la consulta del conteo de registros
            contador.Agregar(new Jen.Eventos.CargarPrimerRegistro(), Evento.DespuesDeRecuperar);
            // declara un campo para obtener la cantidad de registros
            Campo cuenta = new Campo()
            {
                Id = Lenguaje.Cuenta,
                Nombre = Lenguaje.Count + Lenguaje.AbreParentesis + consultor.Campos[0].Nombre + Lenguaje.CierreParentesis,
                ExpresionSql = true
            };
            // agrega el campo para obtener el numero de registros de la consulta
            contador.Campos.Agregar(cuenta);
            // inicializa el valor a cero
            contador.Campos[Lenguaje.Cuenta].Valor = Lenguaje.Cero;
            // establece el driver de base de datos para la consulta
#if MultipleActiveResultSets
            contador.BaseDeDatos = consultor.BaseDeDatos;
#else
            contador.BaseDeDatos = (BaseDatos)Constructor.Embriones[consultor.BaseDeDatos.Clase].Germinar();
            contador.BaseDeDatos.Driver = consultor.BaseDeDatos.Driver;
#endif
        }
    }
}