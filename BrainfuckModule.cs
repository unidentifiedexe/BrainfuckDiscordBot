
using BrainfuckDiscordBot.Runner;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckDiscordBot
{
    public class BrainfuckModule
    {
        const string runCommandToken = "/bf";

        public static async Task<(bool isInterpretable, string reply)> ReciveCommandAsync(string command)
        {
            return await Task.Factory.StartNew(() =>
            {
                var check = ReciveCommand(command, out var replay);
                if (string.IsNullOrEmpty(replay))
                    replay = "Output is empty.";
                return (check, replay);
            });
        }

        public static bool ReciveCommand(string command, out string replay)
        {
            replay = default;
            if (command.StartsWith(runCommandToken))
            {
                var args = EnumerateAgs(command);
                if (args.Contains(RunArgumentFlags.Help))
                {
                    replay = "helpは未実装だよ。";
                    return true;
                }

                if (args.Contains(RunArgumentFlags.WithMemory))
                {
                    replay = RunMem(command, null, false);
                }
                else if (args.Contains(RunArgumentFlags.WithHexMemory))
                {
                    replay = RunMem(command, null, true);
                }
                else
                {
                    replay = Run(command, null);
                }
                return true;
            }
            else if (HelperModule.ReciveCommand(command, out replay))
            {
                return true;
            }
            return false;
        }

        private static string Run(string code, string input)
        {
            return BasicRunner.RunWithMem(code, input, new TimeSpan(0, 0, 1)).Output;
        }
        private static string RunMem(string code, string input, bool byhex)
        {
            var ret = BasicRunner.RunWithMem(code, input, new TimeSpan(0, 0, 1));
            return $"memory = {ret.GetMemoryString(byhex)}" + Environment.NewLine + ret.Output;
        }

        private static ISet< RunArgumentFlags> EnumerateAgs(string command)
        {
            var splited = command.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);


            var set = new HashSet<RunArgumentFlags>(splited.Skip(1).Select(ConvertToEnum).TakeWhile(p => p != RunArgumentFlags.None));
            return set;

        }

        private static RunArgumentFlags ConvertToEnum(string command)
        {
            return command.ToLower() switch {
                "help" => RunArgumentFlags.Help,
                "mem" => RunArgumentFlags.WithMemory,
                "hmem" => RunArgumentFlags.WithHexMemory,
                _ => RunArgumentFlags.None,
            };
        }
    }

    public class HelperModule
    {
        const string _getCodeCommand = "/code";
        public static bool ReciveCommand(string command, out string replay)
        {
            if (command.StartsWith(_getCodeCommand))
            {
                const int max = 30;
                var comLen = _getCodeCommand.Length;
                if (command.Length > comLen + 1 && command[comLen] == ' ')
                {
                    var substr = command.Substring(comLen + 1);
                    var bu = new StringBuilder();
                    foreach (var item in substr.Take(max))
                    {
                        bu.AppendLine($"{item} = {(int)item} (0x{(int)item:x2})");
                    }
                    if(substr.Length > max)
                        bu.AppendLine($"too meny");

                    replay = bu.ToString();
                    return true;
                }
                replay = default;

                return false;
            }
            replay = default;

            return false;
        }
    }


    public enum RunArgumentFlags
    {
        None,
        Help,
        WithMemory,
        WithHexMemory,
    }
}
