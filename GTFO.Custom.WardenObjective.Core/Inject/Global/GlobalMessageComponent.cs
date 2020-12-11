using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject.Global
{
    internal class GlobalMessageComponent : MonoBehaviour
    {
        internal void Awake()
        {
            
        }

        internal void Update()
        {
            GlobalMessage.OnUpdate?.Invoke();
        }

        internal void FixedUpdate()
        {
            GlobalMessage.OnFixedUpdate?.Invoke();
        }
    }
}
