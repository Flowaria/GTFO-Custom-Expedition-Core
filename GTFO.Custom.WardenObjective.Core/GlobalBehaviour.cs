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
            GameObject.DontDestroyOnLoad(initObject);
            initObject.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<GlobalBehaviour>());
        }

        public static Action OnUpdate;
        public static Action OnFixedUpdate;
        public static Action OnGameInit;

        public GlobalBehaviour(IntPtr intPtr) : base(intPtr) { }

        private void Awake()
        {
            AssetShards.AssetShardManager.add_OnStartupAssetsLoaded((Il2CppSystem.Action)GameLoaded);
        }

        private void GameLoaded()
        {
            OnGameInit?.Invoke();
        }

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
