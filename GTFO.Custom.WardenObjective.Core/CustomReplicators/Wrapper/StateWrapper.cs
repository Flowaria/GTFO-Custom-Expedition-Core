using LevelGeneration;
using System;

namespace CustomExpeditions.CustomReplicators
{
    public class StateWrapper
    {
        public byte Status;

        public readonly ValueWrapper_32 Value1 = new ValueWrapper_32();
        public readonly ValueWrapper_32 Value2 = new ValueWrapper_32();
        public readonly ValueWrapper_32 Value3 = new ValueWrapper_32();

        public bool Flag1;
        public bool Flag2;
        public bool Flag3;
        public bool Flag4;
        public bool Flag5;

        public pDoorState ToOriginal()
        {
            return new pDoorState()
            {
                animProgress = Value1.FloatValue,
                damageTaken = Value2.FloatValue,
                glueRel = Value3.FloatValue,

                hasBeenOpenedDuringGame = Flag1,
                markedOnMap = Flag2,
                sourcePosZ = Flag3,
                triggerTryOpenBroken = Flag4,
                triggerTryOpenStuckInGlue = Flag5,

                status = (eDoorStatus)Status
            };
        }

        public void FromOriginal(pDoorState state)
        {
            Value1.FloatValue = state.animProgress;
            Value2.FloatValue = state.damageTaken;
            Value3.FloatValue = state.glueRel;

            Flag1 = state.hasBeenOpenedDuringGame;
            Flag2 = state.markedOnMap;
            Flag3 = state.sourcePosZ;
            Flag4 = state.triggerTryOpenBroken;
            Flag5 = state.triggerTryOpenStuckInGlue;

            Status = (byte)state.status;
        }
    }
}