using SNetwork;
using System;

namespace CustomExpeditions.Messages
{
    public static class GlobalMessage
    {
        public static Action OnGameInit;

        public static Action OnUpdate;
        public static Action OnFixedUpdate;

        public static Action OnBuildDone;
        public static Action OnElevatorArrive;

        public static Action OnLevelSuccess;
        public static Action OnLevelFail;
        public static Action OnLevelCleanup;

        public static Action OnResetSession;

        public static Action<pWardenObjectiveState, pWardenObjectiveState, bool> OnObjectiveStateChanged;
    }
}