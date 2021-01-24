using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using CustomExpeditions.CustomReplicators.Inject;
using CustomExpeditions.GlobalHandlers.TimedObjectives;
using CustomExpeditions.GlobalHandlers.TweakedSettings;
using CustomExpeditions.Messages;
using CustomExpeditions.SimpleLoader;
using CustomExpeditions.Utility.Attributes;
using CustomExpeditions.Utils;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace CustomExpeditions
{
    [BepInPlugin("Custom.Expedition.Core", "Custom Expedition Core", "1.0.0.0")]
    [BepInDependency("Data-Dumper", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("GTFO.exe")]
    internal class EntryPoint : BasePlugin
    {
        private const string CONFIG_SECTION = "Global";
        private readonly Harmony HarmonyInstance = new Harmony("flowaria.CustomExp.Core.Harmony");

        public override void Load()
        {
            Logger.LogInstance = Log;

            Setup_InjectIL2CPPTypes();
            Setup_HarmonyPatches();
            Setup_ExecuteAutoConstructor();
            Setup_DefaultConfigs();
            Setup_DefaultGlobalHandlers();

            //Setup Simple Loader
            ExpSimpleLoader.Load();

            AssetShards.AssetShardManager.add_OnStartupAssetsLoaded((Il2CppSystem.Action)OnAssetLoaded);
        }

        #region ApplicationStart

        private void Setup_ExecuteAutoConstructor()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<StaticConstructorAutorunAttribute>();
                if(attr != null)
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }

        private void Setup_InjectIL2CPPTypes()
        {
            ClassInjector.RegisterTypeInIl2Cpp<GlobalBehaviour>();
        }

        private void Setup_HarmonyPatches()
        {
            //Setup Harmony Patcher
            HarmonyInstance.PatchAll();

            //Patch Harmony Methods Manually
            Inject_Replication.MakePatch(HarmonyInstance);
        }

        private void Setup_DefaultConfigs()
        {
            Logger.IsGlobalEnabled = GetCfgValue("UseLogger", true, "Enable Logger?");

            var raw = GetCfgValue("UsingLogLevels", "Log, Warning, Error", "Allowed Level for Logger ('Verbose', 'Log', 'Warning' and 'Error')");

            var list = raw.Split(',').Select(item => item.Trim());
            Logger.IsVerboseEnabled =   list.Any(x => IgnoreCaseEqual(x, "Verbose"));
            Logger.IsLogEnabled =       list.Any(x => IgnoreCaseEqual(x, "Log"));
            Logger.IsWarnEnabled =      list.Any(x => IgnoreCaseEqual(x, "Warning"));
            Logger.IsErrorEnabled =     list.Any(x => IgnoreCaseEqual(x, "Error"));

            Config.Save();

            bool IgnoreCaseEqual(string str1, string str2)
            {
                return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
            }
        }

        private T GetCfgValue<T>(string entry, T defaultValue, string description)
        {
            return Config.Bind(new ConfigDefinition(CONFIG_SECTION, entry), defaultValue, new ConfigDescription(description)).Value;
        }

        private void Setup_DefaultGlobalHandlers()
        {
            CustomExpHandlerManager.AddGlobalHandler<TweakedSettingHandler>("TweakedSetting", CustomExpSettings.MAIN_ONLY);
            CustomExpHandlerManager.AddGlobalHandler<TimedObjectiveHandler>("TimedObjective", CustomExpSettings.MAIN_ONLY);
            //CustomObjectiveManager.AddGlobalHandler<FogControlTerminalHandler>(); later
            //TODO: New Global Handler: Change Light Settings (color / type / or whatever)
            // New Global Handler: Change Alarm Settings for existing door
            // New Global Handler: Regen Hibernating Enemies on other zone
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
            PostSetup_GlobalBehaviour();
            PostSetup_LocalConfigs();
            PostSetup_DefaultReplicator();

            GlobalMessage.OnGameInit?.Invoke();
        }

        #region GameInit

        private void PostSetup_GlobalBehaviour()
        {
            GlobalBehaviour.Setup();
            GlobalBehaviour.OnUpdate += () =>
            {
                GlobalMessage.OnUpdate?.Invoke();
                GlobalMessage.OnUpdate_Level?.Invoke();
            };
            GlobalBehaviour.OnFixedUpdate += () =>
            {
                GlobalMessage.OnFixedUpdate?.Invoke();
                GlobalMessage.OnFixedUpdate_Level?.Invoke();
            };
        }

        private void PostSetup_LocalConfigs()
        {
            ConfigUtil.SetupLocalConfig();

            if (!ConfigUtil.HasLocalPath)
                return;

            if (ConfigUtil.TryGetLocalConfig<LocalConfigDTO>("CustomExpPrefs.json", out var config))
            {
                Logger.Log("=== Global Handler Whitelist ===");
                foreach (var p in config.EnabledGlobalHandlers)
                {
                    Logger.Log("Allowed Plugin: {0}", p);
                }
                CustomExpHandlerManager.SetGlobalHandlerWhitelist(config.EnabledGlobalHandlers.ToArray());
            }
        }

        private void PostSetup_DefaultReplicator()
        {
        }

        #endregion GameInit
    }
}