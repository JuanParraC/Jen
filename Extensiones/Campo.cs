
namespace Jen.Extensiones
{
    using Jen;
    using System;
    using System.Text;
    using Ev = System.Environment;
    public static partial class Extensiones
    {
        public static void ScriptBD(this Campo cam, ref StringBuilder sb)
        {
            sb.Append(string.Concat(Convert.ToChar(9), cam.Nombre.ToLower(), " "));
            int largoTexto = int.Parse(cam.Largo);
            if (largoTexto <= 0)
                largoTexto = 10485760;

            /*if (cam.Nombre.Equals("IA_DESCRIPCION"))
            {
                int i = 0;
                //cam.Propiedades
            }*/
            string valDefecto = string.Empty;

            if (cam.Propiedades != null)
            {

                if (cam.Propiedades.ContainsKey("Defecto"))
                {
                    valDefecto = string.Concat("default ", cam.Propiedades["Defecto"]);
                }
            }
            string val = string.Empty;
            switch (cam.Tipo)
            {
                case Tipo.Texto:

                    if (cam.Propiedades != null)
                    {
                        if (cam.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            sb.Append(string.Concat(" ", val, " ").ToLower());
                        }
                        else
                        {
                            sb.Append(string.Concat(" varchar(", largoTexto.ToString(), ")").ToLower());
                        }
                    }
                    else
                    {
                        sb.Append(string.Concat(" varchar(", largoTexto.ToString(), ")").ToLower());
                    }
                    break;
                case Tipo.Numerico:
                    sb.Append(string.Concat(" numeric(", largoTexto.ToString(), ")").ToLower());
                    break;
                case Tipo.Fecha:
                    if (cam.Propiedades != null)
                    {
                        if (cam.Propiedades.TryGetValue("SubTipo", out val))
                        {
                            sb.Append(string.Concat(" ", val, " ").ToLower());
                        }
                    }
                    else
                    {
                        sb.Append(" date");
                    }
                    break;
            }
            if (cam.Clave)
            {
                sb.Append(string.Concat(" constraint pk_", cam.Tabla.Split('.')[1], " primary key").ToLower());
            }
            else if (cam.Existe("ValidarRequerido"))
            {
                if (cam.Unico)
                {
                    sb.Append(" unique ");
                }
                sb.Append(" not null");

            }
            sb.Append(string.Concat(" ", valDefecto, ",", Ev.NewLine));
        }
    }
}

