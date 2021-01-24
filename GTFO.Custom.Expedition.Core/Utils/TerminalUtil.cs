using CustomExpeditions.Messages;
using LevelGeneration;
using System;
using System.Collections.Generic;

namespace CustomExpeditions.Utils
{
    public static class TerminalUtil
    {
        private static readonly Dictionary<int, int> _CommandCustomIDDict;

        static TerminalUtil()
        {
            _CommandCustomIDDict = new Dictionary<int, int>();

            GlobalMessage.OnLevelCleanup += () =>
            {
                _CommandCustomIDDict.Clear();
            };
        }

        public static void AddCommand(LG_ComputerTerminal terminal, string cmdText, string helpText, Action<LG_ComputerTerminal, string, string> onCmdReceived)
        {
            if (terminal == null)
                return;

            if (terminal.m_command.m_commandsPerString.ContainsKey(cmdText.ToLower()))
                return;

            TerminalMessage.OnRecievedCustomCmd += (LG_ComputerTerminal eTerminal, string cmd, string arg1, string arg2) =>
            {
                if (!IsSame(terminal, eTerminal))
                    return;

                if (cmdText.Equals(cmd, StringComparison.OrdinalIgnoreCase))
                {
                    onCmdReceived?.Invoke(eTerminal, arg1, arg2);
                }
            };

            var id = terminal.GetInstanceID();
            var newCmdId = 0;
            if (_CommandCustomIDDict.ContainsKey(id))
            {
                newCmdId = ++_CommandCustomIDDict[id];
            }
            else
            {
                newCmdId = 100000;
                _CommandCustomIDDict.Add(id, newCmdId);
            }

            terminal.m_command.AddCommand((TERM_Command)newCmdId, cmdText, helpText);
        }

        public static void AddStandardCommand(LG_ComputerTerminal terminal, string cmdText, string helpText, TERM_Command type)
        {
            terminal.m_command.AddCommand(type, cmdText, helpText);
        }

        public static void RegisterEnterEvent(LG_ComputerTerminal terminal, Action onEnter)
        {
            TerminalMessage.OnProximityEnter += (LG_ComputerTerminal eTerminal) =>
            {
                if (!IsSame(terminal, eTerminal))
                    return;

                onEnter?.Invoke();
            };
        }

        public static void RegisterEnterEvent(LG_ComputerTerminal terminal, Action<LG_ComputerTerminal> onEnter)
        {
            TerminalMessage.OnProximityEnter += (LG_ComputerTerminal eTerminal) =>
            {
                if (!IsSame(terminal, eTerminal))
                    return;

                onEnter?.Invoke(eTerminal);
            };
        }

        public static void RegisterExitEvent(LG_ComputerTerminal terminal, Action onExit)
        {
            TerminalMessage.OnProximityExit += (LG_ComputerTerminal eTerminal) =>
            {
                if (!IsSame(terminal, eTerminal))
                    return;

                onExit?.Invoke();
            };
        }

        public static void RegisterExitEvent(LG_ComputerTerminal terminal, Action<LG_ComputerTerminal> onExit)
        {
            TerminalMessage.OnProximityExit += (LG_ComputerTerminal eTerminal) =>
            {
                if (!IsSame(terminal, eTerminal))
                    return;

                onExit?.Invoke(eTerminal);
            };
        }

        private static bool IsSame(LG_ComputerTerminal a, LG_ComputerTerminal b)
        {
            return a.GetInstanceID() == b.GetInstanceID();
        }
    }
}