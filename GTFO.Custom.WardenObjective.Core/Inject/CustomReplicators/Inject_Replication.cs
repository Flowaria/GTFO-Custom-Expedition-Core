using HarmonyLib;
using SNetwork;
using System;
using System.Reflection;
using UnhollowerBaseLib;

namespace CustomExpeditions.Inject.CustomReplicators
{
    //[HarmonyPatch(typeof(SNet_Replication))]
    internal static class Inject_Replication
    {
        [Obsolete]
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

        private static void Post_TryGetReplicator(ref bool __result, Il2CppStructArray<byte> bytes, ref IReplicator id, ref int packetIndex)
        {
            Buffer.BlockCopy(bytes, 0, SNet_Replication.m_ushortConverter, 0, 2);
            int num = SNet_Replication.m_ushortConverter[0];

            Logger.Log("{0} {1} {2} {3} {4}", bytes, id, packetIndex, __result, num);
        }
    }
}