using LevelGeneration;
using System;

namespace CustomObjectives.Messages
{
    public static class TerminalMessage
    {
        static TerminalMessage()
        {
            //Clear All Registered Event on LevelCleanup
            GlobalMessage.OnLevelCleanup += () =>
            {
                OnReceivedCmd = null;
                OnRecievedCustomCmd = null;
                OnProximityEnter = null;
                OnProximityExit = null;
            };
        }

        /// <summary>
        /// TERM_Command    CommandType
        /// string  Parameter1
        /// string  Parameter2
        /// </summary>
        public static Action<LG_ComputerTerminal, TERM_Command, string, string> OnReceivedCmd;

        /// <summary>
        /// string  CommandString
        /// string  Parameter1
        /// string  Parameter2
        /// </summary>
        public static Action<LG_ComputerTerminal, string, string, string> OnRecievedCustomCmd;

        public static Action<LG_ComputerTerminal> OnProximityEnter;
        public static Action<LG_ComputerTerminal> OnProximityExit;
    }
}