#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot.InterpreterCompornents
{
    public class Interpreter
    {
        private RunnningCodePointer _sourceCode;
        private readonly Memory _memory;
        private bool _hasStopReqest = false;

        /// <summary> 実行ステートを取得する </summary>
        public RunnningState State { get; private set; }

        /// <summary> 異常終了したときの例外を取得する。異常終了をしていない場合はnullを返す。 </summary>
        public Exception? Exception { get; private set; }

        public Interpreter(string code)
        {

            _sourceCode = new RunnningCodePointer(code);
            _memory = new Memory();
        }

        ///// <summary> コードを実行する </summary>
        //public void ExecuteNextCode()
        //{
        //    ExecuteNextCode(false);
        //}

        ///// <summary> コードを実行する </summary>
        //public void ExecuteNextStepCode()
        //{
        //    ExecuteNextCode(true);
        //}

        /// <summary> 実行の停止を要求します </summary>
        public void NoticeStopReqest()
        {
            _hasStopReqest = true;
        }
        

        /// <summary>
        /// コードを実行する
        /// </summary>
        /// <param name="isOnlyOneCode">1ステップのみ実行するかどうか</param>
        public void ExecuteNextCode(TimeSpan timeSpan)
        {
            if (State == RunnningState.Finished) return;
            if (State == RunnningState.ErrorStoped) return;
            if (State == RunnningState.Runnning) return;
            _hasStopReqest = false;
            State = RunnningState.Runnning;
            Exception = null;
            var startTime = DateTime.Now;
            try
            {
                var count = 0;
                while (!_hasStopReqest)
                {
                    var letter = _sourceCode.Current;
                    if (letter == char.MaxValue) throw new Exception("コードポインタの位置がおかしい");
                    else if (letter == '+') _memory.CurrentIncrement();
                    else if (letter == '-') _memory.CurrentDecrement();
                    else if (letter == '>') _memory.MoveNext();
                    else if (letter == '<') _memory.MovePrev();
                    else if (letter == '[') { if (_memory.CurrentValue == 0) _sourceCode.Jamp(); }
                    else if (letter == ']') { if (_memory.CurrentValue != 0) _sourceCode.Jamp(); }
                    else if (letter == '.') WhiteCharMethod(_memory.CurrentValue);
                    else if (letter == ',')
                    {
                        if (!ReadCharMethod(out var let)) break;
                        _memory.CurrentValue = let;
                    }

                    if (!_sourceCode.MoveNext()) break;
                    count++;
                    if (count % 500 == 0)
                    {
                        if (DateTime.Now - startTime> timeSpan)
                            throw new TimeoutException();
                        count = 0;
                    }
                }
                if (_sourceCode.IsFinished)
                    State = RunnningState.Finished;
                //else
                //    State = RunnningState.Pause;
            }
            catch (Exception exe)
            {
                Exception = exe;
                State = RunnningState.ErrorStoped;
                Console.WriteLine(exe.Message);
            }
        }


        public delegate bool ReadCharDelegate(out byte letter);
        public delegate void WhiteCharDelegate(byte letter);

        public ReadCharDelegate? TryReadChar { get; set; }
        public WhiteCharDelegate? WriteChar { get; set; }
        public int MemoryPosition => _memory.Position;

        /// <summary> 文字の読み取りが完了した時に発火するイベント </summary>
        public event EventHandler? CharRead;

        /// <summary> 文字の書き出しが完了した時に発火するイベント </summary>
        public event EventHandler? CharWriten;

        private bool ReadCharMethod(out byte letter)
        {
            var tryReadChar = TryReadChar ?? throw new NullReferenceException(nameof(TryReadChar));
            while (!tryReadChar(out letter))
            {
                if (_hasStopReqest)
                    return false;
            }
            CharRead?.Invoke(this, new EventArgs());
            return true;
        }

        private void WhiteCharMethod(byte letter)
        {
            var writeChar = WriteChar;
            if (writeChar != null)
            {
                writeChar(letter);
            }
            else
            {
                Console.Write((char)letter);
            }
            CharWriten?.Invoke(this, new EventArgs());
            return;
        }


        public bool IsStopped()
        {
            return
                State == RunnningState.ErrorStoped ||
                State == RunnningState.Finished;
        }

        public IEnumerable<byte> EnumerateMemoryState()
        {
            return _memory.EnumerateMemoryState();
        }
    }

    public enum RunnningState
    {
        Ready = 0,
        Runnning = 1,
        Finished = 3,
        ErrorStoped,
    }
}
