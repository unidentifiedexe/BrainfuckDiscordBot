using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot.InterpreterCompornents
{
    public static class EffectiveCharacters
    {

        static public HashSet<char> Characters { get; } = new HashSet<char>("+-><.,[]");


        /// <summary>
        /// ソースコードの不要な文字を削除したものを返す
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IEnumerable<char> RemoveNonEffectiveChars(string code)
        {
            var ret = code.Where(p => Characters.Contains(p));

            return ret;
        }

        public static bool IsEffectiveChar(char letter)
        {
            return Characters.Contains(letter);
        }

    }
}
