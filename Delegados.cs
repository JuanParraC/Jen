// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// para invocar a funciones sin argumentos y sin retorno
    /// </summary>
    public delegate void Proc();
    /// <summary>
    /// para invocar a funciones sin retorno con argumento TParam
    /// </summary>
    /// <typeparam name="TParam">Tipo del argmento</typeparam>
    /// <param name="param">parametro de entrada a la funcion</param>
    public delegate void Proc<TParam>(TParam param);
    public delegate void Proc<TParam1, TParam2>(TParam1 param1, TParam2 param2);
    /// <summary>
    /// para invocar funciones con retorno del tipo TRetorno sin argumentos
    /// </summary>
    /// <typeparam name="TRetorno">Tipo devuelto</typeparam>
    /// <returns>Retorna la instancia de TRetorno</returns>
    public delegate TRetorno Func<TRetorno>();
    /// <summary>
    /// para invocar a funciones que retornan un TRetorno y reciben como parametro un TParam
    /// </summary>
    /// <typeparam name="TParam">Tipo de entrada a la funcion</typeparam>
    /// <typeparam name="TRetorno">Tipo de retorno de la funcion</typeparam>
    /// <param name="param">Parametro de entrada</param>
    /// <returns></returns>
    public delegate TRetorno Func<TParam, TRetorno>(TParam param);
    /// <summary>
    /// para invocar funciones con retorno y dos parametros de entrada
    /// </summary>
    /// <typeparam name="TParam1">Tipo de parámetro 1</typeparam>
    /// <typeparam name="TParam2">Tipo de parámetro 2</typeparam>
    /// <typeparam name="TRetorno">Tipo de retorno</typeparam>
    /// <param name="param1">Parámetro 1</param>
    /// <param name="param2">Parámetro 2</param>
    /// <returns></returns>
    public delegate TRetorno Func<TParam1, TParam2, TRetorno>(TParam1 param1, TParam2 param2);
}