#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot.Runner
{
    class InputText : IDisposable
    {

        private byte _eof = 0xff;

        private readonly IEnumerator<char> _e;

        public InputText(string? str)
        {
            str ??= string.Empty;
            _e = str.GetEnumerator();
        }

        public void Dispose()
        {
            _e.Dispose();
        }

        public bool TryGetNextLetter(out byte letter)
        {
            letter = _e.MoveNext() ? (byte)_e.Current : _eof;
            return true;
        }

    }
}
