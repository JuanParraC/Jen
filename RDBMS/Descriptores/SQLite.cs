#if BaseDeDatoSQLite
using System;
namespace Jen.RDBMS.Descriptores
{
    using Jen;
    using Jen.Json;
    using Jen.Extensiones;
    using System.Text;
    using Mono.Data.Sqlite;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    public class SQLite: DescBD
    {
        public SQLite()
        {
            Id = Clase = Lenguaje.SQLite;
        }

        protected SQLite(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.SQLite;
        }
        public SQLite(Objeto cfg) : base(cfg)
        {
            Id = Clase = Lenguaje.SQLite;
        }
        public SQLite(string id, string driver) : base(id, driver)
        {
            Id = Clase = Lenguaje.SQLite;
        }
        public override void CrearBD(Recipiente<Dato> inst)
        {
            //Compress=True;
            string bd = Util.Configuracion("Directorio") + "Archivo/SQLite/base.db";

            SqliteConnection conexion = new SqliteConnection("Data Source=" + bd + ";New=True;");
            conexion.Open();
            SqliteCommand cmd = new SqliteCommand(conexion);
            foreach (Dato sql in inst)
            {
                cmd.CommandText = sql.Valor;
                cmd.ExecuteNonQuery();
            }
            conexion.Close();
        }
        public override string SQLCreacionSecuencia
        {
            get
            {
                return string.Empty;
            }
        }
        public override void ScriptIndices(Catalogo cat, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;
            foreach (Registro reg in cat.Registros)
            {
                if (reg.Relaciones != null)
                {
                    foreach (Relacion rel in reg.Relaciones)
                    {
                        if (cat.Registros.Existe(rel.Hijo.Id))
                        {
                            Registro regHijo = cat.Registros[rel.Hijo.Id];
                            instr.Agregar(new Dato()
                            {
                                Id = (++iId).ToString(),
                                Valor = string.Concat("create index  ind_", regHijo.Campos[rel.Hijo[0].Id].Nombre, " on ", regHijo.Tabla,
                                                          " (", regHijo.Campos[rel.Hijo[0].Id].Nombre, ");")
                            });
                        }
                    }
                }
            }

        }

        public override string SQLSelectTabDesc
        {
            get
            {
                return @"select tabla as TABLA,
esquema as ESQUEMA,
case when tabla_padre is null then '' else tabla_padre end as TABLA_PADRE,
case when validacionsql is null then '' else validacionsql end as VALIDACIONSQL,
case when identificador is null then '' else identificador end as IDENTIFICADOR,
case when singular is null then '' else singular end as SINGULAR,
case when plural is null then '' else plural end as PLURAL,
case when denominacion is null then '' else denominacion end as DENAMINACION,
case when controlador is null then '' else controlador end as CONTROLADOR,
case when descripcion is null then '' else descripcion end as DESCRIPCION,
menu as MENU,
case when lov is null then '' else lov end as LOV,
case when patronesjs is null then '' else patronesjs end as PATRONESJS,
case when configuracion_controlador is null then '' else configuracion_controlador end as CONFIGURACION_CONTROLADOR,
seq as SEQ
from lib_tab_desc;";
            }
        }
        public override BaseDatos BaseDatos
        {
            get
            {
                return new Jen.RDBMS.SQLite()
                {
                    Id = this.Id,
                    Driver = this.Driver
                };
            }
        }
        public override void DialectoRelaciones(Recipiente<Dato> instr, Recipiente<Dato> tPadre, Recipiente<Dato> thija, params string[] p)
        {

        }
        public override void ScriptComentarios(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
           
        }

        public override void ScriptFunciones(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            
        }

        public override void SQLCreacionBase(Catalogo cat, Objeto cfg, Recipiente<Dato> instr)
        {

        }
        public override Recipiente<Recipiente<Dato>> UPDTabDescSEQ(Catalogo cat)
        {
            return TabDesc;
        }
        public override void ScriptSecuencias(Recipiente<Recipiente<Dato>> tabDesc, Catalogo catDest, Recipiente<Dato> instr)
        {
        }
        public override void DialectoFunciones(Recipiente<Dato> instr, params string[] p)
        {

        }
        public override void ScriptVistasDominio(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;
            // setea el modo de recuperación permitiendo consultar por cualquier campo
            catOrigen.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;

            string[] dominio = new string[] { "Dominio", "Entidad", "DominioAtributo", "EntidadAtributo", "Atributo" };

            foreach (string tab in dominio)
            {
                if (!catOrigen.Registros.Existe(tab))
                {
                    return;
                }
            }
            // apunta a cada tabla a utilizar
            Registro regDom = catOrigen["Dominio"];
            Registro regEnt = catOrigen["Entidad"];
            Registro regDAt = catOrigen["DominioAtributo"];
            // instancia un consultor
            Consultor consultor = new Consultor();
            Recipiente<Recipiente<Dato>> extensiones = new Recipiente<Recipiente<Dato>>();

            // setea el consultado
            consultor.Consultado = catOrigen;
            consultor.Campos.Agregar(
                regDom.Campos["dmId"],
                regDom.Campos["dmNombre"],
                regDom.Campos["dmAtrJson"]);

            regDom.Campos["dmNombre"].Operador = "!=";
            regDom.Campos["dmNombre"].Valor = "Disponible";
            consultor.Criterios = new Recipiente<Campo>();
            consultor.Criterios.Agregar(regDom.Campos["dmNombre"]);
            consultor.Consultado = catOrigen;
            consultor.Recuperar();
            Catalogo regMM;
            string claseBD = catOrigen.BaseDeDatos.Clase;
            string driverBD = catOrigen.BaseDeDatos.Driver;

            string strAtrJson = string.Empty;
            List<object> atrArr = null;


            Objeto atrJson = Objeto.Ref;

            string entidadID = regEnt.Id;
            foreach (Campos Campos in consultor)
            {
                regEnt.Propiedades = new Propiedades();
                regEnt.Propiedades.Add("dom", Campos["dmId"].Valor);
                regEnt.Id = Campos["dmNombre"].Valor;
                strAtrJson = Campos["dmAtrJson"].Valor;

                if (!string.IsNullOrEmpty(strAtrJson))
                {

                    //atrJson = (Objeto)Util.Json(strAtrJson);
                    Compilador<Objeto>.Crear(ref atrJson, strAtrJson);

                    atrArr = (Arreglo)atrJson["Extiende"];

                    //obtener si el dominio tiene atributos
                    regDAt.Campos["daDominio"].Valor = Campos["dmId"].Valor;
                    regDAt.BaseDeDatos = (BaseDatos)Constructor.Embriones[claseBD].Germinar();
                    regDAt.BaseDeDatos.Driver = driverBD;
                    regDAt.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;
                    regDAt.Criterios = new Recipiente<Campo>();
                    regDAt.Criterios.Agregar(regDAt.Campos["daDominio"]);
                    if (regDAt.Recuperar())
                    {
                        //DominioAtrbuto.BaseDeDatos.CerrarConexion();
                        // tiene atributos invocar el dominio
                        foreach (object o in atrArr)
                        {
                            if (!extensiones.Existe(o.ToString()))
                            {
                                Recipiente<Dato> r = new Recipiente<Dato>() { Id = o.ToString() };
                                r.Agregar(new Dato() { Id = Campos["dmId"].Valor, Valor = Campos["dmId"].Valor });
                                extensiones.Agregar(r);
                            }
                            else
                            {
                                extensiones[o.ToString()].Agregar(new Dato() { Id = Campos["dmId"].Valor, Valor = Campos["dmId"].Valor });
                            }
                        }
                    }
                    else
                    {
                        ScriptVistaExtendidas(regEnt, instr, true);
                    }

                }
                else
                {
                    ScriptVistaExtendidas(regEnt, instr, true);
                }

            }
            regEnt.Id = entidadID;
            int numCase = 14;
            Recipiente<Dato> llamadas = new Recipiente<Dato>();
            foreach (Recipiente<Dato> rd in extensiones)
            {
                StringBuilder sb2 = new StringBuilder();
                Dato d;
                for (int i = 0; i < rd.Largo; i++)
                {
                    d = rd[i];
                    sb2.Append(string.Concat(d.Valor, ","));
                    if (((i + 1) % numCase) == 0 || rd.Largo == 1)
                    {
                        sb2.Length--;
                        llamadas.Agregar(new Dato() { Id = string.Concat(rd.Id, i.ToString()),
                            Valor = string.Concat(rd.Id, ".Atributos?", catDestino.BaseDeDatos.Clase, "=true&dmId=[", sb2.ToString(), "]") });
                            //Valor = string.Concat(rd.Id, ".Atributos?dmId=[", sb2.ToString(), "]")
                        //});
                        sb2.Clear();
                    }
                }
                if (sb2.Length > 0)
                {
                    sb2.Length--;
                    llamadas.Agregar(new Dato() { Id = string.Concat(rd.Id), 
                        Valor = string.Concat(rd.Id, ".Atributos?dmId=[", sb2.ToString(), "]") });
                }
            }
            foreach (Dato d in llamadas)
            {
                string url = d.Valor; //string.Concat(d.Id, ".Atributos?dmId=[", d.Valor, "]");
                System.Console.WriteLine(url);
                InvocarURL:
                try
                {
                    regMM = Constructor.Crear<Catalogo>(url, Request);
                    foreach (Registro r in regMM)
                    {
                        ScriptVistaExtendidas(r, instr, true);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch (System.Exception e)
                {
                    //Console.WriteLine(e.Message);

                    goto InvocarURL;
                }
            }
            catOrigen.BaseDeDatos.CerrarConexion();
        }    
    
        public override void ScriptVistaExtendidas(Registro reg, Recipiente<Dato> instr, bool metacampos = false, bool fueraMmo = false)
        {
            int iId = instr.Largo;
            //instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = bdDestino.ComponerSQLInsertar() });
            StringBuilder sb = new StringBuilder();
            if (reg.Tabla.StartsWith("'"))
            {
                sb.Append(string.Concat("create view ", reg.Tabla.Substring(0, 5), "v", reg.Id, "' as ", "select "));
            }

            foreach (Campo cm in reg.Campos)
            {
                if (!cm.ExpresionSql)
                {
                    sb.Append(string.Concat(cm.Nombre, ", "));
                }
                else if (metacampos)
                {
                    sb.Append(string.Concat(cm.Nombre, @" as """, cm.Nombre, @""", "));
                }
            }

            if (reg.Tabla.StartsWith("'mmo.entidad"))
            {
                sb.Length -= 2;
                sb.Append(string.Concat(" from ", reg.Tabla, " "));
                sb.Append(string.Concat("where ('mmo.entidad'.en_dominio=", reg.Propiedades["dom"], ");"));
                instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = sb.ToString() });
                return;
            }
            if (reg.Tabla.StartsWith("'org.inst_elem"))
            {
                sb.Append(string.Concat(" rel.er_padre as ", @"""Padre.Id"",", "rel.tipo as ", @"""Padre.Tipo"""));
                sb.Append(string.Concat(" from ", "'mmo.entidad', ", reg.Tabla,
                                        " left join ( select inst_elem_rel.er_padre, inst_elem_rel.er_hijo, ent.en_valor as tipo from 'org_inst.elem_rel' inst_elem_rel, 'mmo.entidad' ent where (inst_elem_rel.er_relacion = ent.en_id)) rel on inst_elem_rel.ie_id = rel.er_hijo"));
                sb.Append(string.Concat(" where ('mmo.entidad'.en_dominio", "=", reg.Propiedades["dom"], " and 'mmo.entidad'.en_id = 'org.inst_elem'.ie_tipo);"));
                instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = sb.ToString() });
                return;
            }
            // casos simples como cac.PERSONA, enc.PLANIFICACION
            sb.Length -= 2;
            sb.Append(string.Concat(" from ", reg.Tabla, ";"));
            instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = sb.ToString() });
        }    

        public override string SQLCamposTraducidos()
        {
            return string.Empty;
        }

        public override string SQLCatalogoColumna(string tabla, string columna)
        {
            return string.Empty;
        }

        public override string SQLCatalogoColumnas(string tabla, bool orden = true)
        {
            return string.Empty;
        }

        public override string SQLCatalogoTablas()
        {
            return string.Empty;
        }

        public override string SQLCatalogoTablas(Arreglo tablas)
        {
            return string.Empty;
        }

        public override string SQLClavePrimariaTabla(string tabla)
        {
            return string.Empty;
        }

        public override string SQLComentarioColumna(string tabla, string columna, string comentario)
        {
            return string.Empty;
        }

        public override string SQLComentarioTabla(string tabla, string comentario)
        {
            return string.Empty;
        }

        public override string SQLFunciones()
        {
            return string.Empty;
        }

        public override string SQListaFunciones()
        {
            return string.Empty;
        }

        public override string SQLRelaciones(string tabla)
        {
            return string.Empty;
        }
    }
}
#endif