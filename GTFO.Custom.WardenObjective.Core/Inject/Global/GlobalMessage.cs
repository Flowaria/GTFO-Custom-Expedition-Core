using System;

namespace GTFO.CustomObjectives.Inject.Global
{
    public static class GlobalMessage
    {
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

        public static Action<pWardenObjectiveState, pWardenObjectiveState, bool> OnObjectiveStateChanged;
    }
}