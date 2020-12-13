using System;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject
{
    public class BuilderInfo
    {
        public bool IsWardenObjectiveItem { get; internal set; }
        public Action<GameObject> OnGameObjectSpawned { get; internal set; }
    }
}