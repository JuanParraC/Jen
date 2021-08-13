// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using System.Collections.Generic;

    /// <summary>
    /// Contenedor de delegados
    /// </summary>
    /// <typeparam name="TContexto"></typeparam>
    [Serializable]
    public abstract class Eventos<TContexto> : Recipiente<Evento<TContexto>>
        where TContexto : ISemilla
    {

        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Eventos()
        {
            IniEventos();
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Eventos(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniEventos();
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            int disparadores = info.GetInt32(serie.Valor());
            int eventos;
            for (int i = 0; i < disparadores; i++)
            {
                eventos = info.GetInt32(serie.Valor());
                Evento ev = (Evento)info.GetValue(serie.Valor(), typeof(Evento));
                disparador.Add(ev, new System.Collections.Generic.List<int>());
                for (int j = 0; j < eventos; j++)
                    disparador[ev].Add(info.GetInt32(serie.Valor()));
            }
        }
        void IniEventos()
        {
            disparador = new System.Collections.Generic.Dictionary<Evento,
                                System.Collections.Generic.List<int>>();
            NombreContenido = Lenguaje.Eventos;
            DespuesDeEscribirContenedor = escritorEventos;
            LeerPropiedades = lectorXMLEventos;
        }
        protected abstract TContexto Ambito();

        #endregion
        internal Dictionary<Evento, List<int>> disparador;

        public Dictionary<Evento, List<int>> Disparadores { get { return disparador; } }

        #region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // obtiene la serie del contexto de la serializacion
            Serie serie = (Serie)context.Context;
            info.AddValue(serie.Valor(), disparador.Count, typeof(int));
            foreach (Evento ev in disparador.Keys)
            {
                info.AddValue(serie.Valor(), disparador[ev].Count, typeof(int));
                info.AddValue(serie.Valor(), ev, typeof(Evento));
                foreach (int pos in disparador[ev])
                    info.AddValue(serie.Valor(), pos, typeof(int));
            }
        }
        #endregion
        #region ejecutar
        public virtual bool Ejecutar(Evento trigger)
        {
            if (disparador.ContainsKey(trigger))
            {
                for (int i = 0; i < disparador[trigger].Count; i++)
                {
                    Evento<TContexto> ev = this[disparador[trigger][i] - 1 /*- 1*/];
                    if (!Semilla.En(ev.Estado, Estado.Excluido))
                    {
                        ev.Ejecutar();
                        if (Semilla.En(Estado, Estado.Error))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        //public void Eliminar(Evento<TContexto> evento)
        //{
        //    base.Eliminar(evento.Id);
        //}
        public override void Agregar(params Evento<TContexto>[] obj)
        {
            base.Agregar(obj);
            foreach (Evento<TContexto> c in obj)
            {
                c.Contexto = Ambito();
            }
        }
        public void Inicializar()
        {
            List<string> ls = new List<string>();
            foreach (Evento<TContexto> ec in this)
            {
                ls.Add(ec.Id);
            }

            foreach (string ec in ls)
            {
                this.Eliminar(ec);
            }
            disparador = new Dictionary<Evento, List<int>>();
        }
        public void Agregar(Evento<TContexto> obj, params Evento[] eventos)
        {
            int pos = Largo + 1;
            //int pos = Largo;
            //obj.Contexto = obtenerContexto();
            obj.Contexto = Ambito();
            base.Agregar(obj);
            foreach (Evento ev in eventos)
            {
                if (!disparador.ContainsKey(ev))
                {
                    disparador.Add(ev, new List<int>());
                }
                disparador[ev].Add(pos);
#if RuntimeCache 
                // esta agregando programaticamente 
                if (Semilla.Ocupado(this))
                    // agrega el restaurador 
                    Atributos.Restauradores.Add(
                        delegate ()
                        {
                            //if (disparador[ev].Contains(pos ))
                            {
                                disparador[ev].RemoveAt(pos - 1);
                            }

                            /*try
                            {
                                disparador[ev].RemoveAt(pos - 1);
                            }
                            catch (System.Exception e)
                            {
                                 e.ToString();
                            }*/


                        }
                );
#endif
            }
        }
        #region lectorXMLEventos
        /// <summary>
        /// <c>ReadXml : </c>Genera el objeto desde su representación XML.
        /// </summary>
        /// <mr name="reader">Tipo que permite leer el Archivo xml</mr>
        internal void lectorXMLEventos(System.Xml.XmlReader reader)
        {
            string ev;
            string pos;
            Evento evento;

            reader.Read();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                ev = reader.GetAttribute(Lenguaje.Evento);
                pos = reader.GetAttribute(Lenguaje.Posicion);
                if (!string.IsNullOrEmpty(ev))
                {
                    evento = (Evento)System.Enum.Parse(typeof(Evento), ev, true);
                    if (!disparador.ContainsKey(evento))
                    {
                        disparador.Add(evento, new List<int>());
                    }
                    disparador[evento].Add(int.Parse(pos, Util.Cultura));
                }
                reader.Read();
            }
            reader.Read();
        }
        #endregion

        #region escritorEventos
        /// <summary>
        /// <c>WriteXml : </c>Convierte el objecto a su representación XML .
        /// </summary>
        /// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
        void escritorEventos(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement(Lenguaje.Disparadores);
            foreach (Evento ev in disparador.Keys)
                foreach (int pos in disparador[ev])
                {
                    writer.WriteStartElement(Lenguaje.Disparador);
                    writer.WriteAttributeString(Lenguaje.Evento, ev.ToString());
                    writer.WriteAttributeString(Lenguaje.Posicion, pos.ToString(Util.Cultura));
                    writer.WriteEndElement();
                }
            writer.WriteEndElement();
        }
        #endregion
    }
}