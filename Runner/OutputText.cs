using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot.Runner
{
    class OutputText
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();


        public void Write(byte letter)
        {
            _stringBuilder.Append((char)letter);
        }


        public string GetText() => _stringBuilder.ToString();

    }
}
