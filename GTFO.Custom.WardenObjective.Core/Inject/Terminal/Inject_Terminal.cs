using GTFO.CustomObjectives.Inject.Global;
using Harmony;
using LevelGeneration;
using System;
using System.Collections.Generic;

namespace GTFO.CustomObjectives.Inject.Terminal
{
    [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "ReceiveCommand")]
    internal static class Inject_Terminal_ReceiveCommand
    {
        internal static void Postfix(LG_ComputerTerminalCommandInterpreter __instance, TERM_Command cmd, string inputLine, string param1, string param2)
        {
            if (cmd == TERM_Command.Override)
            {
                var cmdStr = inputLine.Split(' ')[0].ToLower();
                TerminalMessage.OnRecievedCustomCmd?.Invoke(__instance.m_terminal, cmdStr, param1, param2);
            }
            else
            {
                TerminalMessage.OnReceivedCmd?.Invoke(__instance.m_terminal, cmd, param1, param2);
            }
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), "OnProximityEnter")]
    internal static class Inject_Terminal_Enter
    {
        internal static void Postfix(LG_ComputerTerminal __instance)
        {
            TerminalMessage.OnProximityEnter?.Invoke(__instance);
        }
    }

    [HarmonyPatch(typeof(LG_ComputerTerminal), "OnProximityExit")]
    internal static class Inject_Terminal_Exit
    {
        internal static void Postfix(LG_ComputerTerminal __instance)
        {
            TerminalMessage.OnProximityExit?.Invoke(__instance);
        }
    }
}