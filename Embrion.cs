// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using System.Web;
    using BindingFlags = System.Reflection.BindingFlags;
    using CallingConventions = System.Reflection.CallingConventions;
    using ConstructorInfo = System.Reflection.ConstructorInfo;
    using DynamicMethod = System.Reflection.Emit.DynamicMethod;
    using ILGenerator = System.Reflection.Emit.ILGenerator;
    using MethodAttributes = System.Reflection.MethodAttributes;
    using MethodInfo = System.Reflection.MethodInfo;
    using OpCodes = System.Reflection.Emit.OpCodes;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;
    using Type = System.Type;
#if LOGGER
    using NLog;
#endif

    /// <summary>
    ///  Embrion : Clase que permite instancia un objeto via un proceso denominado germinacion
    ///  en donde podrá Germinar<T>() y Germinar<T>(SerializationInfo, StreamingContext)
    /// </summary>
    [Serializable]
    public class Embrion : Semilla
    {
#if LOGGER
        static Logger _logger = LogManager.GetCurrentClassLogger();
#endif         
        // tipo del objeto
        Type tipo;
        // punteros a los constructores del objeto
        Func<object> Ctor;

        Func<SerializationInfo, StreamingContext, object> CtorBin;

        Func<Json.Objeto, object> CtorJson;

        #region constructor
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Embrion()
            : base()
        {
            IniEmbrion();
            Id = Lenguaje.Embrion;
        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Embrion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniEmbrion();
            Serie serie = (Serie)context.Context;
            Tipo = info.GetString(serie.Valor());
        }
        void IniEmbrion()
        {
            Clase = Lenguaje.Embrion;
            Genero = Genero.Masculino;
            EscritorXML = escritorXMLEmbrion;
            LectorXML = lectorXMLEmbrion;
        }
        #endregion

        #region propiedades

        public bool ConstructorJson
        {
            get
            {
                if (CtorJson != null)
                {
                    return true;
                }
                return false;
            }
        }
        #region Tipo
        /// <summary>
        /// Tipo : representa el tipo del objeto
        /// </summary>       
        public string Tipo
        {
            get
            {
                return tipo.AssemblyQualifiedName;
            }
            set
            {// obtiene el tipo del objeto

                tipo = Type.GetType(value);
                // para crear el delegado que instancia el objeto
                DynamicMethod met;
                // para generar codigo IL que accede al constructor de la clase
                ILGenerator gen;
                // criterios de binding para encontrar los constructores
                BindingFlags bfsCtor = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                // define la firma del metodo instanciador
                MethodAttributes ma = MethodAttributes.Static | MethodAttributes.Public;
                // define los parametros del constructor por defecto
                Type[] param = new Type[0];
                ConstructorInfo ctor = null;

                // * * * * * * * * * * * Constructor por defecto sin parametros * * * * * * * * * * * *
#if Try
                try
                {
#endif
                ctor = tipo.GetConstructor(bfsCtor, null, param, null);
#if Try
                }
                catch ( System.Exception e)
                {
                    e.ToString();
                }
#endif
                if (ctor != null)
                {
                    // crea un metodo para instanciar el objeto que reside en el embrion
                    met = new DynamicMethod(Lenguaje.Crear, ma, CallingConventions.Standard, tipo, param, tipo, true);
                    // genera el codigo para la invocacion
                    gen = met.GetILGenerator();
                    // empila la instanciación de un objeto en la pila
                    gen.Emit(OpCodes.Newobj, ctor);
                    // coloca la referencia fuera de la pila y lo almacena en local.0
                    //gen.Emit(OpCodes.Stloc_0);
                    // emite un return
                    gen.Emit(OpCodes.Ret);
                    Ctor = (Func<object>)met.CreateDelegate(typeof(Func<object>));
                }
                // define los parametros del constructor biario
                param = new Type[2] { typeof(SerializationInfo), typeof(StreamingContext) };

                // * * * * * * * * * * * * Constructor binario * * * * * * * * * * * *
#if Try
                try
                {
#endif
                    ctor = tipo.GetConstructor(bfsCtor, null, param, null);
#if Try                
                }
                catch(System.Exception e)
                {
                    e.ToString();
                }
#endif
                if (ctor != null)
                {
                    // crea un metodo para instanciar el objeto que reside en el embrion
                    met = new DynamicMethod(Lenguaje.Crear, ma, CallingConventions.Standard, tipo, param, tipo, true);
                    // genera el codigo para la invocacion
                    gen = met.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldarg_1);
                    // empila la instanciación de un objeto en la pila
                    gen.Emit(OpCodes.Newobj, ctor);
                    // coloca la referencia fuera de la pila y lo almacena en local.0
                    //gen.Emit(OpCodes.Stloc_0);
                    // emite un return
                    gen.Emit(OpCodes.Ret);
                    CtorBin = (Func<SerializationInfo, StreamingContext, object>)met.CreateDelegate(typeof(Func<SerializationInfo, StreamingContext, object>));
                }
                // * * * * * * * * * * * * * * * * * * * * * * *
                // define el parametro del constructor json
                param = new Type[1] { typeof(Json.Objeto) };
#if Try
                try
                {
#endif
                    ctor = tipo.GetConstructor(bfsCtor, null, param, null);
#if Try                
                }
                catch (System.Exception e)
                {
                    e.ToString();
                }
#endif
                if (ctor != null)
                {
                    // crea un metodo para instanciar el objeto que reside en el embrion
                    met = new DynamicMethod(Lenguaje.Crear, ma, CallingConventions.Standard, tipo, param, tipo, true);
                    // genera el codigo para la invocacion
                    gen = met.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    //gen.Emit(OpCodes.Ldarg_1);
                    // empila la instanciación de un objeto en la pila
                    gen.Emit(OpCodes.Newobj, ctor);
                    // coloca la referencia fuera de la pila y lo almacena en local.0
                    //gen.Emit(OpCodes.Stloc_0);
                    // emite un return
                    gen.Emit(OpCodes.Ret);
                    CtorJson = (Func<Json.Objeto, object>)met.CreateDelegate(typeof(Func<Json.Objeto, object>));
                }
                if (tipo.GetInterface("ISemilla") != null)
                {
                    //MethodInfo CtorString;
                    MethodInfo mDeserializar = typeof(Constructor).GetMethod("Crear");
                    // CtorString = 
                    mDeserializar.MakeGenericMethod(tipo);
                }
            }
        }
#endregion

#endregion propiedades

#region métodos

#region Germinar
        /// <summary>
        /// Germinar cosntruye el objeto
        /// </summary>  
        public T Germinar<T>(Json.Objeto p)
        {
            //invoca al constructor del embrion
            object emb = CtorJson.Invoke(p);
            return (T)emb;
        }
        public T Germinar<T>(SerializationInfo info, StreamingContext context)
        {
            //invoca al constructor del embrion
            object emb = CtorBin.Invoke(info, context);
            return (T)emb;
        }
        public object Germinar()
        {
            object ret = Ctor.Invoke();
            if (typeof(ISemilla).IsAssignableFrom(ret.GetType()))
            {
                ISemilla semilla = (ISemilla)ret;
                semilla.Id = this.Id;
                return semilla;
            }
            return ret;
        }
        public object Germinar(HttpRequest request)
        {
            object ret = Ctor.Invoke();
            if (typeof(ISemilla).IsAssignableFrom(ret.GetType()))
            {
                ISemilla semilla = (ISemilla)ret;
                semilla.Id = this.Id;
                return semilla;
            }
            return ret;
        }
#endregion
#region GetObjectData
        /// <summary>
        /// <c>GetObjectData : </c>  serializa las propiedades del objeto.
        /// </summary> 
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            Serie serie = (Serie)context.Context;
            // escribe la propiedad tipo
            info.AddValue(serie.Valor(), tipo.AssemblyQualifiedName);
        }
#endregion

        void escritorXMLEmbrion(System.Xml.XmlWriter writer)
        {
            if (Id.Equals(Clase))
            {
                writer.WriteAttributeString(Lenguaje.Id, Id);
            }

            if (tipo != null)
            {
                writer.WriteAttributeString(Lenguaje.Tipo, tipo.AssemblyQualifiedName);
            }
        }
        void lectorXMLEmbrion(System.Xml.XmlReader reader)
        {
            reader.MoveToAttribute(Lenguaje.Tipo);
#if TRACE
            string tipo = reader.ReadContentAsString();
            _logger.Debug(string.Concat("Creando embrion ", tipo));
#endif

            Tipo = reader.ReadContentAsString();
        }

#endregion métodos
    }
}