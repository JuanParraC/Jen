#if BaseDeDatoSQLServer
namespace Jen.RDBMS.Descriptores
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using Jen;
    using Jen.Extensiones;
    using Jen.Json;

    public class SQLServer : DescBD
    {

        public SQLServer()
        {
            Id = Clase = Lenguaje.SQLServer;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected SQLServer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = Clase = Lenguaje.PostgreSQL;
        }
        public SQLServer(Objeto cfg) : base(cfg)
        {
            Id = Clase = Lenguaje.SQLServer;
        }
        public SQLServer(string id, string driver) : base(id, driver)
        {
            Id = Clase = Lenguaje.SQLServer;
        }
        public override BaseDatos BaseDatos
        {
            get
            {
                return new Jen.RDBMS.SQLServer() { Id = this.Id, Driver = this.Driver };
            }
        }

        public override string SQLSelectTabDesc
        {
            get
            {
                return @"SELECT 
                        [TABLA],
                        ESQUEMA,
                        isnull([TABLA_PADRE],'') AS TABLA_PADRE,
                        isnull([VALIDACIONSQL],'') AS VALIDACIONSQL ,
                        isnull([IDENTIFICADOR],'') AS IDENTIFICADOR,
                        isnull([SINGULAR],'') AS SINGULAR,
                        isnull([PLURAL],'') AS PLURAL,
                        isnull([DENOMINACION],'') AS DENOMINACION,
                        isnull([CONTROLADOR],'') AS CONTROLADOR,
                        isnull([DESCRIPCION],'') AS DESCRIPCION,
                        [MENU],
                        isnull([LOV],'') AS LOV,
                        isnull([PATRONESJS],'') AS PATRONESJS,
                        isnull([CONFIGURACION_CONTROLADOR],'') AS CONFIGURACION_CONTROLADOR,
                        [SEQ]
                        FROM lib.TAB_DESC ;";
            }
        }
        public override string SQLCreacionSecuencia
        {
            get
            {
                return "select 'CREATE SEQUENCE ' + ESQUEMA + '.SEQ_' + TABLA + '   START WITH ' + STR(SEQ) + ' INCREMENT BY  1;' as  dmNombre from dbo.TAB_DESC";
            }
        }
        public override string SQLCatalogoTablas()
        {

            /*return @"select t.name, convert(varchar(max),ex.value)
            from  sys.tables t, sys.extended_properties ex  
                where ex.major_id = t.object_id 
                AND ex.minor_id = 0  
                AND ex.name = 'MS_Description'
                and SCHEMA_NAME(t.schema_id) in (" + Esquemas.ValorQuery() + ")";*/

            return @"select A.Name, '' 
                        from sys.objects A inner join sys.partitions B ON A.object_id = B.object_id 
                        where A.type = 'U' and a.name not like '%PASO%' and SCHEMA_NAME(a.schema_id) in (" + Esquemas.ValorQuery() + @")
                        group by A.schema_id, A.Name 
                        having sum(B.rows) >0";


        }
        public override string SQLCatalogoTablas(Arreglo tablas)
        {
            StringBuilder sb = new StringBuilder(tablas.Count * 10);
            sb.Append(SQLCatalogoTablas() + " and (t.table_name) in(");
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
            return @"SELECT OBJECT_NAME(scfk.id) + '.' + scfk.name as CampoTraducido
FROM SYSFOREIGNKEYS sfk INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS isrc  ON OBJECT_NAME(constid) = isrc.CONSTRAINT_NAME
INNER JOIN SYSCOLUMNS scfk ON sfk.fkeyid = scfk.id AND sfk.fkey = scfk.colid   
INNER JOIN SYSCOLUMNS scpk ON sfk.rkeyid = scpk.id AND sfk.rkey = scpk.colid
where isrc.CONSTRAINT_SCHEMA = " + Esquemas.ValorQuery();
        }
        public override string SQLCatalogoColumna(string tabla, string columna)
        {
            return SQLCatalogoColumnas(tabla, false) + " and lower(cols.column_name) = lower('" + columna + "')";
        }
        public override string SQLCatalogoColumnas(string tabla, bool orden = true)
        {
            string[] ids = tabla.ArrIDs();

            string ord = orden ? " order by 1 " : string.Empty;

            string _sqlCatalogoColumnas =
                @"SELECT  
                            Nombre = c.name, 
                            tipo = CASE t.name 
                                WHEN 'int' THEN 'NUMBER' 
                                WHEN 'smallint' THEN 'NUMBER'
                                WHEN 'float' THEN 'NUMBER' 
                                WHEN 'tinyint' THEN 'NUMBER' 
                                WHEN 'bigint' THEN 'NUMBER' 
                                WHEN 'numeric' THEN 'NUMBER' 
                                WHEN 'char' THEN 'VARCHAR2' 
                                WHEN 'nchar' THEN 'VARCHAR2' 
                                WHEN 'varchar' THEN 'VARCHAR2' 
                                WHEN 'nvarchar' THEN 'VARCHAR2' 
                                WHEN 'text' THEN 'VARCHAR2' 
                                WHEN 'datetime' THEN 'DATE' 
                            END,
                            Largo = c.Max_Length,
                            nulable = case c.is_nullable WHEN 1 THEN 'T' ELSE 'F' END,
                            UNICO = ISNULL((select 'T'
                             from sys.tables t
                            inner join sys.schemas s on t.schema_id = s.schema_id
                            inner join sys.indexes i on i.object_id = t.object_id
                            inner join sys.index_columns ic on ic.object_id = t.object_id
                                    and c.object_id = t.object_id and
                                            ic.column_id = c.column_id and 
                                            i.object_id = ic.object_id and
                                            i.index_id = ic.index_id and
                                            ic.is_included_column=0

                            where i.index_id > 0    
                            and i.type in (1, 2) -- solo clustered y nonclustered
                            and i.is_primary_key = 0 -- excluye claves primarias
                            and i.is_unique_constraint = 1 -- solo claves unicas
                            and i.is_disabled = 0
                            and i.is_hypothetical = 0
                            and ic.key_ordinal > 0),'F'),
                            descripcion = CAST( 
                                            ISNULL(ex.value, 
                                            c.name + '|' + 'Falta ayuda para ' + c.name)  
                                            AS VARCHAR(2000)),
                            autonumerico = case c.is_identity
                                WHEN 1 THEN 'T'
                                ELSE 'F' 
                               END,
                            PREC = c.precision,
                            SCALE = c.scale,
                            object_definition(c.default_object_id) AS valorDefecto,
                            t.name as tipoBD
                        FROM  
                            sys.columns c  JOIN  sys.types AS t ON c.user_type_id=t.user_type_id
                        LEFT OUTER JOIN  
                            sys.extended_properties ex  
                        ON  
                            ex.major_id = c.object_id 
                            AND ex.minor_id = c.column_id  
                            AND ex.name = 'MS_Description'  
                        WHERE  
                            OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0  
                            AND c.object_id in (SELECT object_id
                                        FROM sys.tables
                                        where SCHEMA_NAME(schema_id) in (" + Esquemas.ValorQuery() +  ")  and OBJECT_NAME(object_id) = '" + ids[1] + "') " + ord;

            return _sqlCatalogoColumnas;

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
            return "SP_PKEYS " + ids[1] + ";";

        }
        public override string SQLRelaciones(string tabla)
        {
            string[] ids = tabla.ArrIDs();
            string _sqlRelaciones = @"SELECT constid
                ,OBJECT_NAME(rkeyid) TablaPadre   
                ,scpk.name ColumnaPadre
                ,OBJECT_NAME(fkeyid) TablaHija   
                ,scfk.name ColumnaHija
                ,OBJECT_NAME(constid) NombreRelacion
                ,isrc.UPDATE_RULE
                ,isrc.DELETE_RULE
                FROM SYSFOREIGNKEYS sfk INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS isrc  ON OBJECT_NAME(constid) = isrc.CONSTRAINT_NAME
                INNER JOIN SYSCOLUMNS scfk ON sfk.fkeyid = scfk.id AND sfk.fkey = scfk.colid   
                INNER JOIN SYSCOLUMNS scpk ON sfk.rkeyid = scpk.id AND sfk.rkey = scpk.colid   
                WHERE ( scpk.id in (SELECT object_id
            FROM sys.tables
            where SCHEMA_NAME(schema_id) + '.' + OBJECT_NAME(object_id) = '%TABLA%')) 
            ORDER BY constid";

            return _sqlRelaciones.Replace("%TABLA%", tabla);
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
            return "";
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
#endif