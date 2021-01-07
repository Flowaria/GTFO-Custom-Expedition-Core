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
        public static Action OnResetSession_SNet;
        public static Action OnValidateMasterData;

        public static Action<SNet_Player> OnPlayerJoinedSession;
        public static Action<SNet_Player> OnPlayerLeftSession;

        public static Action<pWardenObjectiveState, pWardenObjectiveState, bool> OnObjectiveStateChanged;
    }
}