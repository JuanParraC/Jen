
namespace Jen.RDBMS.Descriptores
{
    using Jen;
    using Jen.Json;
    using Jen.Extensiones;
    using System.Text;
    //using Ev = System.Environment;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    public class PostgreSQL : DescBD
    {

        public PostgreSQL()
        {
            Id = Clase = Lenguaje.PostgreSQL;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected PostgreSQL(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.PostgreSQL;
        }
        public PostgreSQL(Objeto cfg) : base(cfg)
        {
            Id = Clase = Lenguaje.PostgreSQL;
        }
        public PostgreSQL(string id, string driver) : base(id, driver)
        {
            Id = Clase = Lenguaje.PostgreSQL;
        }
        public override BaseDatos BaseDatos
        {
            get
            {
                return new Jen.RDBMS.PostgreSQL() { Id = this.Id, Driver = this.Driver };
            }
        }

        public override string SQLSelectTabDesc
        {
            get
            {
                return @"select tabla as ""TABLA"",
esquema as ""ESQUEMA"",
case when tabla_padre is null then '' else tabla_padre end as ""TABLA_PADRE"",
case when validacionsql is null then '' else validacionsql end as ""VALIDACIONSQL"",
case when identificador is null then '' else identificador end as ""IDENTIFICADOR"",
case when singular is null then '' else singular end as ""SINGULAR"",
case when plural is null then '' else plural end as ""PLURAL"",
case when denominacion is null then '' else denominacion end as ""DENAMINACION"",
case when controlador is null then '' else controlador end as ""CONTROLADOR"",
case when descripcion is null then '' else descripcion end as ""DESCRIPCION"",
menu as ""MENU"",
case when lov is null then '' else lov end as ""LOV"",
case when patronesjs is null then '' else patronesjs end as ""PATRONESJS"",
case when configuracion_controlador is null then '' else configuracion_controlador end as ""CONFIGURACION_CONTROLADOR"",
seq as ""SEQ""
from lib.tab_desc;";
            }
        }
        public override string SQLCreacionSecuencia
        {
            get
            {
                return "select 'create sequence ' || ESQUEMA || 'SEQ_' || TABLA || '   start with ' || to_char(SEQ, '999999999999') || ' increment by  1;' as  \"dmNombre\" from " + TabDesc["tab_desc"]["ESQUEMA"].Valor + "TAB_DESC";
            }
        }
        public override string SQLCatalogoTablas()
        {

            return @"select (tab.table_name), obj_description(obj.oid)
from pg_class as obj join information_schema.tables as tab on obj.relname = tab.table_name join lib.tab_desc as tabdesc on tabdesc.tabla = (tab.table_name) 
where obj.relname = tab.table_name and lower(tab.table_schema) in (" + Esquemas.ValorQuery() + ") and tab.table_type ='BASE TABLE' and  obj_description(obj.oid) <> '' order by tabdesc.identificador asc";
        }
        public override string SQLCatalogoTablas(Arreglo tablas)
        {
            StringBuilder sb = new StringBuilder(tablas.Count * 10);
            sb.Append(SQLCatalogoTablas() + " and (tab.table_name) in(");
            foreach (object dt in tablas)
            {
                sb.Append(string.Concat("'", dt.ToString(), "',"));
            }
            sb.Length--;
            sb.Append(")");
            return sb.ToString();
        }
        public override string SQLCamposTraducidos()
        {
            return @"select (tc.table_name || '.' || kcu.column_name) as ""CampoTraducido""
from information_schema.table_constraints as tc 
join information_schema.key_column_usage as kcu
on tc.constraint_name = kcu.constraint_name
join information_schema.constraint_column_usage as ccu
on ccu.constraint_name = tc.constraint_name
where constraint_type = 'FOREIGN KEY' and lower(tc.table_schema) in (" + Esquemas.ValorQuery() + ") and ccu.table_name <> tc.table_name -- excluye relaciones reflexivas";
        }
        public override string SQLCatalogoColumna(string tabla, string columna)
        {
            return SQLCatalogoColumnas(tabla, false) + " and lower(cols.column_name) = lower('" + columna + "')";
        }
        public override string SQLCatalogoColumnas(string tabla, bool orden = true)
        {
            string[] ids = tabla.ArrIDs();
            string ord = orden ? " order by 1 " : string.Empty;
            //
            return
                @"select (cols.column_name) as ""Nombre"", 
                case when cols.udt_name = 'numeric' then 'NUMBER' when cols.udt_name = 'varchar' then 'VARCHAR2' else upper(cols.data_type) end as ""Tipo"", 
                case when cols.udt_name = 'numeric' then  cols.numeric_precision when cols.udt_name = 'varchar' then character_maximum_length else 8 end  as ""Largo"",
                case when is_nullable = 'NO' then 'F' else 'T' end  as ""Nulable"",
                case when cols.column_name in (
                    select array_to_string(array_agg(c.attname),'') as const_columns
                    from pg_constraint a, pg_class b, pg_attribute c
                    where
                        a.conrelid = b.oid
                        and a.contype  = 'u'
                        and c.attrelid = b.oid
                        and c.attnum in (select unnest(a.conkey))
                    group by b.relname, a.conname 
                ) then 'T' else 'F' end as ""Unico"",
                (select pgd.description
                from pg_catalog.pg_statio_all_tables as st
                  inner join pg_catalog.pg_description pgd on (pgd.objoid=st.relid)
                  inner join information_schema.columns c on (pgd.objsubid=c.ordinal_position
                    and  c.table_schema=st.schemaname and c.table_name=st.relname)
                    where c.column_name = cols.column_name limit 1) as ""Descripcion"",
                case when cols.is_identity = 'NO' then 'F' else 'T' end as ""Autonumerico"",
                case when cols.numeric_precision is null then 0 else cols.numeric_precision end as ""PREC"",
                case when cols.numeric_scale is null then 0 else cols.numeric_scale end  as ""SCALE"",
                cols.column_default as ""ValorDefecto""
                from information_schema.columns as cols, information_schema.tables as tabs
                where tabs.table_schema in (" + Esquemas.ValorQuery() + ") and tabs.table_schema = cols.table_schema and tabs.table_name = cols.table_name and lower(tabs.table_name)= lower('" + ids[1] + "') " + ord;

        }
        public override string SQLComentarioTabla(string tabla, string comentario)
        {
            return string.Concat("comment on table ", tabla.ToLower(), " is '", comentario, "';");
        }
        public override string SQLComentarioColumna(string tabla, string columna, string comentario)
        {
            return string.Concat("comment on column ", tabla.ToLower(), ".", columna, " is '", comentario, "';");
        }
        public override string SQLClavePrimariaTabla(string tabla)
        {
            string[] ids = tabla.ArrIDs();
            return @"select tab.schemaname as ""TABLE_QUALIFIER"", tab.schemaname as ""TABLE_OWNER"", (tab.tablename) as ""TABLE_NAME"", (atr.attname) as ""COLUMN_NAME"", 1 as ""KEY_SEQ"", (obj.relname) as ""PK_NAME""
                    from   pg_index ind 
                    join   pg_attribute atr on atr.attrelid = ind.indrelid and atr.attnum = ANY(ind.indkey)
                    join (select o.oid, t.tablename, t.schemaname, t.tableowner from pg_class o, pg_tables t where o.relname = t.tablename and lower(t.schemaname) in(" + Esquemas.ValorQuery() + @")) tab on tab.oid = ind.indrelid
                    join pg_class obj on ind.indexrelid = obj.oid 
                    where   ind.indisprimary and lower(tab.schemaname) in(" + Esquemas.ValorQuery() + ") and lower(tab.tablename) = lower('" + ids[1] + "')";
        }
        public override string SQLRelaciones(string tabla)
        {
            string[] ids = tabla.ArrIDs();
            return
                @"
select c.oid as ""constid"", (ccu.table_name) as ""TablaPadre"", pkdef.pkkey as ""ColumnaPadre"",  (tc.table_name) as ""TablaHija"", (array_to_string(array_agg(a.attname),'')) as ""ColumnaHija"",
(c.conname) as ""NombreRelacion"", 'NO ACTION' as ""UPDATE_RULE"", 'NO ACTION' as ""DELETE_RULE""-- c.confupdtype || '?a', c.confdeltype
--case when c.confupdtype = 'a' then 'NO ACTION' when c.confupdtype = 'r' then 'RESTRICT' when c.confupdtype = 'c' then 'CASCADE' else c.confupdtype end   as ""UPDATE_RULE"",
--case when c.confdeltype = 'a' then 'NO ACTION' when c.confdeltype = 'r' then 'RESTRICT' when c.confdeltype = 'c' then 'CASCADE' else c.confdeltype end   as ""DELETE_RULE""
from information_schema.table_constraints tc 
join information_schema.constraint_column_usage ccu 
    on tc.constraint_catalog=ccu.constraint_catalog and tc.constraint_schema = ccu.constraint_schema and tc.constraint_name = ccu.constraint_name and lower(ccu.table_name) =lower('" + ids[1] + @"') 
join pg_constraint c on  ccu.constraint_name = c.conname 
join pg_class cl on c.conrelid = cl.oid 
join pg_attribute a on a.attrelid = cl.oid and a.attnum in (select unnest(c.conkey))
join (
    select (atr.attname) pkkey, tab.tablename tab
    from pg_index ind 
    join pg_attribute atr ON atr.attrelid = ind.indrelid AND atr.attnum = ANY(ind.indkey)
    join pg_class cls on cls.oid = ind.indrelid 
    join pg_tables tab on cls.relname = tab.tablename --and lower(tab.schemaname) in(" + Esquemas.ValorQuery() + @") --and tab.tablename =ccu.table_name
    where ind.indisprimary
) pkdef on pkdef.tab = ccu.table_name
where lower(tc.constraint_type) in ('foreign key')  --and (lower(tc.table_schema) in(" + Esquemas.ValorQuery() + @"))
and ccu.table_name <> tc.table_name -- excluye relaciones reflexivas
group by c.oid, c.conname, pkdef.pkkey, ccu.table_name, tc.table_name, c.confupdtype, c.confdeltype
order by tc.table_name";
        }
        public override string SQLFunciones()
        {
            return @"select pg_get_functiondef (pg_proc.oid::regproc) as codigo 
                                from pg_proc inner join pg_namespace 
                                on pg_proc.pronamespace = pg_namespace.oid 
                                where pg_namespace.nspname in( " + Esquemas.ValorQuery() + ")";
        }
        public override string SQListaFunciones()
        {
            return @"select lib.fun_desc.codigo as codigo, pg_namespace.nspname || '.' || pg_proc.proname as nombre, pg_catalog.oidvectortypes(proargtypes) as args
                     from pg_proc inner join pg_namespace  on pg_proc.pronamespace = pg_namespace.oid,
                        lib.fun_desc 
                     where pg_namespace.nspname in( " + Esquemas.ValorQuery() + ") and pg_namespace.nspname || '.' || pg_proc.proname = lib.fun_desc.nombre " +
                    "order by nombre";
        }
        public override void SQLCreacionBase(Catalogo cat, Objeto cfg, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;
            if (cfg.ContainsKey("BD"))
            {
                Objeto bd = (Objeto)cfg["BD"];
                //  /var/lib/postgresql/9.4/main/pg_tblspc/
                //  creacion de base de datos
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = string.Concat("--create tablespace ts_", cat.Id, " owner us_", cat.Id, " location '", bd["Ubicacion"], cat.Id, "';")
                });
                // creacion de la base de datos
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = string.Concat("--create database db_", cat.Id, " owner us_", cat.Id, " tablespace ", "ts_", cat.Id, ";")
                });

                // reprocesa los esquemas
                Recipiente<Semilla> rs = new Recipiente<Semilla>();
                foreach (Registro reg in cat.Registros)
                {
                    string[] ids = reg.Tabla.ArrIDs();
                    if (!rs.Existe(ids[0]))
                    {
                        rs.Agregar(new Semilla() { Id = ids[0] });
                    }
                }
                //esquemas = new Arreglo(rs.Largo);
                foreach (Semilla s in rs)
                {
                    //esquemas.Add(s.Id);
                    instr.Agregar(new Dato()
                    {
                        Id = (++iId).ToString(),
                        Valor = string.Concat("drop schema if exists ", s.Id, " cascade ", ";")
                    });

                    instr.Agregar(new Dato()
                    {
                        Id = (++iId).ToString(),
                        Valor = string.Concat("create schema ", s.Id, " authorization us_", cat.Id, ";")
                    });
                }

            }
        }
        public override  Recipiente<Recipiente<Dato>> UPDTabDescSEQ(Catalogo cat)
        {
            //StringBuilder sb = new StringBuilder();
            //DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            //int iId = instr.Largo;

            TabDesc["tab_desc"].Estado |= Estado.Excluido;
            TabDesc["fun_desc"].Estado |= Estado.Excluido;

            ActualizarTabDescSeq(cat);
            return TabDesc;
        }

        public override void DialectoRelaciones(Recipiente<Dato> instr, Recipiente<Dato> tPadre, Recipiente<Dato> tHija, params string[] p)
        {
            int iId = instr.Largo;
            instr.Agregar(new Dato()
            {
                Id = (++iId).ToString(),
                Valor = string.Concat("alter table ", tHija["ESQUEMA"].Valor, p[2], " add constraint ", p[4], " foreign key ( ", p[3], " ) references ", 
                    tPadre["ESQUEMA"].Valor, p[0], " ( ", p[1], ") on update cascade on delete no action;")
            });
        }
        public override void ScriptSecuencias(Recipiente<Recipiente<Dato>> tabDesc, Catalogo catDest, Recipiente<Dato> instr)
        {
            foreach (Recipiente<Dato> rd in tabDesc)
            {
                int iId = instr.Largo;
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = string.Concat("create sequence ", rd["ESQUEMA"].Valor, "seq", rd.Id,
                    " start with ", rd["SEQ"].Valor, " increment by  1;")
                });

            }
            tabDesc["tab_desc"].Estado -= Estado.Excluido;

        }

        void ActualizarTabDescSeq(Catalogo cat)
        {
            //DescBD dbd = cat.BaseDeDatos.Descriptor (cat.Id);
            string sql = string.Empty;
            foreach (Recipiente<Dato> rd in TabDesc)
            {
                if (cat.Registros.Existe(rd["IDENTIFICADOR"].Valor))
                {
                    Registro reg = cat.Registros[rd["IDENTIFICADOR"].Valor];
                    Campo clave = reg.Campos.Claves(false)[0];
                    if (clave != null)
                    {
                        sql = string.Concat("select case when max(", clave.Nombre, ") is null then 1 else max(", clave.Nombre, ") + 1 end as Max From ", reg.Tabla);
                        using (ILectorBD qry = cat.BaseDeDatos.OperacionBD(sql))
                        {
                            if (qry.Read())
                                rd["SEQ"].Valor = qry.GetValue(0).ToString();
                        }
                    }
                }
            }
            
        }

        public override void ScriptComentarios(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            //DescBD desc = catOrigen.BaseDeDatos.Descriptor(catOrigen.Id);
            string sqlRel = string.Empty;
            int iId = instr.Largo;
            foreach (Registro reg in catDestino.Registros)
            {
                instr.Agregar(new Dato()
                {
                    Id = (++iId).ToString(),
                    Valor = catDestino.BaseDeDatos.Descriptor.SQLComentarioTabla(reg.Tabla, reg.Etiqueta)
                });

                sqlRel = catOrigen.BaseDeDatos.Descriptor.SQLCatalogoColumnas(catOrigen.Registros[reg.Id].Tabla);
                using (ILectorBD datos = catOrigen.BaseDeDatos.OperacionBD(sqlRel))
                {
                    while (datos.Read())
                    {
                        instr.Agregar(new Dato()
                        {
                            Id = (++iId).ToString(),
                            Valor = catDestino.BaseDeDatos.Descriptor.SQLComentarioColumna(reg.Tabla, datos.GetString(0), datos.GetString(5))
                        });
                    }
                }
            }
        }




    
        public override void ScriptVistasDominio(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;
            // setea el modo de recuperación permitiendo consultar por cualquier campo
            catOrigen.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;

            string[] dominio = new string[] { "Dominio", "Entidad", "DominioAtributo", "EntidadAtributo", "Atributo"};

            foreach (string tab in dominio)
            {
                if (!catOrigen.Registros.Existe(tab))
                {
                    return;
                }
            }

            Registro regDominio = catOrigen["Dominio"];
            //Registro regEntidad = catOrigen["Entidad"];
            Registro regEntidad = catDestino["Entidad"];
            //Registro regEntidadDestino = catDestino["Entidad"];
            Registro regDomAtr = catOrigen["DominioAtributo"];
            // instancia un consultor
            Consultor consultor = new Consultor();
            Recipiente<Recipiente<Dato>> extensiones = new Recipiente<Recipiente<Dato>>();

            // setea el consultado
            consultor.Consultado = catOrigen;
            consultor.Campos.Agregar(
                regDominio.Campos["dmId"],
                regDominio.Campos["dmNombre"],
                regDominio.Campos["dmAtrJson"]);

            regDominio.Campos["dmNombre"].Operador = "!=";
            regDominio.Campos["dmNombre"].Valor = "Disponible";
            consultor.Criterios = new Recipiente<Campo>();
            consultor.Criterios.Agregar(regDominio.Campos["dmNombre"]);
            consultor.Consultado = catOrigen;
            consultor.Recuperar();
            Catalogo regMM;
            string claseBD = catOrigen.BaseDeDatos.Clase;
            string driverBD = catOrigen.BaseDeDatos.Driver;

            string strAtrJson = string.Empty;
            List<object> atrArr = null;


            Objeto atrJson = Objeto.Ref;

            string entidadID = regEntidad.Id;
            foreach (Campos Campos in consultor)
            {
                regEntidad.Propiedades = new Propiedades();
                regEntidad.Propiedades.Add("dom", Campos["dmId"].Valor);
                regEntidad.Id = Campos["dmNombre"].Valor;

                strAtrJson = Campos["dmAtrJson"].Valor;

                if (!string.IsNullOrEmpty(strAtrJson))
                {

                    //atrJson = (Objeto)Util.Json(strAtrJson);
                    Compilador<Objeto>.Crear(ref atrJson, strAtrJson);

                    atrArr = (Arreglo)atrJson["Extiende"];

                    //obtener si el dominio tiene atributos
                    regDomAtr.Campos["daDominio"].Valor = Campos["dmId"].Valor;
                    regDomAtr.BaseDeDatos = (BaseDatos)Constructor.Embriones[claseBD].Germinar();
                    regDomAtr.BaseDeDatos.Driver = driverBD;
                    regDomAtr.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;
                    regDomAtr.Criterios = new Recipiente<Campo>();
                    regDomAtr.Criterios.Agregar(regDomAtr.Campos["daDominio"]);
                    if (regDomAtr.Recuperar())
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
                        catDestino.BaseDeDatos.Descriptor.ScriptVistaExtendidas(regEntidad, instr, true);
                    }

                }
                else
                {
                    catDestino.BaseDeDatos.Descriptor.ScriptVistaExtendidas(regEntidad, instr, true);
                }
            }
            regEntidad.Id = entidadID;
            //int numCase = 14;
            int numCase = 1;
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
                        sb2.Length--;//sqlite=true&
                        llamadas.Agregar(new Dato() { Id = string.Concat(rd.Id, i.ToString()), Valor = string.Concat(rd.Id, ".Atributos?", catDestino.BaseDeDatos.Clase, "=true&dmId=[", sb2.ToString(), "]&plataforma=", catOrigen.Id) });
                        sb2.Clear();
                    }
                }
                if (sb2.Length > 0)
                {
                    sb2.Length--;
                    llamadas.Agregar(new Dato() { Id = string.Concat(rd.Id), Valor = string.Concat(rd.Id, ".Atributos?", catDestino.BaseDeDatos.Clase, "=true&dmId=[", sb2.ToString(), "]&plataforma=", catOrigen.Id) });
                }
            }
            foreach (Dato d in llamadas)
            {
                string url = string.Concat( Util.Configuracion("URL"), d.Valor); //string.Concat(d.Id, ".Atributos?dmId=[", d.Valor, "]");
                System.Console.WriteLine(url);

                try
                {
                    regMM = Constructor.Crear<Catalogo>(url, Request);
                    foreach (Registro r in regMM)
                    {
                        catDestino.BaseDeDatos.Descriptor.ScriptVistaExtendidas(r, instr, true, true);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch (System.Exception e)
                {
                    //Console.WriteLine(e.Message);

                    goto InvocarURL;
                }
                InvocarURL:
                    bool b = true;
            }
            catOrigen.BaseDeDatos.CerrarConexion();
        }    
    
        public override void ScriptVistaExtendidas(Registro reg, Recipiente<Dato> instr, bool metacampos = false, bool fueraMmo = false)
        {
            int iId = instr.Largo;
            //instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = bdDestino.ComponerSQLInsertar() });
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Concat("create view ", reg.Tabla.Substring(0, 4), "v", reg.Id, " as ", "select "));
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

            if (reg.Tabla.StartsWith("mmo.entidad"))
            {
                sb.Length -= 2;
                sb.Append(string.Concat(" from ", reg.Tabla, " "));
                sb.Append(string.Concat("where (mmo.entidad.en_dominio=", reg.Propiedades["dom"], ");"));
                instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = sb.ToString() });
                return;
            }
            if (reg.Tabla.StartsWith("org.inst_elem"))
            {
                sb.Append(string.Concat(" rel.er_padre AS ", @"""Padre.Id"",", "rel.tipo AS ", @"""Padre.Tipo"""));
                sb.Append(string.Concat(" from ", "mmo.entidad, ", reg.Tabla,
                                        " left join ( select inst_elem_rel.er_padre, inst_elem_rel.er_hijo, ent.en_valor as tipo from org.inst_elem_rel, mmo.entidad ent where (org.inst_elem_rel.er_relacion = ent.en_id)) rel ON inst_elem.ie_id = rel.er_hijo"));
                sb.Append(string.Concat(" where (mmo.entidad.en_dominio", "=", reg.Propiedades["dom"], " and mmo.entidad.en_id = org.inst_elem.ie_tipo);"));
                instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = sb.ToString() });
                return;
            }

            // casos simples como cac.PERSONA, enc.PLANIFICACION
            sb.Length -= 2;
            sb.Append(string.Concat(" from ", reg.Tabla, ";"));
            instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = sb.ToString() });
        }
        public override void DialectoFunciones(Recipiente<Dato> instr, params string[] p)
        {
            int iId = instr.Largo;
            instr.Agregar(new Dato()
            {
                Id = (++iId).ToString(),
                Valor = string.Concat(p)
            });
        }
        public override void ScriptFunciones(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;

            Recipiente<Semilla> rd = new Recipiente<Semilla>();
            foreach (Registro reg in catOrigen.Registros)
            {
                if (!rd.Existe(reg.Tabla.ArrIDs()[0]))
                {
                    rd.Agregar(new Semilla() { Id = reg.Tabla.ArrIDs()[0] });
                }
            }

            StringBuilder sbEsquemas = new StringBuilder();
            foreach (Semilla s in rd)
            {
                sbEsquemas.Append(s.Id.ValorQuery() + ",");
            }
            sbEsquemas.Length--;

            //DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            using (ILectorBD lector = catOrigen.BaseDeDatos.OperacionBD(SQLFunciones()))
            {
                while (lector.Read())
                {
                    catDestino.BaseDeDatos.Descriptor.DialectoFunciones(instr, lector.GetString(0), ";");
                    /*instr.Agregar(new Dato() { Id = (++iId).ToString(), 
                        Valor = string.Concat(lector.GetString(0), ";") });*/
                }
            }
        }

        public override void CrearBD(Recipiente<Dato> inst)
        {
            throw new System.NotImplementedException();
        }
    }
}
