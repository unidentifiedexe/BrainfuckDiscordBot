using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot.InterpreterCompornents
{
    class RunnningCode
    {
        //private readonly string _sourceCode;
        private readonly char[] _trimedSourceCode;

        /// <summary>
        /// 指定位置でのコードの文字を返す。終端の場合は Char.MinValue を返す。
        /// </summary>
        /// <param name="index">トリムされたコードの位置</param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
                if (index >= _trimedSourceCode.Length)
                    return Char.MinValue;
                else
                    return _trimedSourceCode[index];
            }
        }

        /// <summary> ソースコードの長さを返す </summary>
        public int Length => _trimedSourceCode.Length;

        /// <summary> ソースコードの長さを返す </summary>
        public long LongLength => _trimedSourceCode.LongLength;


        /// <summary> 列挙子を取得する </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator() => _trimedSourceCode.AsEnumerable().GetEnumerator();

        /// <summary> 不要な文字が削除されたコードを取得する </summary>
        /// <returns></returns>
        public string GetTrimedCode()
        {
            return new string(_trimedSourceCode);
        }

        /// <summary> ソースコードよりインスタンスを生成する </summary>
        /// <param name="sourceCode"></param>
        public RunnningCode(string sourceCode)
        {
            _trimedSourceCode = EffectiveCharacters.RemoveNonEffectiveChars(sourceCode).ToArray();
        }
    }
}
