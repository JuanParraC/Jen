
namespace Jen.Extensiones
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using Jen;
    using Jen.Extensiones;
    using Jen.Json;
    using Jen.RDBMS.Descriptores;
    using Ev = System.Environment;

    public static partial class Extensiones
    {

        public static void ScriptEsquemas(this Catalogo cat, ref StringBuilder sb)
        {
            string rol = "rol_ubt";
            if (cat.Propiedades.ContainsKey("rol"))
            {
                rol = cat.Propiedades["rol"];
            }

            Recipiente<Semilla> rs = new Recipiente<Semilla>();
            foreach (Registro reg in cat.Registros)
                if (!rs.Existe(reg.Tabla.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]))
                    rs.Agregar(new Semilla() { Id = reg.Tabla.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0] });

            foreach (Semilla s in rs)
            {
                sb.Append(string.Concat(" -- eliminacion de esquema", Ev.NewLine,
                  Ev.NewLine, "drop schema if exists ", s.Id, " cascade ", ";", Ev.NewLine, Ev.NewLine).ToLower());

                sb.Append(string.Concat(" -- Creacion de esquema", Ev.NewLine,
                Ev.NewLine, "create schema ", s.Id, " authorization ", rol, ";", Ev.NewLine, Ev.NewLine).ToLower());
            }
        }

        public static void ScriptBaseDeDatos(this Catalogo cat, ref StringBuilder sb)
        {
            string rol = "rol_ubt";
            if (cat.Propiedades.ContainsKey("rol"))
            {
                rol = cat.Propiedades["rol"];
            }

            sb.Append(string.Concat(" -- Creacion del tablespace", Ev.NewLine,
            Ev.NewLine, "--create tablespace ts_", cat.Id, " owner ", rol, " location '/var/lib/postgresql/9.4/main/pg_tblspc/", cat.Id, "';", Ev.NewLine, Ev.NewLine).ToLower());

            sb.Append(string.Concat(" -- Creacion de la base de datos", Ev.NewLine,
            Ev.NewLine, "--create database db_", cat.Id, " owner ", rol, " tablespace ", "ts_", cat.Id, ";", Ev.NewLine, Ev.NewLine).ToLower());
        }

        public static void ScriptBD(this Jen.Catalogo cat, ref StringBuilder sb)
        {


            foreach (Registro reg in cat.Registros)
            {
                reg.ScriptBD(ref sb);
            }

            cat.ScriptSecuencias(ref sb);
            cat.ScriptComentarios(ref sb);
            cat.ScriptInserciones(ref sb);
            cat.ScriptRelaciones(ref sb);
            cat.ScriptIndices(ref sb);
            cat.ScriptVistasDominio(ref sb);
            cat.ScriptVistasPersonalizadas(ref sb);

            cat.ScriptFunciones(ref sb);
            cat.ScriptTriggers(ref sb);

            cat.ScriptFinal(ref sb);


        }
        public static void CambiarPropietario(this Jen.Catalogo cat, ref StringBuilder sb)
        {
            foreach (Registro reg in cat.Registros)
            {
                sb.Append(string.Concat("ALTER TABLE ", reg.Tabla, " OWNER TO ", cat.Propiedades["rol"], ";", Ev.NewLine));
            }
            //StringBuilder sb = new StringBuilder();
            DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            dbd.TabDesc["tab_desc"].Estado |= Estado.Excluido;
            dbd.TabDesc["fun_desc"].Estado |= Estado.Excluido;

            //cat.ActualizarTabDescSeq(dbd.TabDesc);
            foreach (Recipiente<Dato> rd in dbd.TabDesc)
            {


                sb.Append(string.Concat("ALTER SEQUENCE ", rd["ESQUEMA"].Valor, "seq_", rd.Id,
                                        " OWNER TO ", cat.Propiedades["rol"], " ;", Ev.NewLine).ToLower());

            }
            dbd.TabDesc["tab_desc"].Estado -= Estado.Excluido;


            cat.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;
            // apunta a cada tabla a utilizar
            Registro Dominio = cat["Dominio"];
            Registro Entidad = cat["Entidad"];
            Registro DominioAtrbuto = cat["DominioAtributo"];
            // instancia un consultor
            Consultor consultor = new Consultor();
            Recipiente<Recipiente<Dato>> extensiones = new Recipiente<Recipiente<Dato>>();

            // setea el consultado
            consultor.Consultado = cat;
            consultor.Campos.Agregar(
                Dominio.Campos["dmId"],
                Dominio.Campos["dmNombre"],
                Dominio.Campos["dmAtrJson"]);

            Dominio.Campos["dmNombre"].Operador = "!=";
            Dominio.Campos["dmNombre"].Valor = "Disponible";
            consultor.Criterios = new Recipiente<Campo>();
            consultor.Criterios.Agregar(Dominio.Campos["dmNombre"]);
            consultor.Consultado = cat;
            string strAtrJson = string.Empty;
            string claseBD = cat.BaseDeDatos.Clase;
            string driverBD = cat.BaseDeDatos.Driver;
            List<object> atrArr = null;
            Objeto atrJson = Objeto.Ref;
            if (consultor.Recuperar())
            {
                foreach (Campos Campos in consultor)
                {
                    strAtrJson = Campos["dmAtrJson"].Valor;

                    if (!string.IsNullOrEmpty(strAtrJson))
                    {
                        Compilador<Objeto>.Crear(ref atrJson, strAtrJson);
                        if (atrJson.ContainsKey("Extiende"))
                        {
                            atrArr = (Arreglo)atrJson["Extiende"];

                            foreach (object o in atrArr)
                            {

                                Registro reg = cat.Registros[o.ToString()];

                                sb.Append(string.Concat("alter table ", reg.Tabla.Substring(0, 4), "v", Campos["dmNombre"].Valor, " OWNER TO ", cat.Propiedades["rol"], ";", Ev.NewLine).ToLower());

                            }
                        }
                        else
                        {
                            sb.Append(string.Concat("alter table ", Entidad.Tabla.Substring(0, 4), "v", Campos["dmNombre"].Valor, " OWNER TO ", cat.Propiedades["rol"], ";", Ev.NewLine).ToLower());
                        }
                    }
                    else
                    {
                        sb.Append(string.Concat("alter table ", Entidad.Tabla.Substring(0, 4), "v", Campos["dmNombre"].Valor, " OWNER TO ", cat.Propiedades["rol"], ";", Ev.NewLine).ToLower());
                    }
                }
            }


            //sb.Append(string.Concat("alter table ", Entidad.Tabla.Substring(0, 4), "v", Campos["dmNombre"].Valor, " OWNER TO ", cat.Propiedades["rol"], ";", Ev.NewLine).ToLower());

            using (ILectorBD lector = dbd.BaseDatos.OperacionBD(dbd.SQLFunciones()))
            {
                while (lector.Read())
                {
                    sb.Append(string.Concat("alter function ", lector.GetString(1), " owner to ", cat.Propiedades["rol"], ";", Ev.NewLine));
                }
            }
        }
        static void ScriptFinal(this Catalogo cat, ref StringBuilder sb)
        {

            /*foreach (Registro reg in cat.Registros)
            {
                
            }*/


            sb.Append(string.Concat("-- vacuum full analyze ", Ev.NewLine, ";", Ev.NewLine));
            //}

        }
        static void ScriptTriggers(this Catalogo cat, ref StringBuilder sb)
        {

            //insert into mmo.entidad ( en_codigo,en_dependiente,en_dominio,en_id,en_padre,en_valor ) 
            //values('',null, 0,10917, 10019'"Formato2":"Formato2" );




            Patron trgUpd;// = Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Trigger/triggerUpd.xml");
            Patron trgIns;// = Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Trigger/triggerIns.xml");
            Patron trgDel;// = Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Trigger/triggerDel.xml");

            Recipiente<Patron> rp = new Recipiente<Patron>();
            rp.Agregar(Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Campos/ccTexto.xml"),
                       Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Campos/ccFecha.xml"),
                       Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Campos/ccNumerico.xml"),
                       Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Campos/cnTexto.xml"),
                       Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Campos/cnFecha.xml"),
                       Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Campos/cnNumerico.xml")
            );

            Patron compruebaCambios;
            Patron compruebaNulos;

            cat.Registros["Transaccion"].Estado |= Estado.Excluido;
            cat.Registros["TransaccionNodo"].Estado |= Estado.Excluido;
            cat.Registros["FunDesc"].Estado |= Estado.Excluido;
            cat.Registros["TabDesc"].Estado |= Estado.Excluido;
            string rol = "rol_ubt";
            if (cat.Propiedades.ContainsKey("rol"))
            {
                rol = cat.Propiedades["rol"];
            }
            DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            dbd.TabDesc["tab_desc"].Estado |= Estado.Excluido;
            dbd.TabDesc["fun_desc"].Estado |= Estado.Excluido;
            foreach (Registro reg in cat.Registros)
            {

                Objeto trg = Objeto.Ref;
                string tabla = reg.Tabla.ArrIDs()[1];
                Compilador<Objeto>.Crear(ref trg, dbd.TabDesc[tabla]["CONFIGURACION_CONTROLADOR"].Valor);
                /*
                trgUpd = Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Trigger/triggerUpd.xml");
                trgIns = Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Trigger/triggerIns.xml");
                trgDel = Constructor.Crear<Patron>("Plantillas/Generacion/Sql/Trigger/triggerDel.xml");
*/
                trgUpd = Constructor.Crear<Patron>(trg["trgUpd"].ToString());
                trgIns = Constructor.Crear<Patron>(trg["trgIns"].ToString());
                trgDel = Constructor.Crear<Patron>(trg["trgDel"].ToString());

                string esquema = reg.Tabla.ArrIDs()[0];
                //string tabla = reg.Tabla.ArrIDs()[1];
                string funcion = string.Concat(tabla, "_upd");
                string clave = reg.Campos.Claves(false)[0].Nombre;

                trgUpd.Variables["esquema"].Asignar(esquema);
                trgUpd.Variables["tabla"].Asignar(tabla);
                trgUpd.Variables["funcion"].Asignar(funcion);
                if (trgUpd.Variables.Existe("clave"))
                {
                    trgUpd.Variables["clave"].Asignar(clave);
                }
                trgUpd.Variables["rol"].Asignar(rol);

                funcion = string.Concat(tabla, "_del");
                trgDel.Variables["esquema"].Asignar(esquema);
                trgDel.Variables["tabla"].Asignar(tabla);
                trgDel.Variables["funcion"].Asignar(funcion);
                //trgDel.Variables["clave"].Asignar(clave);
                if (trgDel.Variables.Existe("clave"))
                {
                    trgDel.Variables["clave"].Asignar(clave);
                }
                trgDel.Variables["rol"].Asignar(rol);

                funcion = string.Concat(tabla, "_ins");
                trgIns.Variables["esquema"].Asignar(esquema);
                trgIns.Variables["tabla"].Asignar(tabla);
                trgIns.Variables["funcion"].Asignar(funcion);
                //trgIns.Variables["clave"].Asignar(clave);

                if (trgIns.Variables.Existe("clave"))
                {
                    trgIns.Variables["clave"].Asignar(clave);
                }
                trgIns.Variables["rol"].Asignar(rol);

                StringBuilder sbCm = new StringBuilder();

                foreach (Campo cm in reg.Campos)
                {
                    compruebaNulos = rp["cn" + cm.Tipo.ToString()];
                    compruebaCambios = rp["cc" + cm.Tipo.ToString()];
                    compruebaCambios.Variables["campo"].Asignar(cm.Nombre);
                    compruebaNulos.Variables["campo"].Asignar(cm.Nombre);
                    trgUpd.Variables["compruebaCambiosCampos"].Asignar(compruebaCambios.Pintar());
                    trgIns.Variables["compruebaCamposNulos"].Asignar(compruebaNulos.Pintar());
                    sbCm.Append(cm.Nombre);
                    sbCm.Append(", ");

                }
                sbCm.Length = sbCm.Length - 2;
                trgIns.Variables["campos"].Asignar(sbCm.ToString());
                sb.Append(trgUpd.Pintar());
                sb.Append(trgIns.Pintar());
                sb.Append(trgDel.Pintar());
            }
        }

        static void ScriptFunciones(this Catalogo cat, ref StringBuilder sb)
        {
            string rol = "rol_rol";
            if (cat.Propiedades.ContainsKey("rol"))
            {
                rol = cat.Propiedades["rol"];
            }
            DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            using (ILectorBD lector = dbd.BaseDatos.OperacionBD(dbd.SQLFunciones()))
            {
                while (lector.Read())
                {
                    sb.Append(string.Concat(lector.GetString(0), Ev.NewLine, ";", Ev.NewLine,
                                              "alter function ", lector.GetString(1), " owner to ", rol, ";", Ev.NewLine));
                }
            }
        }

        public static void Serializar(this Jen.Catalogo cat, string archivoDestino)
        {
            string xml = Jen.Constructor.Serializar(cat);

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElement;
            // excluye el encabezado
            string xmlHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";

            // excluye la primera linea
            string xDoc = xml.Substring(xml.IndexOf(xmlHeader) + xmlHeader.Length);
            //&quot;

            // carga el documento
            xmlDoc.LoadXml(xDoc);
            //// selecciona todos los atributos que esten en blanco o con valor en false

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("*");
            //System.Collections.Generic.List<XmlNode> nodes2Delete = new System.Collections.Generic.List<XmlNode>();

            xmlNodeList = xmlDoc.GetElementsByTagName("Relaciones");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                xmlElement = (XmlElement)xmlNode;
                XmlNodeList xmlNodeListCampos = xmlElement.GetElementsByTagName("Campo");
                foreach (XmlNode xmlNodeCampo in xmlNodeListCampos)
                {
                    XmlElement xmlElementCampos = (XmlElement)xmlNodeCampo;
                    xmlElementCampos.RemoveAttribute("Ayuda");
                    xmlElementCampos.RemoveAttribute("Autonumerico");
                    xmlElementCampos.RemoveAttribute("Clave");
                    xmlElementCampos.RemoveAttribute("Etiqueta");
                    xmlElementCampos.RemoveAttribute("Formato");
                    xmlElementCampos.RemoveAttribute("Largo");
                    xmlElementCampos.RemoveAttribute("Operador");
                    xmlElementCampos.RemoveAttribute("Traductor");
                }
            }

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = false;
            xmlWriterSettings.OmitXmlDeclaration = false;
            //xmlWriterSettings.Encoding.ToString();
            XmlWriter xmlWriter = XmlWriter.Create(archivoDestino, xmlWriterSettings);
            xmlDoc.Save(xmlWriter);

        }

        public static void ScriptComentarios(this Catalogo catOrigen, ref StringBuilder sb)
        {
            DescBD desc = catOrigen.BaseDeDatos.Descriptor(catOrigen.Id);
            string sqlRel = string.Empty;
            foreach (Registro reg in catOrigen.Registros)
            {
                sb.Append(desc.SQLComentarioTabla(reg.Tabla, reg.Etiqueta));
                sqlRel = desc.SQLCatalogoColumnas(reg.Tabla);
                using (ILectorBD datos = desc.BaseDatos.OperacionBD(sqlRel))
                {
                    while (datos.Read())
                    {
                        sb.Append(desc.SQLComentarioColumna(reg.Tabla, datos.GetString(0), datos.GetString(5)));
                    }
                }
            }
        }
        /*

CREATE OR REPLACE VIEW trx.nodos AS 
 SELECT usuario_login.ul_estado,
    usuario_login.ul_fecha_inicio,
    usuario_login.ul_fecha_termino,
    usuario_login.ul_id,
    usuario_login.ul_tipo,
    usuario_login.ul_usuario,
    usuario_login.ul_valor
   FROM cac.usuario_login
  WHERE usuario_login.ul_tipo = 861::numeric;

ALTER TABLE trx.nodos
  OWNER TO us_vw;

        */

        static void ScriptVistasPersonalizadas(this Catalogo cat, ref StringBuilder sb)
        {
            sb.Append(
@"CREATE OR REPLACE VIEW trx.nodos AS 
 SELECT usuario_login.ul_estado,
    usuario_login.ul_fecha_inicio,
    usuario_login.ul_fecha_termino,
    usuario_login.ul_id,
    usuario_login.ul_tipo,
    usuario_login.ul_usuario,
    usuario_login.ul_valor
   FROM cac.usuario_login
  WHERE usuario_login.ul_tipo = 861::numeric;

ALTER TABLE trx.nodos
  OWNER TO rol_" + cat.Id + ";" + Ev.NewLine);
        }
        static void ScriptVistasDominio(this Catalogo cat, ref StringBuilder sb)
        {
            // setea el modo de recuperación permitiendo consultar por cualquier campo
            cat.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;
            // apunta a cada tabla a utilizar
            Registro Dominio = cat["Dominio"];
            Registro Entidad = cat["Entidad"];
            Registro DominioAtrbuto = cat["DominioAtributo"];
            // instancia un consultor
            Consultor consultor = new Consultor();
            Recipiente<Recipiente<Dato>> extensiones = new Recipiente<Recipiente<Dato>>();

            // setea el consultado
            consultor.Consultado = cat;
            consultor.Campos.Agregar(
                Dominio.Campos["dmId"],
                Dominio.Campos["dmNombre"],
                Dominio.Campos["dmAtrJson"]);

            Dominio.Campos["dmNombre"].Operador = "!=";
            Dominio.Campos["dmNombre"].Valor = "Disponible";
            consultor.Criterios = new Recipiente<Campo>();
            consultor.Criterios.Agregar(Dominio.Campos["dmNombre"]);
            consultor.Consultado = cat;
            consultor.Recuperar();
            Catalogo regMM;
            string claseBD = cat.BaseDeDatos.Clase;
            string driverBD = cat.BaseDeDatos.Driver;

            string strAtrJson = string.Empty;
            List<object> atrArr = null;


            //StringBuilder updAtrJson = new StringBuilder();


            Objeto atrJson = Objeto.Ref;

            //updAtrJson.Append(Ev.NewLine);
            /*Registro regDom = Constructor.DesXml<Registro>(Constructor.Serializar(Dominio));
            regDom.BaseDeDatos = Dominio.BaseDeDatos;
            regDom.BaseDeDatos.Recuperacion = Recuperacion.PorClave;
            for (int icampo = 0; icampo < regDom.Campos.Largo; icampo++)
            {
                if (!consultor.Campos.Existe(regDom.Campos[icampo].Id))
                {
                    regDom.Campos[icampo].Estado |= Estado.Excluido;
                }
            }*/

            foreach (Campos Campos in consultor)
            {
                Entidad.Propiedades = new Propiedades();
                Entidad.Propiedades.Add("dom", Campos["dmId"].Valor);
                Entidad.Id = Campos["dmNombre"].Valor;

                strAtrJson = Campos["dmAtrJson"].Valor;

                if (!string.IsNullOrEmpty(strAtrJson))
                {

                    //atrJson = (Objeto)Util.Json(strAtrJson);
                    Compilador<Objeto>.Crear(ref atrJson, strAtrJson);

                    atrArr = (Arreglo)atrJson["Extiende"];

                    //obtener si el dominio tiene atributos
                    DominioAtrbuto.Campos["daDominio"].Valor = Campos["dmId"].Valor;
                    DominioAtrbuto.BaseDeDatos = (BaseDatos)Constructor.Embriones[claseBD].Germinar();
                    DominioAtrbuto.BaseDeDatos.Driver = driverBD;
                    DominioAtrbuto.BaseDeDatos.Recuperacion = Recuperacion.PorConsulta;
                    DominioAtrbuto.Criterios = new Recipiente<Campo>();
                    DominioAtrbuto.Criterios.Agregar(DominioAtrbuto.Campos["daDominio"]);
                    if (DominioAtrbuto.Recuperar())
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

                            /*InvocarURL:
                                try
                                {
                                    regMM = Constructor.Crear<Catalogo>(o.ToString());
                                    regMM.Registros[0].ScriptVistaExtendidas(ref sb, true);
                                    //Entidad.ScriptVistaDominio(ref sb, true);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    goto InvocarURL;
                                }*/
                        }
                    }
                    else
                    {
                        Entidad.ScriptVistaExtendidas(ref sb, true);
                    }

                }
                else
                {
                    Entidad.ScriptVistaExtendidas(ref sb, true);
                }
            }
            // inserta la actualizacion del dominio
            //sb.Append(updAtrJson.ToString());

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
                        sb2.Length--;
                        llamadas.Agregar(new Dato() { Id = string.Concat(rd.Id, i.ToString()), Valor = string.Concat(rd.Id, ".Atributos?dmId=[", sb2.ToString(), "]&plataforma=", cat.Id) });
                        sb2.Clear();
                    }
                }
                if (sb2.Length > 0)
                {
                    sb2.Length--;
                    llamadas.Agregar(new Dato() { Id = string.Concat(rd.Id), Valor = string.Concat(rd.Id, ".Atributos?dmId=[", sb2.ToString(), "]&plataforma=", cat.Id) });
                    //lamadas.Agregar(new Dato() { Id = rd.Id, Valor = sb2.ToString() });
                }
            }
            foreach (Dato d in llamadas)
            {
                string url = d.Valor; //string.Concat(d.Id, ".Atributos?dmId=[", d.Valor, "]");
                Console.WriteLine(url);

                try
                {
                    regMM = Constructor.Crear<Catalogo>(url);
                    foreach (Registro r in regMM)
                    {
                        StringBuilder sb2 = new StringBuilder();
                        r.ScriptVistaExtendidas(ref sb2, true);
                        sb.Append(sb2.ToString());
                    }
                    System.Threading.Thread.Sleep(1000);
                    //Entidad.ScriptVistaDominio(ref sb, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    goto InvocarURL;
                }
            InvocarURL:
                string s = "";
            }
            cat.BaseDeDatos.CerrarConexion();
        }

        static void ScriptIndices(this Catalogo cat, ref StringBuilder sb)
        {
            foreach (Registro reg in cat.Registros)
            {
                if (reg.Relaciones != null)
                {
                    foreach (Relacion rel in reg.Relaciones)
                    {
                        Registro regHijo = cat.Registros[rel.Hijo.Id];
                        sb.Append(string.Concat("create index  ind_", rel.Id, //regHijo.Campos[rel.Hijo[0].Id].Id,
                        " on ", regHijo.Tabla, "(", regHijo.Campos[rel.Hijo[0].Id].Nombre, ");", Ev.NewLine));
                    }
                }
            }

        }

        static void ScriptRelaciones(this Catalogo cat, ref StringBuilder sb)
        {
            //ILectorBD datos;
            string sqlRel = string.Empty;
            DescBD desc = cat.BaseDeDatos.Descriptor(cat.Id);
            foreach (Registro padres in cat.Registros)
            {
                sqlRel = desc.SQLRelaciones(padres.Tabla);
                using (ILectorBD datos = desc.BaseDatos.OperacionBD(sqlRel))
                {
                    while (datos.Read())
                    {
                        Recipiente<Dato> tbHija = desc.TabDesc[datos.GetValue(3).ToString()];
                        Recipiente<Dato> tbPadre = desc.TabDesc[datos.GetValue(1).ToString()];

                        sb.Append(string.Concat("alter table ",
                            tbHija["ESQUEMA"].Valor, datos.GetValue(3).ToString(),
                            " add constraint ", datos.GetValue(5).ToString(),
                          " foreign key ( ", datos.GetValue(4).ToString(), " ) references ",
                            tbPadre["ESQUEMA"].Valor, datos.GetValue(1).ToString(),
                          //" ( ", datos.GetValue (2).ToString (), " );", Ev.NewLine));
                          " ( ", datos.GetValue(2).ToString(), ") on update cascade ON delete no action;", Ev.NewLine).ToLower());
                    }
                }
            }
        }

        static void ScriptSecuencias(this Catalogo cat, ref StringBuilder seq)
        {
            StringBuilder sb = new StringBuilder();
            DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            dbd.TabDesc["tab_desc"].Estado |= Estado.Excluido;
            dbd.TabDesc["fun_desc"].Estado |= Estado.Excluido;

            cat.ActualizarTabDescSeq(dbd.TabDesc);
            foreach (Recipiente<Dato> rd in dbd.TabDesc)
            {

                double valSeq = double.Parse(rd["SEQ"].Valor);
                if (valSeq < 1)
                {
                    valSeq = 1;
                }

                sb.Append(string.Concat("create sequence ", rd["ESQUEMA"].Valor, "seq_", rd.Id,
                                        " start with ", valSeq.ToString(), " INCREMENT BY  1;", Ev.NewLine).ToLower());

            }
            dbd.TabDesc["tab_desc"].Estado -= Estado.Excluido;
            seq.Append(sb.ToString());
        }

        static void ActualizarTabDescSeq(this Catalogo cat, Recipiente<Recipiente<Dato>> tabDesc)
        {
            DescBD dbd = cat.BaseDeDatos.Descriptor(cat.Id);
            string sql = string.Empty;
            foreach (Recipiente<Dato> rd in tabDesc)
            {
                if (cat.Registros.Existe(rd["IDENTIFICADOR"].Valor))
                {
                    Registro reg = cat.Registros[rd["IDENTIFICADOR"].Valor];
                    Campo clave = ObtenerClave(reg.Campos);
                    if (clave != null)
                    {
                        sql = string.Concat("select case when max(", clave.Nombre, ") is null then 1 else max(", clave.Nombre, ") + 1 end as Max From ", reg.Tabla);
                        using (ILectorBD qry = dbd.BaseDatos.OperacionBD(sql))
                        {
                            if (qry.Read())
                            {
                                rd["SEQ"].Valor = qry.GetValue(0).ToString();
                            }
                        }
                    }
                }
            }

        }

        static Campo ObtenerClave(Campos cms)
        {
            foreach (Campo c in cms)
            {
                if (c.Clave)
                    return c;
            }
            return null;
        }

        static void ScriptInserciones(this Catalogo cat, ref StringBuilder sb)
        {
            foreach (Registro reg in cat.Registros)
            {
                Consultor cons = new Consultor();
                cons.Consultado = cat;

                Campo clave = ObtenerClave(reg.Campos);
                if (clave != null)
                {
                    cons.Ordenadores = new Recipiente<Campo>();
                    cons.Ordenadores.Agregar(clave);
                }
                cons.Campos.Agregar(reg.Campos);

                if (cons.Recuperar())
                {
                    foreach (Campos cs in cons)
                    {
                        foreach (Campo c in cs)
                        {
                            string val = string.Empty;
                            if (c.Propiedades != null)
                            {
                                if (c.Propiedades.TryGetValue("SubTipo", out val))
                                {
                                    if (val.Contains("JSON") && string.IsNullOrEmpty(c.Valor))
                                    {
                                        c.Valor = "null";
                                    }
                                }
                            }
                        }
                        sb.Append(string.Concat(cat.BaseDeDatos.ComponerSQLInsertar(), ";", Ev.NewLine));
                    }
                }
                cat.BaseDeDatos.CerrarConexion();

            }
        }

    }
}
