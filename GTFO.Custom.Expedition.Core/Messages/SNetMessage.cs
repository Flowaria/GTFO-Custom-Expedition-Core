using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Messages
{
    public static class SNetMessage
    {
        static SNetMessage()
        {
            GlobalMessage.OnResetSession += () =>
            {
                OnResetSession_SNet = null;
                OnValidateMasterData = null;
                OnPlayerJoinedSession = null;
                OnPlayerLeftSession = null;
                OnPlayerGameStateChanged = null;
                OnPlayerEnterInLevel = null;

                if(!SNet.IsInLobby)
                    return;

                OnPlayerGameStateChanged += (player, state) =>
                {
                    switch (state)
                    {
                        case eGameStateName.InLevel:
                            OnPlayerEnterInLevel?.Invoke(player);
                            break;

                        case eGameStateName.Generating:
                            OnPlayerEnterGenerating?.Invoke(player);
                            break;
                    }
                };
            };
        }

        public static Action OnResetSession_SNet;
        public static Action OnValidateMasterData;

        public static Action<SNet_Player> OnPlayerJoinedSession;
        public static Action<SNet_Player> OnPlayerLeftSession;

        public static Action<SNet_Player, eGameStateName> OnPlayerGameStateChanged;
        public static Action<SNet_Player> OnPlayerEnterInLevel;
        public static Action<SNet_Player> OnPlayerEnterGenerating;
    }
}
