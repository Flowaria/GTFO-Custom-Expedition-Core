using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using GTFO.CustomObjectives;
using GTFO.CustomObjectives.GlobalHandlers.TimedObjectives;
using GTFO.CustomObjectives.Inject.CustomReplicators;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.SimpleLoader;
using GTFO.CustomObjectives.Utils;
using HarmonyLib;
using SNetwork;
using System;
using System.Linq;
using System.Reflection;
using TestPlugin.ShuffleSeed;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GTFO.CustomObjectives
{
    [BepInPlugin("Custom-WardenObjective-Core", "Custom WardenObjective Core", "1.0.0.0")]
    internal class Entry : BasePlugin
    {
        private const string CONFIG_SECTION = "Global";

        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<GlobalBehaviour>();

            Setup_DefaultConfigs();
            Setup_DefaultGlobalHandlers();

            //Setup Harmony Patcher
            var harmonyInstance = new Harmony("flowaria.CustomWObjective.Core.Harmony");
            harmonyInstance.PatchAll();

            Inject_Replication.MakePatch(harmonyInstance);

            //Setup Simple Loader
            ObjectiveSimpleLoader.Setup();

            AssetShards.AssetShardManager.add_OnStartupAssetsLoaded((Il2CppSystem.Action)OnGameInit);
        }

        #region ApplicationStart

        private void Setup_DefaultConfigs()
        {
            Logger.LogInstance = this.Log;
            Logger.IsGlobalEnabled = GetCfgValue("UseLogger", true, "Enable Logger?");

            var raw = GetCfgValue("UsingLogLevels", "Log, Warning, Error", "Allowed Level for Logger ('Verbose', 'Log', 'Warning' and 'Error')");
            var list = raw.Split(',').Select(item => item.Trim());
            Logger.IsVerboseEnabled =   list.Any(x => x.Equals("Verbose", StringComparison.OrdinalIgnoreCase));
            Logger.IsLogEnabled =       list.Any(x => x.Equals("Log", StringComparison.OrdinalIgnoreCase));
            Logger.IsWarnEnabled =      list.Any(x => x.Equals("Warning", StringComparison.OrdinalIgnoreCase));
            Logger.IsErrorEnabled =     list.Any(x => x.Equals("Error", StringComparison.OrdinalIgnoreCase));

            Config.Save();
        }

        private T GetCfgValue<T>(string entry, T defaultValue, string description)
        {
            return Config.Bind<T>(new ConfigDefinition(CONFIG_SECTION, entry), defaultValue, new ConfigDescription(description)).Value;
        }

        private void Setup_DefaultGlobalHandlers()
        {
            CustomObjectiveManager.AddGlobalHandler<TimedObjectiveHandler>("TimedObjective", CustomObjectiveSettings.MAIN_ONLY);
            //CustomObjectiveManager.AddGlobalHandler<FogControlTerminalHandler>(); later
            //TODO: New Global Handler: Change Light Settings (color / type / or whatever)
            //TODO: New Global Handler: Change Alarm Settings for existing door
            //TODO: New Global Handler: Regen Hibernating Enemies on other zone
        }

        #endregion

        public void OnGameInit()
        {
            //Setup Global Handler
            GlobalBehaviour.Setup();
            GlobalBehaviour.OnUpdate += OnUpdate;
            GlobalBehaviour.OnFixedUpdate += OnFixedUpdate;

            Setup_LocalConfigs();
            Setup_DefaultReplicator();
        }

        #region GameInit

        private void Setup_LocalConfigs()
        {
            ConfigUtil.SetupLocalConfig(Config);

            if (!ConfigUtil.HasLocalPath)
                return;

            if (ConfigUtil.TryGetLocalConfig<LocalConfigDTO>("ObjectivePrefs.json", out var config))
            {
                Logger.Log("=== Global Handler Whitelist ===");
                foreach (var p in config.EnabledModules)
                {
                    Logger.Log("Allowed Plugin: {0}", p);
                }
                CustomObjectiveManager.SetGlobalHandlerWhitelist(config.EnabledModules);
            }
        }

        ShuffleSeedReplicator Replicator;
        private void Setup_DefaultReplicator()
        {
            Replicator = new ShuffleSeedReplicator();
            Replicator.Setup(eSNetReplicatorLifeTime.NeverDestroyed, SNet_ChannelType.SessionOrderCritical);
        }

        #endregion

        private void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                Replicator.State.SeedNumber++;
                Replicator.UpdateState();
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Replicator.State.SeedNumber--;
                Replicator.UpdateState();
            }

            GlobalMessage.OnUpdate?.Invoke();
        }

        private void OnFixedUpdate()
        {
            GlobalMessage.OnFixedUpdate?.Invoke();
        }
    }
}