using GTFO.CustomObjectives.Inject.Global;
using Harmony;
using LevelGeneration;
using System;
using System.Collections.Generic;

namespace GTFO.CustomObjectives.Inject
{
    [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "ReceiveCommand")]
    internal static class Inject_Terminal
    {
        private static Dictionary<string, Action<LG_ComputerTerminal, string, string>> _handlerDict;

        internal static void Postfix(LG_ComputerTerminalCommandInterpreter __instance, TERM_Command cmd, string inputLine, string param1, string param2)
        {
            var newInputLine = inputLine.Split(' ')[0].ToLower();

            //Unused code
            if (cmd != TERM_Command.Override)
                return;

            if (_handlerDict.TryGetValue(newInputLine, out var handler))
            {
                handler.Invoke(__instance.m_terminal, param1, param2);
            }
        }

        static Inject_Terminal()
        {
            _handlerDict = new Dictionary<string, Action<LG_ComputerTerminal, string, string>>();

            GlobalMessage.OnLevelCleanup += () =>
            {
                _handlerDict.Clear();
            };
        }

        public static void AddCustomCommandHandler(string cmdText, Action<LG_ComputerTerminal, string, string> handler)
        {
            var newCmdText = cmdText.ToLower();

            if (_handlerDict.ContainsKey(newCmdText))
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
    }
}