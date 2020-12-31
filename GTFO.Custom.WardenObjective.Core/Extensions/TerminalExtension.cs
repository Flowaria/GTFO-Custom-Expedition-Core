using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives.Extensions
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
