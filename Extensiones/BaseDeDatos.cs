namespace Jen.Extensiones
{
    using Jen.RDBMS.Descriptores;
    public static partial class Extensiones
    {
        public static DescBD Descriptor(this BaseDatos bd, string id)
        {
            //if (bd.Clase.Equals ("SQLServer"))
            //  return new Descriptores.SQLServer (id, bd.Driver);
            return new PostgreSQL(id, bd.Driver);
        }
    }
}
