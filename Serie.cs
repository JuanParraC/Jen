// Autor : Juan Parra
// 3Soft
namespace Jen
{
    /// <summary>
    /// Serie genera claves de tipo string
    /// </summary>
    public class Serie
    {
        // mascara de 8 bits 11111111
        const byte mBits = (byte)0xff;
        // variable de incremento de la serie
        uint serie = 2;
        // ultimo valor retornado
        string val = string.Empty;
        /// <summary>
        /// Obtiene el valor actual de la serie
        /// </summary>
        /// <returns></returns>
        public string Valor()
        {
            string strVal = string.Empty;
            // obtiene el valor de la serie incrementado
            uint num = ++serie;
            // declara un bite para descomponer el uint de 8  bits por iteracion
            byte vByte;
            // itera sobre el numero descomponiendo de 8 bits mientras sea mayor a cero
            while (num > 0)
            {
                // obtiene los 8 primeros bits
                vByte = (byte)(num & mBits);
                // almacena el caracter
                strVal += ((char)vByte);
                // desplaza 8 bits
                num = num >> 8;
            }
            return (val = strVal);
        }
        public string Proximo()
        {
            string vActual = val;
            string ret = Valor();
            val = vActual;
            serie--;
            return ret;
        }
        public string Actual()
        {
            return val;
        }
    }
}
