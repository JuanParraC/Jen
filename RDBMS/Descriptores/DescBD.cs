
namespace Jen.RDBMS.Descriptores
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using Jen;
    using Jen.Json;

    // clase abstracta que define la funcionalidad que debe implementar un descriptor de base de datos en particular.
    public abstract class DescBD : Semilla
    {
        
        string _driver;
        Recipiente<Recipiente<Dato>> tabDesc;
        Recipiente<Recipiente<Dato>> dominio;

        public DescBD()
        {
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected DescBD(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public DescBD(Objeto cfg) : this(cfg["id"].ToString(),cfg["driver"].ToString())
        {
        }
        public DescBD(string id, string driver)
        {
            Id = id; _driver = driver;
            if (!string.IsNullOrEmpty(driver))
            {
                tabDesc = cargarTabla(SQLSelectTabDesc);
                dominio = cargarTabla(SQLSelectDominio);
            }
        }
        public abstract void CrearBD(Recipiente<Dato> inst);

        //public DescBD DescriptorDestino { get; set; }
        public string Esquemas
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                string esquema;
                List<string> ls = new List<string>();
                foreach (Recipiente<Dato> rd in tabDesc)
                {
                    esquema = rd["ESQUEMA"].Valor;
                    if (!ls.Contains(esquema))
                    {
                        sb.Append("'");
                        esquema = rd["ESQUEMA"].Valor;
                        sb.Append(esquema.Substring(0, esquema.Length - 1).ToLower());
                        sb.Append("',");
                        ls.Add(esquema);
                    }
                }
                sb.Length--;
                return sb.ToString();
            }
        }
        public abstract Recipiente<Recipiente<Dato>> UPDTabDescSEQ(Catalogo cat);
        public abstract void SQLCreacionBase(Catalogo cat, Objeto cfg, Recipiente<Dato> instr);

        public abstract string SQLCatalogoTablas();
        public abstract string SQLCatalogoTablas(Arreglo tablas);
        public abstract string SQLCatalogoColumnas(string tabla, bool orden = true);
        public abstract string SQLCatalogoColumna(string tabla, string columna);
        public abstract string SQLComentarioTabla(string tabla, string comentario);
        public abstract string SQLComentarioColumna(string tabla, string columna, string comentario);

        public abstract string SQLClavePrimariaTabla(string tabla);
        public abstract string SQLRelaciones(string tabla);
        public abstract string SQLCamposTraducidos();

        public abstract string SQLFunciones();

        public abstract string SQListaFunciones();

        public abstract void ScriptSecuencias(Recipiente<Recipiente<Dato>> tabDesc, Catalogo catDest, Recipiente<Dato> instr);

        public abstract void ScriptComentarios(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr);

        public abstract void DialectoRelaciones(Recipiente<Dato> instr, Recipiente<Dato> tPadre , Recipiente<Dato> thija, params string[] p);

        public abstract void DialectoFunciones(Recipiente<Dato> instr, params string[] p);

        public void ScriptInserciones(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            int iId = instr.Largo;
            //var l = r.OrderBy(key => key.Key);

            List<string> orden = new List<string>(catOrigen.Registros.Largo);
            Dictionary<string, string> dic = new Dictionary<string, string>(catOrigen.Registros.Largo);
            foreach (Registro r in catDestino.Registros)
            {
                orden.Add(r.Tabla);
                dic.Add(r.Tabla, r.Id);
            }
            orden.Sort();
            Registro reg;
            string esquema = string.Empty;
            foreach (string r in orden)
            {
                reg = catOrigen.Registros[dic[r]];
                if (!En(reg.Estado, Estado.Excluido))
                {


                    Consultor cons = new Consultor();
                    cons.Consultado = catOrigen;

                    Campo clave = reg.Campos.Claves(false)[0];
                    if (clave != null)
                    {
                        cons.Ordenadores = new Recipiente<Campo>();
                        cons.Ordenadores.Agregar(clave);
                    }
                    //BaseDatos bdOriginal = reg.BaseDeDatos;
                    //bdDestino.Contexto = reg;
                    Registro regDestino = catDestino.Registros[reg.Id];
                    catDestino.BaseDeDatos.Contexto = regDestino;
                    //reg.BaseDeDatos = bdDestino;

                    cons.Campos.Agregar(reg.Campos);
                    if (reg.Criterios != null)
                    {
                        cons.Criterios = reg.Criterios;
                    }

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
                                            c.Valor = Lenguaje.Null;
                                        }
                                    }
                                }
                                regDestino.Campos[c.Id].Valor = c.Valor;

                            }

                            instr.Agregar(new Dato() { Id = (++iId).ToString(), Valor = string.Concat(catDestino.BaseDeDatos.ComponerSQLInsertar(), ";") });
                        }
                    }
                    catOrigen.BaseDeDatos.CerrarConexion();
                    //reg.BaseDeDatos = bdOriginal;
                }
            }
        }

        public void ScriptRelaciones(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr)
        {
            string sqlRel = string.Empty;
            int iId = instr.Largo;

            foreach (Registro padres in catOrigen.Registros)
            {
                sqlRel = SQLRelaciones(padres.Tabla);
                using (ILectorBD datos = catOrigen.BaseDeDatos.OperacionBD(sqlRel))
                {
                    while (datos.Read())
                    {
                        Recipiente<Dato> tbHija = TabDesc[datos.GetValue(3).ToString()];
                        Recipiente<Dato> tbPadre = TabDesc[datos.GetValue(1).ToString()];
                        catDestino.BaseDeDatos.Descriptor.DialectoRelaciones(instr, tbPadre, tbHija,
                                    datos.GetValue(1).ToString(), datos.GetValue(2).ToString(), datos.GetValue(3).ToString(),
                                    datos.GetValue(4).ToString(), datos.GetValue(5).ToString());

                    }
                }
            }
        }

        public virtual void ScriptIndices(Catalogo cat, Recipiente<Dato> instr)
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
                                Valor = string.Concat("create index ind_", regHijo.Campos[rel.Hijo[0].Id].Nombre.ToLower(), " on ", regHijo.Tabla,
                                                          "(", regHijo.Campos[rel.Hijo[0].Id].Nombre, ");")
                            });
                        }
                    }
                }
            }

        }

        public abstract void ScriptVistasDominio(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr);

        public abstract void ScriptVistaExtendidas(Registro reg, Recipiente<Dato> instr, bool metacampos = false, bool fueraMmo = false);

        public abstract void ScriptFunciones(Catalogo catOrigen, Catalogo catDestino, Recipiente<Dato> instr);

        public abstract string SQLCreacionSecuencia

        {
            get;
        }
        public virtual Jen.Evento subscripcionSecuencia
        {
            get
            {
                return Jen.Evento.AntesDeInsertar;
            }
        }
        public Recipiente<Recipiente<Dato>> TabDesc
        {
            get
            {
                return tabDesc;
            }
        }
        public Recipiente<Recipiente<Dato>> Dominio
        {
            get
            {
                return dominio;
            }
        }
        Recipiente<Recipiente<Dato>> cargarTabla(string sql)
        {
            Recipiente<Recipiente<Dato>> tabla = new Recipiente<Recipiente<Dato>>();
            if (!string.IsNullOrEmpty(sql))
            {

                using (ILectorBD datos = BaseDatos.OperacionBD(sql))
                {
                    while (datos.Read())
                    {
                        Recipiente<Dato> fila = new Recipiente<Dato>();
                        fila.Id = datos.GetValue(0).ToString();
                        for (int i = 1; i < datos.FieldCount; i++)
                        {
                            fila.Agregar(new Dato() { Id = datos.GetName(i), Valor = datos.GetValue(i).ToString() });
                        }
                        tabla.Agregar(fila);
                    }
                }
            }
            return tabla;
        }

        public abstract string SQLSelectTabDesc { get; }



        public string SQLSelectDominio
        {
            get
            {
                if (tabDesc.Existe("dominio"))
                {
                    return "select dm_id, dm_nombre, dm_acronimo, dm_descripcion, dm_crea_nuevo, dm_padre from " + tabDesc["dominio"]["ESQUEMA"].Valor + "dominio;";
                }
                return string.Empty;
            }
        }
        public string Driver { get { return _driver; } }
        public abstract BaseDatos BaseDatos { get; }
    }
}
