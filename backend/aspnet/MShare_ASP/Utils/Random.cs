using System;

namespace MShare_ASP.Utils
{
    /// <summary>Extensions for class 'Random'</summary>
    public static class RandomExtensions
    {

        private const string CHAR_SET = "0123456789qwertzuioplkjhgfdsayxcvbnm-_QWERTZUIOPLKJHGFDSAYXCVBNM";

        /// <summary>Makes a new random string of a given length</summary>
        public static string RandomString(this Random rand, int lenght)
        {
            char[] chars = new char[lenght];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = CHAR_SET[rand.Next(0, CHAR_SET.Length)];
            }
            return new String(chars);
        }
    }
}
