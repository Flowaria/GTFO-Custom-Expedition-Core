using LevelGeneration;
using System;

namespace GTFO.CustomObjectives.Inject.CustomReplicators
{
    public abstract class StateWrapperBase
    {
        public abstract pDoorState ToOriginal();

        public abstract void FromOriginal(pDoorState state);

        public int FloatByteToInt(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }

        public float IntByteToFloat(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}