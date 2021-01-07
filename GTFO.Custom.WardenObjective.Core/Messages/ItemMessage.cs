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
