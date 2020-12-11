using GameData;
using GTFO.CustomObjectives.Inject;
using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO.CustomObjectives.Utils
{
    using LogList = Il2CppSystem.Collections.Generic.List<TerminalLogFileData>;

    public static class ItemUtil
    {
        private readonly static Dictionary<string, BuilderInfo> _ItemBuilderDict;

        static ItemUtil()
        {
            _ItemBuilderDict = new Dictionary<string, BuilderInfo>();

            GlobalMessage.OnLevelCleanup += OnLevelCleanup;
        }

        private static void OnLevelCleanup()
        {
            Clear();
        }

        public static void Clear()
        {
            _ItemBuilderDict.Clear();
        }

        public static BuilderInfo FindInfoByGUID(string guid)
        {
            return _ItemBuilderDict.ContainsKey(guid) ? _ItemBuilderDict[guid] : null;
        }

        public static BuilderInfo FindInfoByItem(LG_DistributeItem item)
        {
            var guid = GetGUID(item);
            if(guid == null)
            {
                return null;
            }

            return _ItemBuilderDict.ContainsKey(guid) ? _ItemBuilderDict[guid] : null;
        }

        public static void RegisterItem(LG_DistributeItem item, bool isWardenObj, Action<GameObject> onSpawn)
        {
            var info = FindInfoByItem(item);
            if (info == null)
            {
                info = new BuilderInfo();
                info.OnGameObjectSpawned += onSpawn;
                info.IsWardenObjectiveItem = isWardenObj;

                ItemUtil.SetGUID(item);

                _ItemBuilderDict.Add(ItemUtil.GetGUID(item), info);
            }
        }

        public static bool HasRegistered(LG_DistributeItem item)
        {
            return FindInfoByItem(item) != null;
        }

        public static bool IsWardenObjectiveItem(LG_DistributeItem item)
        {
            return FindInfoByItem(item)?.IsWardenObjectiveItem ?? false;
        }

        public static void SetGUID(LG_DistributeItem item, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
                guid = Guid.NewGuid().ToString();

            if(TryGetGUIDData(item.m_localTerminalLogFiles, out var existingGuid))
            {
                existingGuid.FileContent = guid;
            }
            else
            {
                item.m_localTerminalLogFiles = new LogList();
                item.m_localTerminalLogFiles.Add(new TerminalLogFileData()
                {
                    FileName = "!PLUGIN_REVERVED_SPECIAL_GUID",
                    FileContent = guid
                });
            }
        }

        public static string GetGUID(LG_DistributeItem item)
        {
            return GetGUID(item.m_localTerminalLogFiles);
        }

        public static string GetGUID(LogList logs)
        {
            if (TryGetGUIDData(logs, out var guid))
            {
                return guid.FileContent;
            }

            return null;
        }

        private static bool TryGetGUIDData(LogList logs, out TerminalLogFileData guidData)
        {
            if (logs != null)
            {
                foreach (var log in logs)
                {
                    if(log.FileName.Equals("!PLUGIN_REVERVED_SPECIAL_GUID"))
                    {
                        guidData = log;
                        return true;
                    }
                }
            }
            guidData = null;
            return false;
        }
    }
}
