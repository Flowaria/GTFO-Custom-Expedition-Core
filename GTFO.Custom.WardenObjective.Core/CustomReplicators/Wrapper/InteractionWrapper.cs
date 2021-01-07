using LevelGeneration;
using SNetwork;
using UnityEngine;
using static SNetwork.SNetStructs;

namespace CustomExpeditions.CustomReplicators
{
    public class InteractionWrapper
    {
        public byte Type;

        public readonly ValueWrapper_32 Value1 = new ValueWrapper_32();
        public readonly ValueWrapper_32 Value2 = new ValueWrapper_32();
        public readonly ValueWrapper_32 Value3 = new ValueWrapper_32();
        public readonly ValueWrapper_32 Value4 = new ValueWrapper_32();
        public readonly ValueWrapper_32 Value5 = new ValueWrapper_32();

        public bool Flag1;

        public ulong UserID;

        public SNet_Player Player
        {
            get
            {
                SNet.Core.TryGetPlayer(UserID, out var player, false);
                return player;
            }
            set
            {
                UserID = value.Lookup;
            }
        }

        public pDoorInteraction ToOriginal()
        {
            return new pDoorInteraction()
            {
                type = (eDoorInteractionType)Type,
                val1 = Value1.FloatValue,
                val2 = Value2.FloatValue,
                position = new Vector3()
                {
                    x = Value3.FloatValue,
                    y = Value4.FloatValue,
                    z = Value5.FloatValue
                },
                isPlayerIntertaction = Flag1,
                user = new pPlayer()
                {
                    lookup = UserID
                }
            };
        }

        public void FromOriginal(pDoorInteraction i)
        {
            Type = (byte)i.type;
            Value1.FloatValue = i.val1;
            Value2.FloatValue = i.val2;
            Value3.FloatValue = i.position.x;
            Value4.FloatValue = i.position.y;
            Value5.FloatValue = i.position.z;
            Flag1 = i.isPlayerIntertaction;
            UserID = i.user.lookup;
        }
    }
}