using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GTFO.CustomObjectives
{
    public class GlobalBehaviour : MonoBehaviour
    {
        public static void Setup()
        {
            var initObject = new GameObject();
            DontDestroyOnLoad(initObject);
            initObject.AddComponent<GlobalBehaviour>();
        }

        public static Action OnUpdate;
        public static Action OnFixedUpdate;

        public GlobalBehaviour(IntPtr intPtr) : base(intPtr) { }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }
}
