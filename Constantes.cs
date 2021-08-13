// Autor : Juan Parra
// 3Soft
namespace Jen
{

    #region Atributo
    internal enum Atributo
    {
        accion,
        autonumerico,
        ayuda,
        baseDeDatos,
        clave,
        conexion,
        consejos,
        consulta,
        contador,
        corteControl,
        criterios,
        cuenta,
        datos,
        driver,
        estado,
        etiqueta,
        evento,
        expresionRegular,
        expresionSql,
        formato,
        id,
        indices,
        largo,
        recuperacion,
        nombre,
        objetoGrafico,
        operador,
        ordenamiento,
        padre,
        propiedades,
        proxy,
        registros,
        rel1,
        rel2,
        relacion,
        requerimiento,
        scripts,
        serie,
        soloLectura,
        sql,
        tabla,
        tag,
        tipo,
        tieneTodos,
        traductor,
        unico,
        valor,
        seleccionado

    }
    #endregion

    #region Estado
    /// <summary>
    /// <c>Estado : </c>colección de posibles estados de los objetos del sistema
    /// </summary>
    [System.Flags]
    public enum Estado
    {
        /// <summary>
        /// instancia : estado inicial o por defecto. 
        /// </summary>
        Instancia = 1,
        /// <summary>
        /// inicializado : estado de objeto cuando ha sido inicializado.
        /// </summary> 
        Inicializado = 2,
        /// <summary>
        /// excluido : estado de un objeto cuando no se condidera para las operaciones.
        /// </summary> 
        Excluido = 4,
        /// <summary>
        /// controlado: estado de un objeto cuando otro los esta manipulando
        /// </summary>
        Controlado = 8,
        /// <summary>
        ///  ocupado : estado de un objeto cuando esté en uso por el programador
        /// </summary> 
        Ocupado = 16,
        /// <summary>
        ///  error : hubo un error
        /// </summary> 
        Error = 32,
        /// <summary>
        ///  Inicio : el objeto esta en estado inicial
        /// </summary> 
        Inicio = 64,
        /// <summary>
        ///  Fin : el objeto esta en estado final
        /// </summary> 
        Fin = 128,
        /// <summary>
        ///  Aceptacion : el objeto esta en estado de aceptacion
        /// </summary> 
        Aceptacion = 256

    }
    #endregion

    #region Eventos
    /// <summary>
    /// <c>Evento : </c>colección de posibles eventos de los objetos del sistema
    /// </summary>
    public enum Evento
    {
        /// <summary>
        /// Al cambiar valor
        /// </summary>
        AlCambiarValor,
        /// <summary>
        /// Al instanciar
        /// </summary>
        AlInstanciar,
        /// <summary>
        /// Antes de eliminar
        /// </summary>
        AntesDeEliminar,
        /// <summary>
        /// Antes de componer criterios
        /// </summary>
        AntesDeComponerCriterios,
        /// <summary>
        /// Antes de insertar
        /// </summary>
        AntesDeInsertar,
        /// <summary>
        /// Antes de actualizar
        /// </summary>
        AntesDeActualizar,
        /// <summary>
        /// Antes de pintar
        /// </summary>
        AntesDePintar,
        /// <summary>
        /// Antes de recuperar
        /// </summary>
        AntesDeRecuperar,
        /// <summary>
        /// antes de serializar en formato binario
        /// </summary>
        AntesDeSerializarBinario,
        /// <summary>
        /// despues de actualizar
        /// </summary>
        DespuesDeActualizar,
        /// <summary>
        /// Despues de insertar
        /// </summary>
        DespuesDeInsertar,
        /// <summary>
        /// Despuesd de recuperar
        /// </summary>
        DespuesDeRecuperar,
        /// <summary>
        /// Despues de eliminar
        /// </summary>
        DespuesDeEliminar,
        /// <summary>
        /// Para ejecutar eventos programaticamente antes de ejecutar
        /// </summary>
        AntesDeEjecutar,
        /// <summary>
        /// Para ejecutar eventos programaticamente
        /// </summary>
        Ejecutar,
        /// <summary>
        /// Para ejecutar eventos programaticamente antes de ejecutar
        /// </summary>
        DespuesDeEjecutar
    }
    #endregion

    #region Genero
    /// <summary>
    /// <c>Genero : </c>tipificación del genero del objeto
    /// </summary>
    public enum Genero
    {
        /// <summary>
        /// Genero masculino
        /// </summary>
        Masculino,
        /// <summary>
        /// Genero femenino
        /// </summary>
        Femenino,
        /// <summary>
        /// Genero neutro
        /// </summary>
        Neutro
    }
    #endregion

    #region numero
    /// <summary>
    /// <c>Numero : </c>tipificación del numero del objeto
    /// </summary>
    public enum Numero
    {
        /// <summary>
        /// Singular
        /// </summary>
        Singular,
        /// <summary>
        /// Plural
        /// </summary>
        Plural
    }
    #endregion

    #region ModoDeRecuperacion
    /// <summary>
    /// <c>ModoDeRecuperacion</c> modo de recuperación desde la base de datos
    /// </summary>
    public enum Recuperacion
    {
        /// <summary>
        /// <c>porClave : </c> indica que la recuperación desde la base de datos se realizará 
        /// por la registroClave primaria de la entidad
        /// </summary>
        PorClave,
        /// <summary>
        /// <c>porConsulta :</c> indica que la recuperación desde la base de datos se realizará 
        /// por todos los objetos que contengan un valor 
        /// </summary>
        PorConsulta
    }
    #endregion

    #region TipoRefresco
    /// <summary>
    /// <c>TipoRefresco</c>
    /// </summary>
    public enum TipoRefresco
    {
        /// <summary>
        /// Solo una vez
        /// </summary>
        UnaVez,
        /// <summary>
        /// Cada vez que entra en un contexto http
        /// </summary>
        CadaVez
    }
    #endregion

    #region TipoRelacion
    /// <summary>
    /// <c>rel</c>
    /// </summary>
    public enum TipoRelacion
    {
        /// <summary>
        /// Relación interna
        /// </summary>
        Interna,
        /// <summary>
        /// Relación izquierda
        /// </summary>
        Izquierda,
        /// <summary>
        /// Relación derecha
        /// </summary>
        Derecha
    }
    #endregion

    #region Tipo
    /// <summary>
    /// <c>Tipo</c> tipificación de los campos
    /// </summary>
    public enum Tipo
    {
        /// <summary>
        /// Sin tipo
        /// </summary>
        SinTipo,
        /// <summary>
        /// <c>texto :</c> origen que almacena texto
        /// </summary>
        Texto,
        /// <summary>
        /// <c>Numerico :</c> origen que almacena información numérica
        /// </summary>
        Numerico,
        /// <summary>
        /// <c>strFecha :</c> origen que almacena información del tipo fecha
        /// </summary>
        Fecha,
        /// <summary>
        /// <c>Objeto :</c> origen que almacena información del tipo objeto
        /// </summary>
        Objeto

    }
    #endregion

    #region Lenguaje
    public struct Lenguaje
    {
        public const string AbreParentesis = "(";
        public const string Agrupar = "Agrupar";
        public const string And = " and ";
        public const string AndOr = "AndOr";
        public const string Arroba = "&";
        public const string As = " as ";
        public const string Asc = " asc ";
        public const string Autonumerico = "Autonumerico";
        public const string Ayuda = "Ayuda";
#if SOLinux
        public const string SepDir = "/";
#else
            public const string SepDir = @"\";
#endif
        public const string BackSlashN = "\n";
        public const string BackSlashT = "\t";
        public const string Base64 = "Base64";
        public const string Blancos = " \t\n\r";
        public const string URL = "URL";
        public const string Directorio = "Directorio";
        public const string CarpetaCache = "Cache";
        public const string CarpetaXml = "Xml";
        public const string CalcularAhora = "CalcularAhora";
        public const string CalcularSecuencia = "CalcularSecuencia";
        public const string Campo = "Campo";
        public const string CampoCuenta = " \"Cuenta\" ";
        public const string CampoNoConcuerdaConValidacion = " no concuerda con la validación ";
        public const string Campos = "Campos";
        public const string CargarPrimerRegistro = "CargarPrimerRegistro";
        public const string Catalogo = "Catalogo";
        public const string Cero = "0";
        public const string CheckBox = "CheckBox";
        public const string Chequeado = "checked";
        public const string CierreParentesis = ")";
        public const string Clase = "Clase";
        public const string Clases = "Clases.xml";
        public const string Clave = "Clave";
        public const string ClaveRuntime = "ClaveRuntime";
        public const string Coma = ",";
        public const string ComaEspacio = ", ";
        public const string Comilla = "'";
        public const string ComillaDoble = "\"";
        public const string Comprimir = "Comprimir";
        public const string Conexion = "Conexion";
        public const string Consejo = "Consejo";
        public const string Consejos = "Consejos";
        public const string Consulta = "Consulta";
        public const string Consultor = "Consultor";
        public const string Contador = "Contador";
        public const string ContarRegistros = "ContarRegistros";
        public const string Contenido = "Contenido";
        public const string Contexto = "Contexto";
        public const string Convert = "convert(";
        public const string Count = "count";
        public const string Crear = "c";
        public const string Cuenta = "Cuenta";
        public const string Cubo = "Cubo";
        public const string Cubos = "Cubos";
        public const string d = "d";
        public const string Dir = "dir";
        public const string DateTime = "dateTime";
        public const string Dato = "Dato";
        public const string FmtAhora = "dd/MM/yyyy HH:mm:ss";
        public const string DebeSerNoNulo = "debe tener algún valor";
        public const string DebeSerNumerico = "debe ser tipo numérico";
        public const string Declaracion = "Declaracion";
        public const string Delete = "delete";
        public const string Desc = " desc ";
        public const string Dimension = "Dimension";
        public const string Dimensiones = "Dimensiones";
        public const string DimensionesUsadas = "DimensionesUsadas";
        public const string Disparador = "Disparador";
        public const string Disparadores = "Disparadores";
        public const string Distinct = "distinct ";
        public const string DobleComilla = "''";
        public const string DosPuntos = ":";
        public const string Dual = " dual ";
        public const string dv = "dv";
        public const string DvNoConcuerda = "no concuerda con el dígito verificador ingresado";
        public const string El = "El";
        public const string En = "En ";
        public const string Embrion = "Embrion";
        public const string Error = "Error";
        public const string ErrorConexion = "Ha ocurrido un error al acceder a la base de datos, la conexión se ha cerrado.";
        public const string ErrorDriverBaseDato = "Error al intentar componer la conexión a la base de datos.";
        public const string EsCL = "es-CL";
        public const string Espacio = " ";
        public const string Estado = "Estado";
        public const string Etiqueta = "Etiqueta";
        public const string Evento = "Evento";
        public const string Eventos = "Eventos";
        public const string ExcluirSecuencia = "ExcluirSecuencia";
        public const string Expresion = "Expresion";
        public const string ExpresionSQL = "ExpresionSQL";
        public const string FalloAlValidar = " falló al validar. ";
        public const string False = "FALSE";
        public const string FechaValidaConFormato = "debe ser una fecha válida con el siguiente formato";
        public const string FmtFechasORA = "FmtFechasOracle.xml";
        public const string FmtFechasPGSQL = "FmtFechasPostgreSQL.xml";
        public const string FmtFechasSQL = "FmtFechasSQLServer.xml";
        public const string FmtFechasSQLite = "FmtFechasSQLite.xml";
        public const string Formatear = "Formatear";
        public const string Formato = "Formato";
        public const string Format = "format";
        public const string StrFTime = "strftime";
        public const string From = " from ";
        public const string Funcion = "Funcion"; 
        public const string Funciones = "Funciones";
        public const string Hijo = "Hijo";
        public const string Id = "Id";
        public const string Igual = "=";
        public const string IncluirSecuencia = "IncluirSecuencia";
        public const string IntArrJson = "IntArrJson";
        public const string IntArrXml = "IntArrXml";
        public const string Instancias = "Instancias";
        public const string InsertInto = "insert into ";
        public const string k = "k";
        public const string La = "La";
        public const string Largo = "Largo";
        public const string Las = "Las";
        public const string Limite = "limit";
        public const string Lo = "Lo";
        public const string Login = "login";
        public const string Los = "Los";
        public const string Madre = "Madre";
        public const string MasEntreParentesis = "(+)";
        public const string MayorQue = ">";
        public const string MenorQue = "<";
        public const string Metrica = "Metrica";
        public const string Metricas = "Metricas";
        public const string Motor = "Motor";
        public const string NextVal = ".nextval ";
        public const string MaxNumObjInMem = "MaxNumObjInMem";
        public const string NoFuePosibleEliminar = ", no ha sido posible realizar la eliminación ";
        public const string Nombre = "Nombre";
        public const string Null = "null";
        public const string Numeros = "0123456789+-.eE";
        public const string ObjetoJson = "ObjetoJson";
        public const string BoolJson = "BoolJson";
        public const string BoolXml = "BoolXml";
        public const string TextoJson = "TextoJson";
        public const string TextoXml = "TextoXml";
        public const string Opciones = "Opciones";
        public const string Operador = "Operador";
        public const string Option = "</option>";
        public const string FechaJson = "FechaJson";
        public const string FechaXml = "FechaXml";
        public const string OptionValue = "<option value='";
        public const string Oracle = "ORACLE";
        public const string OrderBy = " order by ";
        public const string Password = "Password";
        public const string Patron = "Patron";
        public const string Plantilla = "Plantilla";
        public const string Por = "*";
        public const string Posicion = "Posicion";
        public const string PostgreSQL = "PostgreSQL";
        public const string Propiedad = "Propiedad";
        public const string Propiedades = "Propiedades";
        public const string Punto = ".";
        public const string Readonly = "Readonly";
        public const string Recipiente = "Recipiente";
        public const string RecipienteCampos = "RecipienteCampos";
        public const string Refresco = "Refresco";
        public const string RegExpComprimir = @"\>\s+\<";
        public const string RegExpInstancias = "@@[^@]*@";
        public const string RegExpVariables = @"\$\{[^}]*}";
        public const string Registro = "Registro";
        public const string RegistroPadreConReferencias = "El registro contiene referencias en la tabla ";
        public const string Registros = "Registros";
        public const string RegistroYaExiste = "El registro ya existe en la tabla ";
        public const string Relacion = "Relacion";
        public const string Relaciones = "Relaciones";
        public const string RowNum = " rowNumI ";
        public const string RowNumber = " row_number() ";
        public const string ScopeIdentity = "scope_identity()";
        public const string Select = "select ";
        public const string SelectCount = "select count(*) ";
        public const string Selected = " selected";
        public const string Semilla = "Semilla";
        public const string SeparadorDecimal = "separadorDecimal";
        public const string Separadores = "Separadores";
        public const string SeparadorInstancias = "[:]";
        public const string Serie = "s";
        public const string Sesion = "sesion";
        public const string SEQ = "SEQ";
        public const string Set = " set ";
        public const string Slash = "/";
        public const string SoloLectura = "SoloLectura";
        public const string Sort = "sort";
        public const string SQLServer = "SQLServer";
        public const string SQLite = "SQLite";
        public const string BDSinc = "SincOraSqlOra";
        public const string Inicio = "start";
        public const string Pagina = "page";
        public const string Tabla = "Tabla";
        public const string Tag = "Tag";
        public const string TieneTodos = "TieneTodos";
        public const string Tipo = "Tipo";
        public const string ToChar = "to_char(";
        public const string ToDate = "to_date(";
        public const string ToTimestamp = "to_timestamp(";
        public const string Traducciones = "Traducciones";
        public const string Traducir = "Traducir";
        public const string Traductor = "Traductor";
        public const string Traductores = "Traductores";
        public const string True = "true";
        public const string Unico = "Unico";
        public const string Uno = "1";
        public const string Update = "update ";
        public const string Usuario = "Usuario";
        public const string ValidarAntesDeEliminar = "ValidarAntesDeEliminar";
        public const string ValidarClave = "ValidarClave";
        public const string ValidarCriterios = "ValidarCriterios";
        public const string ValidarDigitoVerificador = "ValidarDigitoVerificador";
        public const string ValidarExpresionRegular = "ValidarExpresionRegular";
        public const string ValidarExpresionSQL = "ValidarExpresionSQL";
        public const string ValidarFecha = "ValidarFecha";
        public const string ValidarNumerico = "ValidarNumerico";
        public const string ValidarRequerido = "ValidarRequerido";
        public const string ValidarUnico = "ValidarUnico";
        public const string Valor = "Valor";
        public const string ValorJson = "ValorJson";
        public const string ValorXml = "ValorXml";
        public const string Values = " values( ";
        public const string Varchar = "varchar";
        public const string Variable = "Variable";
        public const string Variables = "Variables";
        public const string Where = " where ";
        public const string x2 = "x2";
        public const string YaExiste = "ya existe en la tabla";



    }
    #endregion

    #region Caracteres

    internal struct CaracteresJson
    {
        public const char a = 'a';
        public const char apostrofe = '`';
        public const char b = 'b';
        public const char backSlashB = '\b';
        public const char backSlashBackSlash = '\\';
        public const char backSlashF = '\f';
        public const char backSlashN = '\n';
        public const char backSlashR = '\r';
        public const char backSlashT = '\t';
        public const char braketAbierto = '[';
        public const char braketCerrado = ']';
        public const char cero = '0';
        public const char cinco = '5';
        public const char coma = ',';
        public const char comilla = '\'';
        public const char comillaDoble = '"';
        public const char cuatro = '4';
        public const char dos = '2';
        public const char dosPuntos = ':';
        public const char e = 'e';
        public const char f = 'f';
        public const char guion = '-';
        public const char l = 'l';
        public const char llaveAbierta = '{';
        public const char llaveCerrada = '}';
        public const char n = 'n';
        public const char N = 'N';
        public const char nueve = '9';
        public const char ocho = '8';
        public const char r = 'r';
        public const char s = 's';
        public const char seis = '6';
        public const char siete = '7';
        public const char slash = '/';
        public const char t = 't';
        public const char tres = '3';
        public const char u = 'u';
        public const char uno = '1';
    }
    #endregion


}