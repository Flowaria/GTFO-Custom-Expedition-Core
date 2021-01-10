using CustomExpeditions.CustomReplicators;
using HarmonyLib;
using Il2CppSystem.Runtime.InteropServices;
using LevelGeneration;
using SNetwork;
using System;
using System.Reflection;
using UnhollowerBaseLib;

namespace CustomExpeditions.Inject.CustomReplicators
{
    //[HarmonyPatch(typeof(SNet_Replication))]
    static class Inject_Replication
    {
        internal static void MakePatch(Harmony harmonyInstance)
        {
            var pMethodBinding = BindingFlags.NonPublic | BindingFlags.Static;
            var pMethodType = typeof(Inject_Replication);
            var pMethodName = nameof(Inject_Replication.Post_TryGetReplicator);
            var pMethod = pMethodType.GetMethod(pMethodName, pMethodBinding);
            var harmonyMethod = new HarmonyMethod(pMethod);

            var tMethodBinding = BindingFlags.Public | BindingFlags.Instance;
            var tMethodType = typeof(SNet_Replication);
            var tMethodName = nameof(SNet_Replication.TryGetReplicator);
            var tMethod = tMethodType.GetMethod(tMethodName, tMethodBinding, null, new Type[]
            {
                typeof(Il2CppStructArray<byte>),
                typeof(IReplicator).MakeByRefType(),
                typeof(int).MakeByRefType()
            }, null);

            harmonyInstance.Patch(tMethod, postfix: harmonyMethod);
        }

        static SNet_Marshaller<pDoorState> Marshaller;
        static Inject_Replication()
        {
            SNet_Marshal.Setup();
            Marshaller = new SNet_Marshaller<pDoorState>();
            Marshaller.Setup(Marshal.SizeOf<pDoorState>());
        }
        
        private static void Post_TryGetReplicator(ref bool __result, Il2CppStructArray<byte> bytes, ref IReplicator id, ref int packetIndex)
        {
            var pIndex = bytes[2];
            if(pIndex >= CustomReplicatorProvider.PACKET_OFFSET && Marshaller.SizeWithIDs == bytes.Length) //It's custom Replicator
            {
                //Read Replicator Index
                Buffer.BlockCopy(bytes, 0, SNet_Replication.m_ushortConverter, 0, 2);
                int replicatorIndex = SNet_Replication.m_ushortConverter[0];

                packetIndex = pIndex;

                __result = CustomReplicatorManager.TryGetReplicator(replicatorIndex, out id);
            }
        }
    }
}