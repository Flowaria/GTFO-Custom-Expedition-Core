using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomExpeditions.Messages
{
    public static class ItemMessage
    {
        static ItemMessage()
        {
            GlobalMessage.OnLevelCleanup += () =>
            {
                OnStandardItemSpawned = null;
                OnPickupItemSpawned = null;
                OnGateKeyItemSpawned = null;
                OnResourceContainerSpawned = null;
                OnItemSpawned = null;
            };
        }

        /// <summary>
        /// string      GUID
        /// GameObject  GameObject Instance
        /// </summary>
        public static Action<string, GameObject> OnStandardItemSpawned;

        /// <summary>
        /// string      GUID
        /// GameObject  GameObject Instance
        /// </summary>
        public static Action<string, GameObject> OnPickupItemSpawned;

        /// <summary>
        /// string      GUID
        /// GameObject  GameObject Instance
        /// </summary>
        public static Action<string, GameObject> OnGateKeyItemSpawned;

        /// <summary>
        /// string      GUID
        /// GameObject  GameObject Instance
        /// </summary>
        public static Action<string, GameObject> OnResourceContainerSpawned;

        /// <summary>
        /// string      GUID
        /// GameObject  GameObject Instance
        /// </summary>
        public static Action<string, GameObject> OnItemSpawned;
    }
}
