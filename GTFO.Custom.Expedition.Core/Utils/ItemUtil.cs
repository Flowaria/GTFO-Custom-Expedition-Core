using CustomExpeditions.Inject.MarkerItem;
using CustomExpeditions.Messages;
using CustomExpeditions.Utility.Attributes;
using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExpeditions.Utils
{
    using LogList = Il2CppSystem.Collections.Generic.List<TerminalLogFileData>;

    [StaticConstructorAutorun]
    public static class ItemUtil
    {
        private readonly static Dictionary<string, BuilderInfo> _ItemBuilderDict;

        static ItemUtil()
        {
            _ItemBuilderDict = new Dictionary<string, BuilderInfo>();

            GlobalMessage.OnLevelCleanup += () =>
            {
                _ItemBuilderDict.Clear();
            };
        }

        public static BuilderInfo FindInfoByGUID(string guid)
        {
            return _ItemBuilderDict.ContainsKey(guid) ? _ItemBuilderDict[guid] : null;
        }

        public static BuilderInfo FindInfoByItem(LG_DistributeItem item)
        {
            var guid = GetGUID(item);
            if (guid == null)
            {
                return null;
            }

            return _ItemBuilderDict.ContainsKey(guid) ? _ItemBuilderDict[guid] : null;
        }

        public static void RegisterItem(LG_DistributeItem item, string guid = "", Action<GameObject> onSpawn = null)
        {
            ItemUtil.SetGUID(item, guid);

            if(onSpawn != null)
            {
                ItemMessage.OnItemSpawned += (itemGuid, gameObject) =>
                {
                    if (itemGuid != GetGUID(item))
                        return;

                    onSpawn?.Invoke(gameObject);
                };
            }
        }

        public static void RegisterItem(LG_DistributeItem item, Action<GameObject> onSpawn)
        {
            var info = FindInfoByItem(item);
            if (info == null)
            {
                info = new BuilderInfo();

                ItemUtil.SetGUID(item);

                _ItemBuilderDict.Add(ItemUtil.GetGUID(item), info);
            }

            //TODO: Refactor code
            ItemMessage.OnItemSpawned += (guid, gameObject) =>
            {
                if (guid == GetGUID(item))
                {
                    onSpawn?.Invoke(gameObject);
                }
            };
        }

        public static bool HasRegistered(LG_DistributeItem item)
        {
            return FindInfoByItem(item) != null;
        }

        public static bool IsWardenObjectiveItem(LG_DistributeItem item)
        {
            return FindInfoByItem(item)?.IsWardenObjectiveItem ?? false;
        }

        public static bool IsReusableItem(LG_DistributeItem item)
        {
            return !IsWardenObjectiveItem(item);
        }

        public static void SetGUID(LG_DistributeItem item, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
                guid = Guid.NewGuid().ToString();

            if (TryGetGUIDData(item.m_localTerminalLogFiles, out var existingGuid))
            {
                existingGuid.FileContent = guid;
            }
            else
            {
                if (item.m_localTerminalLogFiles == null)
                    item.m_localTerminalLogFiles = new LogList();

                item.m_localTerminalLogFiles.Add(new TerminalLogFileData()
                {
                    FileName = "!PLUGIN_RESERVED_SPECIAL_GUID",
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
                    if (log.FileName.Equals("!PLUGIN_RESERVED_SPECIAL_GUID"))
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