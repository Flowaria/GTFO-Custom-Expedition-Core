using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.CustomReplicators
{
    public class ValueWrapper_32
    {
        public int IntValue
        {
            get;
            set;
        }

        public float FloatValue
        {
            get => BitConverter.ToSingle(Bytes, 0);
            set { Bytes = BitConverter.GetBytes(value); }
        }

        public uint UIntValue
        {
            get => BitConverter.ToUInt32(Bytes, 0);
            set { Bytes = BitConverter.GetBytes(value); }
        }

        //Length 4
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(IntValue);
            set { IntValue = BitConverter.ToInt32(value, 0); }
        }
    }
}
