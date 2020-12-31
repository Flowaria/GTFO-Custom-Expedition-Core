using GTFO.CustomObjectives.Inject.Terminal;
using LevelGeneration;
using System;

namespace GTFO.CustomObjectives.Utils
{
    public static class TerminalUtil
    {
        public static void AddCommand(LG_ComputerTerminal terminal, string cmdText, string helpText, Action<LG_ComputerTerminal, string, string> onCmdReceived)
        {
            TerminalMessage.OnRecievedCustomCmd += (LG_ComputerTerminal eTerminal, string cmd, string arg1, string arg2) =>
            {
                if (!IsSame(terminal, eTerminal))
                    return;

                if (cmdText.Equals(cmd, StringComparison.OrdinalIgnoreCase))
                {
                    onCmdReceived?.Invoke(eTerminal, arg1, arg2);
                }
            };

            terminal.m_command.AddCommand(TERM_Command.Override, cmdText, helpText);
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