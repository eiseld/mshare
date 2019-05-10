using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Utils {
    /// <summary>
    /// Extensions for class 'Random'
    /// </summary>
    public static class RandomExtensions {
        const string CHAR_SET = "0123456789qwertzuioplkjhgfdsayxcvbnm-_QWERTZUIOPLKJHGFDSAYXCVBNM";

        /// <summary>
        /// Makes a new random string of a given length
        /// </summary>
        /// <param name="r">Random extension parameters</param>
        /// <param name="len">Length of the string</param>
        public static string RandomString(this Random r, int len) {
            char[] chars = new char[len];
            for (int i = 0; i < chars.Length; i++) {
                chars[i] = CHAR_SET[r.Next(0, CHAR_SET.Length)];
            }
            return new String(chars);
        }
    }
}
