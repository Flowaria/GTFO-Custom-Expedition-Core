﻿using CustomExpeditions.Messages;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditions.Messages.Inject.Terminal
{
    [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter))]
    internal class Inject_TerminalCommandInterpreter
    {
        [HarmonyPostfix]
        [HarmonyPatch("ReceiveCommand")]
        internal static void Post_ReceiveCommand(LG_ComputerTerminalCommandInterpreter __instance, TERM_Command cmd, string inputLine, string param1, string param2)
        {
            if ((int)cmd >= 100000)
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

    [HarmonyPatch(typeof(LG_ComputerTerminal))]
    internal class Inject_Terminal
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnProximityEnter")]
        internal static void Post_ProxEnter(LG_ComputerTerminal __instance)
        {
            TerminalMessage.OnProximityEnter?.Invoke(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnProximityExit")]
        internal static void Postfix(LG_ComputerTerminal __instance)
        {
            TerminalMessage.OnProximityExit?.Invoke(__instance);
        }
    }
}