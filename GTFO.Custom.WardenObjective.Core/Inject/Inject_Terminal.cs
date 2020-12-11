using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Inject
{
    [HarmonyPatch]
    internal static class Inject_Terminal
    {
        private static Dictionary<string, Action<LG_ComputerTerminal, string, string>> _handlerDict;

        static Inject_Terminal()
        {
            _handlerDict = new Dictionary<string, Action<LG_ComputerTerminal, string, string>>();
        }

        public static void AddCustomCommandHandler(string cmdText, Action<LG_ComputerTerminal, string, string> handler)
        {
            var newCmdText = cmdText.ToLower();

            if(_handlerDict.ContainsKey(newCmdText))
            {
                _handlerDict[newCmdText] += handler;
            }
            else
            {
                _handlerDict.Add(newCmdText, handler);
            }
        }

        public static void UnregisterAll(string cmdText)
        {
            if (_handlerDict.ContainsKey(cmdText))
            {
                _handlerDict[cmdText] = null;
            }
        }

        [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "ReceiveCommand")]
        [HarmonyPostfix]
        static void Post_ReceiveCommand(LG_ComputerTerminalCommandInterpreter __instance, TERM_Command cmd, string inputLine, string param1, string param2)
        {
            var newInputLine = inputLine.Split(' ')[0].ToLower();
            Console.WriteLine("got command");

            //Unused code
            if (cmd == TERM_Command.Override)
            {
                Console.WriteLine("got override command");
                if (_handlerDict.ContainsKey(newInputLine))
                {
                    Console.WriteLine("it's override command {0} {1}", param1, param2);
                    _handlerDict[newInputLine].Invoke(__instance.m_terminal, param1, param2);
                }
            }
        }
    }
}
