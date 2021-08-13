// Autor : Juan Parra
// 3Soft
namespace Jen
{
    using System.Security.Permissions;
    using System.Web;
    using Serializable = System.SerializableAttribute;
    using SerializationInfo = System.Runtime.Serialization.SerializationInfo;
    using StreamingContext = System.Runtime.Serialization.StreamingContext;

    /// <summary>
    /// <c> Campo : </c> representación de un origen de una tabla
    /// </summary>
    [Serializable]
	public class Campo : Eventos<Campo>, IEtiquetaIConsejos, IValor, ITabla
	{
		internal string ordenamiento = Lenguaje.Asc;
		internal bool _corteControl = false;
		#region propiedades
		#region propiedad ayuda
		//declara el origen _usuario privado para la propiedad
		private string _ayuda =  string.Empty;

		/// <summary>
		/// <c>usuario : </c> propiedad usuario.
		/// </summary>  
		public string Ayuda
		{
			get { return _ayuda; }
			set
			{
				#if RuntimeCache             
				//Atributos.Asignar<string>(Atributo.ayuda, _ayuda, value);
				string ay = _ayuda;
				if (Atributos.Respaldable(Atributo.ayuda))
					Atributos.Agregar(Atributo.ayuda, delegate() { _ayuda = ay; });
				#endif
				_ayuda = value;
			}

		}
		#endregion
		#region propiedad autonumerico
		//declara el origen _clave privado para la propiedad
		private bool _autonumerico;

		/// <summary>
		/// <c>autonumerico : </c> propiedad autonumerico.
		/// </summary>  
		public bool Autonumerico 
		{
			get { return _autonumerico; }
			set 
			{
				#if RuntimeCache 
				bool autonum = _autonumerico;
				if (Atributos.Respaldable(Atributo.autonumerico))
					Atributos.Agregar(Atributo.autonumerico, delegate() { _autonumerico = autonum; });
				#endif
				_autonumerico = value; 
			}

		}
		#endregion
		#region propiedad clave
		//declara el origen _clave privado para la propiedad
		private bool _clave;

		/// <summary>
		/// <c>registroClave : </c> propiedad registroClave.
		/// </summary>  
		public bool Clave 
		{
			get { return _clave; }
			set 
			{
				#if RuntimeCache 
				bool clv = _clave;
				if (Atributos.Respaldable(Atributo.clave))
					Atributos.Agregar(Atributo.clave, delegate() { _clave = clv; });
				#endif
				_clave = value;
			}

		}
		#endregion
		#region propiedad consejos
		//declara el origen _driver privado para la propiedad
		private string _consejos =  string.Empty;

		/// <summary>
		/// <c>driver : </c> propiedad driver.
		/// </summary>  
		public string Consejos 
		{
			get 
			{ 
				return _consejos; 
			}
			set 
			{
				#if RuntimeCache 
				string cjs = _consejos;
				if (Atributos.Respaldable(Atributo.consejos))
					Atributos.Agregar(Atributo.consejos, delegate() { _consejos = cjs; });
				#endif
				_consejos = value; 
			}

		}
		#endregion
		#region propiedad etiqueta
		//declara el origen _etiqueta privado para la propiedad
		private string _etiqueta =  string.Empty;

		/// <summary>
		/// <c>etiqueta : </c> propiedad etiqueta.
		/// </summary>  
		public string Etiqueta 
		{
			get { return _etiqueta; }
			set
			{
				#if RuntimeCache
				string et = _etiqueta;
				if (Atributos.Respaldable(Atributo.etiqueta))
					Atributos.Agregar(Atributo.etiqueta, delegate() { _etiqueta = et; });
				#endif
				_etiqueta = value; 
			}

		}
		#endregion
		#region propiedad expresionSql
		//declara el origen _expresionSql privado para la propiedad
		private bool _expresionSql =  false;

		/// <summary>
		/// <c>expresionSql : </c> propiedad expresionSql.
		/// </summary>  
		public bool ExpresionSql 
		{
			get { return _expresionSql; }
			set 
			{

				#if RuntimeCache
				bool expSql = _expresionSql;
				if (Atributos.Respaldable(Atributo.expresionSql))
					Atributos.Agregar(Atributo.expresionSql, delegate() { _expresionSql = expSql; });
				#endif
				_expresionSql = value; 
			}

		}
		#endregion
		#region propiedad formato
		//declara el origen _formato privado para la propiedad
		private string _formato =  string.Empty;

		/// <summary>
		/// <c>formato : </c> propiedad formato.
		/// </summary>  
		public string Formato 
		{
			get { return _formato; }
			set 
			{
				#if RuntimeCache
				string fmt = _formato;
				if (Atributos.Respaldable(Atributo.formato))
					Atributos.Agregar(Atributo.formato, delegate() { _formato = fmt; });
				#endif
				_formato = value; 
			}

		}
		#endregion
		#region propiedad largo
		//declara el origen _largo privado para la propiedad
		private string _largo = string.Empty;

		/// <summary>
		/// <c>largo : </c> propiedad largo.
		/// </summary>  
		new public string Largo 
		{
			get { return _largo; }
			set 
			{
				#if RuntimeCache
				string lrg = _largo;
				if (Atributos.Respaldable(Atributo.largo))
					Atributos.Agregar(Atributo.largo, delegate() { _largo = lrg; });
				#endif
				_largo = value; 
			}

		}
		#endregion
		#region propiedad nombre
		private string _nombre;
		/// <summary>
		/// nombre : nombre del origen
		/// </summary>
		public string Nombre 
		{
			get { return _nombre; }
			set 
			{
				#if RuntimeCache
				string nom = _nombre;
				if (Atributos.Respaldable(Atributo.nombre))
					Atributos.Agregar(Atributo.nombre, delegate() { _nombre = nom; });
				#endif
				_nombre = value; 
			}

		}
		#endregion
		#region propiedad operador
		//declara el origen _operador privado para la propiedad
		private string _operador =  Lenguaje.Igual;

		/// <summary>
		/// <c>operador : </c> propiedad operador.
		/// </summary>  
		public string Operador 
		{
			get { return _operador; }
			set 
			{
				#if RuntimeCache
				string op = _operador;
				if (Atributos.Respaldable(Atributo.operador))
					Atributos.Agregar(Atributo.operador, delegate() { _operador = op; });
				#endif
				_operador = value; 
			}

		}
		#endregion
		#region propiedades
		private Propiedades _propiedades;
		/// <summary>
		/// <c>propiedades : </c> Permite acceder a la coleccion de propiedades del objeto
		/// </summary>
		public Propiedades Propiedades 
		{
			get { return _propiedades; }
			set 
			{
				#if RuntimeCache
				Propiedades prop = _propiedades;
				if (Atributos.Respaldable(Atributo.propiedades))
					Atributos.Agregar(Atributo.propiedades, delegate() { _propiedades = prop; });
				#endif
				_propiedades = value; 
			}

		}
		#endregion
		#region propiedad solo lectura
		private bool _soloLectura = false;
		/// <summary>
		/// <c>soloLectura : </c> indica si el objeto en de solo lectura
		/// </summary>
		public bool SoloLectura 
		{
			get { return _soloLectura; }
			set 
			{
				#if RuntimeCache
				bool solLec = _soloLectura;
				if (Atributos.Respaldable(Atributo.soloLectura))
					Atributos.Agregar(Atributo.soloLectura, delegate() { _soloLectura = solLec; });
				#endif
				_soloLectura = value; 
			}

		}
		#endregion
		#region propiedad tabla
		//declara el origen _tabla privado para la propiedad
		private string _tabla =  string.Empty;

		/// <summary>
		/// <c>tabla : </c> propiedad tabla.
		/// </summary>  
		public string Tabla 
		{
			get { return _tabla; }
			set 
			{
				#if RuntimeCache
				string tb = _tabla;
				if (Atributos.Respaldable(Atributo.tabla))
					Atributos.Agregar(Atributo.tabla, delegate() { _tabla = tb; });
				#endif
				_tabla = value; 
			}

		}
		#endregion
		#region propiedad tabla
		//declara el operador de union logica de criterios privado para la propiedad
		private string _andOr = Lenguaje.And;

		/// <summary>
		/// <c>tabla : </c> propiedad tabla.
		/// </summary>  
		public string AndOr
		{
			get { return _andOr; }
			set
			{
				_andOr = value;
			}

		}
		#endregion
		#region propiedad tipo
		//declara el origen _tabla privado para la propiedad
		private Tipo _tipo;

		/// <summary>
		/// <c>Tipo : </c> propiedad Tipo.
		/// </summary>  
		public Tipo Tipo 
		{
			get { return _tipo; }
			set 
			{
				#if RuntimeCache
				Tipo tp = _tipo;
				if (Atributos.Respaldable(Atributo.tipo))
					Atributos.Agregar(Atributo.tipo, delegate() { _tipo = tp; });
				#endif
				_tipo = value; 
			}

		}
		#endregion
		#region propiedad traductor
		//declara el origen _traductor privado para la propiedad
		private string _traductor =  string.Empty;

		/// <summary>
		/// <c>traductor : </c> propiedad traductor.
		/// </summary>  
		public string Traductor 
		{
			get { return _traductor; }
			set 
			{
				#if RuntimeCache
				string trad = _traductor;
				if (Atributos.Respaldable(Atributo.traductor))
					Atributos.Agregar(Atributo.traductor, delegate() { _traductor = trad; });
				#endif
				_traductor = value; 
			}

		}
		#endregion
		#region propiedad unico
		//declara el origen _unico privado para la propiedad
		private bool _unico;

		/// <summary>
		/// <c>clave : </c> propiedad clave.
		/// </summary>  
		public bool Unico 
		{
			get { return _unico; }
			set 
			{
				#if RuntimeCache
				bool unic = _unico;
				if (Atributos.Respaldable(Atributo.unico))
					Atributos.Agregar(Atributo.unico, delegate() { _unico = unic; });
				#endif
				_unico = value; 
			}

		}
		#endregion
		#region propiedad valor
		//declara el origen _valor privado para la propiedad
		internal object _valor;
		public object Objeto{
            get
            {
                return _valor;
            }
            set
            {
                _valor = value;
            }
		}

		/// <summary>
		/// <c>valor : </c> propiedad valor.
		/// </summary>  
		public string Valor
		{
			// si desea el valor crudo use ToString
			get
			{
				//return _valor.ToString();
				return this[_valor_].Valor;
			}
			set 
			{
				#if RuntimeCache
				if (Atributos.Respaldable(Atributo.valor))
				{
					string val = null;
					if(_valor != null)
						val = _valor.ToString();
					Atributos.Agregar(Atributo.valor,
						delegate()
						{
							// restaura el valor anterior
							_valor = val;
							// actualiza los otros valores
							Ejecutar(Evento.AlCambiarValor);
						}
					);

				}
				#endif
				_valor = value;
				Ejecutar(Evento.AlCambiarValor);
			}
		}
		#endregion
		#endregion
		#region constructor
		internal string _valor_;
		/// <summary>
		/// Costructor por defecto
		/// </summary>
		public Campo()
			: base() 
		{
			Id = Lenguaje.Campo;
			IniCampo();
		}
		/// <summary>
		/// Constructor binario
		/// </summary>
		/// <param name="info">información de la serialización</param>
		/// <param name="context">Contexto del proceso de serialización</param>
		protected Campo(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			IniCampo();
			// obtiene la serie del contexto de la serializacion
			Serie serie = (Serie)context.Context;
			_andOr = info.GetString(serie.Valor());
			_ayuda = info.GetString(serie.Valor());
			_autonumerico = info.GetBoolean(serie.Valor());
			_clave = info.GetBoolean(serie.Valor());
			_consejos = info.GetString(serie.Valor());
			_etiqueta = info.GetString(serie.Valor());
			_expresionSql = info.GetBoolean(serie.Valor());
			_formato = info.GetString(serie.Valor());
			_largo = info.GetString(serie.Valor());
			_nombre = info.GetString(serie.Valor());
			_operador = info.GetString(serie.Valor());
            _valor_ = info.GetString(serie.Valor());

            if (info.GetBoolean(serie.Valor()))
            {
                _propiedades = Constructor.Embriones[Lenguaje.Propiedades].Germinar<Propiedades>(info, context);
            }
			_soloLectura = info.GetBoolean(serie.Valor());
			_tabla = info.GetString(serie.Valor());
			Tag = info.GetString(serie.Valor());
			_tipo = (Tipo)info.GetValue(serie.Valor(), typeof(Tipo));
			_traductor = info.GetString(serie.Valor());
			_unico = info.GetBoolean(serie.Valor());
		}
		void IniCampo()
		{
			Clase = Lenguaje.Campo;
			Genero = Genero.Masculino;
			Numero = Numero.Singular;
			AntesDeEscribirContenedor = escritorXMLCampo;
			LeerPropiedades = lectorXMLCampo;
			LeerAtributos = lectorXMLEventos;
		}
		protected override Campo Ambito()
		{
			return this;
		}


		#endregion
		#region metodos

		#region corteControl
		/// <summary>
		/// <rc>corteControl : </rc> setea corte de control.
		/// </summary>  
		public Campo CorteControl()
		{
			#if RuntimeCache  
			bool corContr = _corteControl;
			if (Atributos.Respaldable(Atributo.corteControl))
				Atributos.Agregar(Atributo.corteControl, 
					delegate() 
					{
						_corteControl = corContr; 
					}
				);
			#endif
			_corteControl = true;
			return this;
		}
		#endregion

		#region Crear
		/// <summary>
		/// <rc>corteControl : </rc> setea corte de control.
		/// </summary>  
		public static Campo Crear(string strCatalogo, string strTabla, string strCampo, HttpRequest request = null)
		{

			Catalogo catalogo = Catalogo.Modelo(strCatalogo, "['" + strTabla + "']", request);
			// direcciona la tabla
			Registro registro = catalogo[strTabla];
			// direcciona el campo
			Campo campo = registro.Campos[strCampo];
			Jen.Campo ret = new Jen.Campo();
			ret.Id = string.Concat(campo.Id, "1");
			ret.Agregar(new Jen.Eventos.Valor(), Evento.AlCambiarValor);
			return ret;
		}
		#endregion
		#region descendente
		/// <summary>
		/// <c>descendente : </c> setea orden descendente.
		/// </summary>  
		public Campo Descendente() 
		{
			#if RuntimeCache  
			if (Atributos.Respaldable(Atributo.ordenamiento))
				Atributos.Agregar(Atributo.ordenamiento, delegate() { ordenamiento = Lenguaje.Asc; } );
			#endif
			ordenamiento = Lenguaje.Desc;
			return this; 
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
			// obtiene la serie del contexto de la serializacion
			Serie serie = (Serie)context.Context;
			info.AddValue(serie.Valor(), _andOr);
			info.AddValue(serie.Valor(), _ayuda);
			info.AddValue(serie.Valor(), _autonumerico);
			info.AddValue(serie.Valor(), _clave);
			info.AddValue(serie.Valor(), _consejos);
			info.AddValue(serie.Valor(), _etiqueta);
			info.AddValue(serie.Valor(), _expresionSql);
			info.AddValue(serie.Valor(), _formato);
			info.AddValue(serie.Valor(), _largo);
			info.AddValue(serie.Valor(), _nombre);
			info.AddValue(serie.Valor(), _operador);
            info.AddValue(serie.Valor(), _valor_);

            // verifica si hay propiedades
            if (_propiedades != null)
            {
                info.AddValue(serie.Valor(), true, typeof(bool));
                _propiedades.GetObjectData(info, context);
            }
            else
            {
                info.AddValue(serie.Valor(), false, typeof(bool));
            }
			info.AddValue(serie.Valor(), _soloLectura);
			info.AddValue(serie.Valor(), _tabla);
			info.AddValue(serie.Valor(), Tag);
			info.AddValue(serie.Valor(), _tipo, typeof(Tipo));
			info.AddValue(serie.Valor(), _traductor);
			info.AddValue(serie.Valor(), _unico);

		}
		#endregion
		#region limpiar
		/// <summary>
		/// <c>limpiar : </c> limpia el campo sin notificar a los valores
		/// </summary>  
		internal void Limpiar() 
		{
			_valor = string.Empty;
		}
		#endregion
		#region ToString
		/// <summary>
		/// <c>ToString : </c> retorna el valor crudo del semilla.
		/// </summary>  
		public override string ToString() 
		{ 
			if (_valor == null)
				return string.Empty;
			return _valor.ToString(); 
		}
		#endregion

		#region escritorXMLCampo
		/// <summary>
		/// Convierte el objecto a su representación XML .
		/// </summary>
		/// <mr name="writer"> Tipo que permite escribir el Archivo xml</mr>
		internal void escritorXMLCampo(System.Xml.XmlWriter writer)
		{
			string val = string.Empty;
            if (_valor != null)
            {
                val = _valor.ToString();
            }
            if (!string.IsNullOrEmpty((string)val))
            {
                writer.WriteAttributeString(Lenguaje.Valor, val);
            }

			if (!string.IsNullOrEmpty(_ayuda))
				writer.WriteAttributeString(Lenguaje.Ayuda, _ayuda);

			if (_autonumerico)
			{
				writer.WriteStartAttribute(Lenguaje.Autonumerico);
				writer.WriteValue(_autonumerico);
				writer.WriteEndAttribute();
			}
			if (_clave)
			{
				writer.WriteStartAttribute(Lenguaje.Clave);
				writer.WriteValue(_clave);
				writer.WriteEndAttribute();
			}
            if (!string.IsNullOrEmpty(_consejos))
            {
                writer.WriteAttributeString(Lenguaje.Consejos, _consejos);
            }

            if (!string.IsNullOrEmpty(_etiqueta))
            {
                writer.WriteAttributeString(Lenguaje.Etiqueta, _etiqueta);
            }

			if(_expresionSql)
			{
				writer.WriteStartAttribute(Lenguaje.ExpresionSQL);
				writer.WriteValue(_expresionSql);
				writer.WriteEndAttribute();
			}
            if (!string.IsNullOrEmpty(_formato))
            {
                writer.WriteAttributeString(Lenguaje.Formato, _formato);
            }

            if (!string.IsNullOrEmpty(_largo))
            {
                writer.WriteAttributeString(Lenguaje.Largo, _largo);
            }

            if (!_operador.Equals(Lenguaje.Igual))
            {
                writer.WriteAttributeString(Lenguaje.Operador, _operador);
            }

            if (!string.IsNullOrEmpty(_nombre))
            {
                writer.WriteAttributeString(Lenguaje.Nombre, _nombre);
            }

			if (_soloLectura)
			{
				writer.WriteStartAttribute(Lenguaje.SoloLectura);
				writer.WriteValue(_soloLectura);
				writer.WriteEndAttribute();
			}
            if (!string.IsNullOrEmpty(Tag))
            {
                writer.WriteAttributeString(Lenguaje.Tag, Tag);
            }

            if (!string.IsNullOrEmpty(_tabla))
            {
                writer.WriteAttributeString(Lenguaje.Tabla, _tabla);
            }

            if (Tipo.SinTipo != _tipo)
            {
                writer.WriteAttributeString(Lenguaje.Tipo, _tipo.ToString());
            }

            if (!string.IsNullOrEmpty(_traductor))
            {
                writer.WriteAttributeString(Lenguaje.Traductor, _traductor);
            }

			if (_unico)
			{
				writer.WriteStartAttribute(Lenguaje.Unico);
				writer.WriteValue(_unico);
				writer.WriteEndAttribute();
			}

            if (_propiedades != null)
            {
                _propiedades.escritorXMLPropiedades(writer);
            }
		}
        #endregion
        void lectorXMLCampo(System.Xml.XmlReader reader)
        {
            string strValor = reader.GetAttribute(Lenguaje.Valor);
            if (!string.IsNullOrEmpty(strValor))
            {
                _valor = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.AndOr);
			if (!string.IsNullOrEmpty(strValor))
			{
				_andOr = strValor;
			}
			strValor = reader.GetAttribute(Lenguaje.Ayuda);
            if (!string.IsNullOrEmpty(strValor))
            {
                _ayuda = strValor;
            }

			strValor = reader.GetAttribute(Lenguaje.Autonumerico);
            if (!string.IsNullOrEmpty(strValor))
            {
                _autonumerico = bool.Parse(strValor);
            }
			strValor = reader.GetAttribute(Lenguaje.Clave);
            if (!string.IsNullOrEmpty(strValor))
            {
                _clave = bool.Parse(strValor);
            }
			string Consejos = reader.GetAttribute(Lenguaje.Consejos);
            if (!string.IsNullOrEmpty(Consejos))
            {
                _consejos = Consejos;
            }
			strValor = reader.GetAttribute(Lenguaje.Etiqueta);
            if (!string.IsNullOrEmpty(strValor))
            {
                _etiqueta = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.ExpresionSQL);
            if (!string.IsNullOrEmpty(strValor))
            {
                _expresionSql = bool.Parse(strValor);
            }
			strValor = reader.GetAttribute(Lenguaje.Formato);
            if (!string.IsNullOrEmpty(strValor))
            {
                _formato = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.Largo);
            if (!string.IsNullOrEmpty(strValor))
            {
                _largo = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.Operador);
            if (!string.IsNullOrEmpty(strValor))
            {
                _operador = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.Nombre);
            if (!string.IsNullOrEmpty(strValor))
            {
                _nombre = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.SoloLectura);
            if (!string.IsNullOrEmpty(strValor))
            {
                _soloLectura = bool.Parse(strValor);
            }
			strValor = reader.GetAttribute(Lenguaje.Tag);
            if (!string.IsNullOrEmpty(strValor))
            {
                Tag = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.Tabla);
            if (!string.IsNullOrEmpty(strValor))
            {
                _tabla = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.Tipo);
            if (!string.IsNullOrEmpty(strValor))
            {
                _tipo = (Tipo)System.Enum.Parse(typeof(Tipo), strValor, true);
            }
			strValor = reader.GetAttribute(Lenguaje.Traductor);
            if (!string.IsNullOrEmpty(strValor))
            {
                _traductor = strValor;
            }
			strValor = reader.GetAttribute(Lenguaje.Unico);
            if (!string.IsNullOrEmpty(strValor))
            {
                _unico = bool.Parse(strValor);
            }
            if (reader.IsEmptyElement)
            {
                return;
            }
			reader.Read();
			if (reader.IsStartElement(Lenguaje.Propiedades))
			{
				_propiedades = new Propiedades();
				_propiedades.lectorXMLPropiedades(reader);
				reader.Read();
			}

		}
		#endregion

	}
}