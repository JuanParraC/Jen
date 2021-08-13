namespace Jen.Json
{
    public struct JsonToken
    {
        public const int Inicio = 0;
        public const int Blanco = 1;
        public const int LlaveAbrierta = 2;
        public const int LlaveCerrada = 3;
        public const int BraketAbierto = 4;
        public const int BraketCerrado = 5;
        public const int DosPuntos = 6;
        public const int Coma = 7;
        public const int Texto = 8;
        public const int Numero = 9;
        public const int Verdadero = 10;
        public const int Falso = 11;
        public const int Nulo = 12;
        public const int Nan = 13;
    }
}
