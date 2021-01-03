using LevelGeneration;
using System;

namespace CustomObjectives.Extensions
{
    public static class TerminalExtension
    {
        public static void SetEndOfQueue(this LG_ComputerTerminal terminal, Action endOfQueue)
        {
            SetEndOfQueue(terminal.m_command, endOfQueue);
        }

        public static void SetEndOfQueue(this LG_ComputerTerminalCommandInterpreter cmd, Action endOfQueue)
        {
            cmd.OnEndOfQueue = new Action(endOfQueue);
        }
    }
}