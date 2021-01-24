using SNetwork;
using System;

namespace CustomExpeditions.Messages
{
    public static class GlobalMessage
    {
        static GlobalMessage()
        {
            OnLevelCleanup += () =>
            {
                OnUpdate_Level = null;
                OnFixedUpdate_Level = null;
            };
        }

        public static Action OnGameInit;

        public static Action OnUpdate, OnUpdate_Level;
        public static Action OnFixedUpdate, OnFixedUpdate_Level;

        public static Action OnBuildDone;
        public static Action OnBuildDoneLate;

        public static Action OnElevatorArrive;

        public static Action OnLevelSuccess;
        public static Action OnLevelFail;
        public static Action OnLevelCleanup;

        public static Action OnResetSession;

        public static Action<pWardenObjectiveState, pWardenObjectiveState, bool> OnObjectiveStateChanged;
    }
}