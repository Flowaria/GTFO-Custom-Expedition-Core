using System;
using UnityEngine;

namespace CustomObjectives
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

        public GlobalBehaviour(IntPtr intPtr) : base(intPtr)
        {
        }

        protected void Update()
        {
            OnUpdate?.Invoke();
        }

        protected void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }
}