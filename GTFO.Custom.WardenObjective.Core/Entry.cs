using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using CustomObjectives.GlobalHandlers.TimedObjectives;
using CustomObjectives.Messages;
using CustomObjectives.SimpleLoader;
using CustomObjectives.Utils;
using HarmonyLib;
using System;
using System.Linq;
using UnhollowerRuntimeLib;

namespace CustomObjectives
{
    [BepInPlugin("Custom-WardenObjective-Core", "Custom WardenObjective Core", "1.0.0.0")]
    [BepInDependency("Data-Dumper", BepInDependency.DependencyFlags.SoftDependency)]
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

            //Setup Simple Loader
            ObjectiveSimpleLoader.Setup();

            AssetShards.AssetShardManager.add_OnStartupAssetsLoaded((Il2CppSystem.Action)OnAssetLoaded);
        }

        #region ApplicationStart

        private void Setup_DefaultConfigs()
        {
            Logger.LogInstance = this.Log;
            Logger.IsGlobalEnabled = GetCfgValue("UseLogger", true, "Enable Logger?");

            var raw = GetCfgValue("UsingLogLevels", "Log, Warning, Error", "Allowed Level for Logger ('Verbose', 'Log', 'Warning' and 'Error')");
            var list = raw.Split(',').Select(item => item.Trim());
            Logger.IsVerboseEnabled = list.Any(x => x.Equals("Verbose", StringComparison.OrdinalIgnoreCase));
            Logger.IsLogEnabled = list.Any(x => x.Equals("Log", StringComparison.OrdinalIgnoreCase));
            Logger.IsWarnEnabled = list.Any(x => x.Equals("Warning", StringComparison.OrdinalIgnoreCase));
            Logger.IsErrorEnabled = list.Any(x => x.Equals("Error", StringComparison.OrdinalIgnoreCase));

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

        #endregion ApplicationStart

        private bool _gameInitCalled = false;

        private void OnAssetLoaded()
        {
            if (_gameInitCalled)
                return;

            OnGameInit_Once();
            _gameInitCalled = true;
        }

        private void OnGameInit_Once()
        {
            //Setup Global Handler
            GlobalBehaviour.Setup();
            GlobalBehaviour.OnUpdate += OnUpdate;
            GlobalBehaviour.OnFixedUpdate += OnFixedUpdate;

            Setup_LocalConfigs();
            Setup_DefaultReplicator();

            GlobalMessage.OnGameInit?.Invoke();
        }

        #region GameInit

        private void Setup_LocalConfigs()
        {
            ConfigUtil.SetupLocalConfig();

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

        private void Setup_DefaultReplicator()
        {
        }

        #endregion GameInit

        private void OnUpdate()
        {
            GlobalMessage.OnUpdate?.Invoke();
        }

        private void OnFixedUpdate()
        {
            GlobalMessage.OnFixedUpdate?.Invoke();
        }
    }
}