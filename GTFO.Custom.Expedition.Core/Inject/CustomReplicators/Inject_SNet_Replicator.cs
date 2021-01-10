using CustomExpeditions.CustomReplicators;
using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditions.Inject.CustomReplicators
{
    [HarmonyPatch(typeof(SNet_Replicator))]
    class Inject_SNet_Replicator
    {
        [HarmonyPrefix]
        [HarmonyPatch("RevieveBytes")] //Not a Typo, Trust me
        internal static void Pre_RecieveBytes(ref int packetIndex, byte[] bytes)
        {
            if (packetIndex >= CustomReplicatorProvider.PACKET_OFFSET)
            {
                packetIndex -= CustomReplicatorProvider.PACKET_OFFSET;
            }
        }
    }
}
