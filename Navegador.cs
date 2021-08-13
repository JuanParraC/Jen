#if BDMD
namespace Jen
{
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    class Navegador : Eventos<Navegador>
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Navegador()
            : base()
        {
            Id = Lenguaje.Consultor;
            IniNavegador();

        }
        /// <summary>
        /// Constructor binario
        /// </summary>
        /// <param name="info">información de la serialización</param>
        /// <param name="context">Contexto del proceso de serialización</param>
        protected Navegador(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IniNavegador();
        }
        void IniNavegador()
        {
        }

        protected override Navegador Ambito()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif