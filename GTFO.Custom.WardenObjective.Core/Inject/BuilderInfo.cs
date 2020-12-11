using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.Inject
{
    public class BuilderInfo
    {
        public bool IsWardenObjectiveItem { get; internal set; }
        public Action<GameObject> OnGameObjectSpawned { get; internal set; }
    }
}
