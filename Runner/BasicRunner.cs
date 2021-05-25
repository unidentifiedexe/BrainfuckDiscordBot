#nullable enable
using BrainfuckDiscordBot.InterpreterCompornents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot.Runner
{
    class BasicRunner
    {

        public static Result RunWithMem(string code, string input, TimeSpan timeout)
        {
            using var inputText = new InputText(input);
            var outputText = new OutputText();
            Interpreter interpreter;
            try
            {
                interpreter = new Interpreter(code)
                {
                    TryReadChar = inputText.TryGetNextLetter,
                    WriteChar = outputText.Write,
                };
            }
            catch (Exception exc)
            {
                return new Result(exc.Message, null, 0);
            }
            interpreter.ExecuteNextCode(timeout);

            var ex = interpreter.Exception;
            var output = ex?.Message ?? outputText.GetText();
            var mem = interpreter.EnumerateMemoryState();
            var ptr = interpreter.MemoryPosition;
            return new Result(output, mem, ptr);
        }
    }

    class Result
    {
        public Result(string output, IEnumerable<byte>? memory, int memotyPosition)
        {
            Output = output;
            Memory = memory?.ToArray() ?? new byte[] { };
            MemotyPosition = memotyPosition;
        }

        public string Output { get; }
        public IReadOnlyList<byte> Memory { get; }

        public int MemotyPosition { get; }


        public string GetMemoryString(bool hex)
        {
            var bu = new StringBuilder();
            bu.Append("[");
            for (int i = 0; i < Memory.Count; i++)
            {
                var item = Memory[i];
                if (i != 0)
                    bu.Append(",");
                if (hex)
                    bu.Append($"0x{i:x2}");
                else
                    bu.Append(item);
                if (i == MemotyPosition)
                    bu.Append("*");
            }

            bu.Append("]");
            return bu.ToString();
        }

    }

}
