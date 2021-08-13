namespace Jen.Extensiones
{
    using Jen;
    using System.Text;
    using Ev = System.Environment;
    public static partial class Extensiones
    {
        public static void ScriptBD(this Registro reg, ref StringBuilder sb)
        {
            //sqlScriptTablaTabDesc (ref sbCT);
            sb.Append(string.Concat("create table ", reg.Tabla, "(" + Ev.NewLine).ToLower());
            foreach (Campo cam in reg.Campos)
                cam.ScriptBD(ref sb);
            sb.Length -= 2;
            sb.Append(Ev.NewLine + ");" + Ev.NewLine);

        }
        public static void ScriptVistaExtendidas(this Registro reg, ref StringBuilder sb, bool metacampos = false)
        {

            sb.Append(string.Concat("create view ", reg.Tabla.Substring(0, 4), "v", reg.Id, " as ", Ev.NewLine, "select ").ToLower());
            foreach (Campo cm in reg.Campos)
            {
                if (!cm.ExpresionSql)
                {
                    sb.Append(string.Concat(cm.Nombre, ", ").ToLower());
                }
                else if (metacampos)
                {
                    sb.Append(string.Concat(cm.Nombre.ToLower(), @" as """, cm.Nombre, @""", "));
                }
            }

            if (reg.Tabla.StartsWith("mmo.entidad"))
            {
                sb.Length -= 2;
                sb.Append(string.Concat(Ev.NewLine, "from ", reg.Tabla, Ev.NewLine).ToLower());
                sb.Append(string.Concat("where (mmo.entidad.en_dominio=", reg.Propiedades["dom"], ");", Ev.NewLine, Ev.NewLine));
                return;
            }
            if (reg.Tabla.StartsWith("org.inst_elem"))
            {
                sb.Append(string.Concat(Ev.NewLine, "rel.er_padre as ", @"""Padre.Id"",", "rel.tipo as "));
                sb.Append(@"""Padre.Tipo""");
                sb.Append(string.Concat(Ev.NewLine, "from ", "mmo.entidad, ", reg.Tabla, Ev.NewLine,
                "left join ( select inst_elem_rel.er_padre, inst_elem_rel.er_hijo, ent.en_valor as tipo from org.inst_elem_rel, mmo.entidad ent where (org.inst_elem_rel.er_relacion = ent.en_id)) rel on inst_elem.ie_id = rel.er_hijo",
                    Ev.NewLine));
                sb.Append(string.Concat("where (mmo.entidad.en_dominio", "=", reg.Propiedades["dom"], " and mmo.entidad.en_id = org.inst_elem.ie_tipo);", Ev.NewLine, Ev.NewLine));
                return;
            }

            // casos simples como cac.PERSONA, enc.PLANIFICACION
            sb.Length -= 2;
            sb.Append(string.Concat(Ev.NewLine, "FROM ", reg.Tabla, ";", Ev.NewLine, Ev.NewLine).ToLower());
        }
    }
}