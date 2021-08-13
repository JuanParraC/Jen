// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using DbDataReader = System.Data.Common.DbDataReader;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using System.Security.Permissions;

    /// <summary>
    /// <c>Traductor : </c> clase para el mapeo de listas asociadas a campos
    /// </summary>
    [Serializable]
    public class Traductor : Recipiente<Dato>
    {

        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Traductor()
            : base()
        {
            Id = Lenguaje.Traductor;
            IniTraductor();
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Traductor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniTraductor();
            Serie serie = (Serie)context.Context;
            if (info.GetBoolean(serie.Valor()))
            {
                _consulta = Constructor.Embriones[Lenguaje.Patron].Germinar<Patron>(info, context);
            }

            _estado = (Estado)info.GetValue(serie.Valor(), typeof(Estado));
            _tipoRefresco = (TipoRefresco)info.GetValue(serie.Valor(), typeof(TipoRefresco));

        }
        void IniTraductor()
        {
            Clase = Lenguaje.Traductor;
            _estado = Estado.Instancia | Estado.Excluido;
            NombreContenido = Lenguaje.Traducciones;
            AntesDeEscribirContenedor = escritorXMLTraductor;
            LectorXML = lectorXMLTraductor;
        }
        #endregion

        #region propiedades


        #region consulta
        private Patron _consulta;
        /// <summary>
        /// <c>plantilla : </c> almacena la plantilla del traductor
        /// </summary>
        public Patron Consulta
        {
            get { return _consulta; }
            set
            {
#if RuntimeCache 
                Patron cons = _consulta;
                if (Atributos.Respaldable(Atributo.consulta))
                    Atributos.Agregar(Atributo.consulta, delegate() { _consulta = cons; });
#endif
                _consulta = value;
            }

        }
        #endregion

        #region estado
        /// <summary>
        /// <c>estado : </c> estado actual del objeto
        /// </summary>
        private Estado _estado;
        public new Estado Estado
        {
            get
            {
                return _estado;
            }
            set
            {
                _estado = value;
                // si lo activan carga el traductor
                if (!Semilla.En(value, Estado.Excluido))
                {
                    CargaTraductor();
                }
            }

        }
        #endregion

        #region requerimiento
        /// <summary>
        /// <c>requerimiento : </c> traspasa los valores provenientes de un requerimiento 
        /// hacia el traductor para resolver expresiones 
        /// </summary>
        public void Requerimiento(Requerimiento req)
        {

            // obtiene las variables de la plantilla sql
            Variables variables = _consulta.Variables;

            // si no estan procesadas llama al creador de variables
            if (variables == null)
            {
                variables = _consulta.CrearVariables();
            }

            // si siguie siendo nulo esta consulta no posee criterios
            if (variables == null)
            {
                return;
            }

            if (variables.Existe(this.Id))
            {
                Variable var = variables[this.Id];
                foreach (Declaracion s in var)
                {
                    var.Asignar(req.Campos[s.Identificadores[0]]);
                }

            }
        }
        #endregion

        #region sql
        /// <summary>
        /// sql asociado al traductor
        /// </summary>
        public string SQL
        {
            get
            {
                if (_consulta.Variables.Largo > 0)
                {
                    Registro reg = (Registro)Padre;
                    foreach(Variable v in _consulta.Variables)
                    {
                        if (reg.Campos.Existe(v.Id))
                        {
                            v.Asignar(reg.Campos[v.Id].Valor);
                        }
                    }
                }
                return _consulta.Pintar();
            }
        }
        #endregion

        #region tipoRefresco
        TipoRefresco _tipoRefresco;
        /// <summary>
        /// define el tipo de refresco del traductor
        /// </summary>
        public TipoRefresco TipoRefresco
        {
            get
            {
                return _tipoRefresco;
            }
            set
            {
                _tipoRefresco = value;
            }
        }
        #endregion

        #endregion

        #region métodos

        #region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            Serie serie = (Serie)context.Context;
            if (_consulta != null)
            {
                info.AddValue(serie.Valor(), true);
                _consulta.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false);
            }

            info.AddValue(serie.Valor(), _estado, typeof(Estado));
            info.AddValue(serie.Valor(), _tipoRefresco, typeof(TipoRefresco));
        }
        #endregion

        #region restaurar
        /// <summary>
        /// <c>restaurar : </c> vuelve el objeto al estado inicial.
        /// </summary>  
#if RuntimeCache
        public override void Restaurar()
        {
            Atributos.Restauradores.Add(
                delegate()
                {
                    Estado estadoTraductor = _estado;

                    if (!Semilla.En(Estado, Estado.Excluido))
                    {
                        estadoTraductor |= Estado.Excluido;
                    }

                    if (Semilla.En(Estado, Estado.Ocupado))
                    {
                        estadoTraductor -= Estado.Ocupado;
                    }

                    if (TipoRefresco == TipoRefresco.CadaVez)
                    {    
                        if (Semilla.En(Estado, Estado.Inicializado))
                        {
                            estadoTraductor -= Estado.Inicializado;
                        }
                    }
                    _estado = estadoTraductor;
                }
            );
            if (Consulta != null)
            {
                Consulta.Restaurar();
            }
            base.Restaurar();
        }
#endif
        #endregion

        public void CargaTraductor(BaseDatos bd = null)
        {
            // si el traductor solo se carga una vez y ya está inicializado, no lo vuelve a cargar
            if ((_tipoRefresco == TipoRefresco.UnaVez) && (En(_estado, Estado.Inicializado)))
            {
                return;
            }
            if (bd == null)
            {
                bd = ((Registro)Padre).BaseDeDatos;
            }
            using (ILectorBD datos = bd.OperacionBD(SQL))
            {
                Borrar();
                if (datos.HasRows)
                {
                    while (datos.Read())
                    {
                        Dato trad = new Dato();
                        trad.Id = datos.GetValue(0).ToString();
                        trad.Valor = datos.GetValue(1).ToString();
                        Agregar(trad);
                    }
                    _estado |= Estado.Inicializado;
                }
            }
        }
        #endregion

        #region WriteXml
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorXMLTraductor(System.Xml.XmlWriter writer)
        {
            if (_consulta != null)
            {
                writer.WriteAttributeString(Lenguaje.Consulta, _consulta.Valor);
            }

            writer.WriteAttributeString(Lenguaje.Refresco, _tipoRefresco.ToString());

        }
        #endregion
        void lectorXMLTraductor(System.Xml.XmlReader reader)
        {
            string Consulta = reader.GetAttribute(Lenguaje.Consulta);
            if (!string.IsNullOrEmpty(Consulta))
            {
                _consulta = new Patron(Consulta);
            }

            string Refresco = reader.GetAttribute(Lenguaje.Refresco);
            if (!string.IsNullOrEmpty(Refresco))
            {
                _tipoRefresco = (TipoRefresco)System.Enum.Parse(typeof(TipoRefresco), Refresco, true);
            }
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                if (reader.IsStartElement(Lenguaje.Traducciones))
                {
                    LectorXMLRecipiente(reader);
                }
            }
        }
    }
}