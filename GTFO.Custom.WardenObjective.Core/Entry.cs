using GTFO.CustomObjectives;
using GTFO.CustomObjectives.GlobalHandlers.TimedObjectives;
using GTFO.CustomObjectives.Inject.Global;
using GTFO.CustomObjectives.SimpleLoader;
using GTFO.CustomObjectives.Utils;
using MelonLoader;
using System;

[assembly: MelonInfo(typeof(Entry), "Custom WardenObjective Core", "1.0", "Flowaria")]
[assembly: MelonGame("10 Chambers Collective", "GTFO")]

namespace GTFO.CustomObjectives
{
    internal class Entry : MelonMod
    {
        private const string CONFIG_SECTION = "Custom WardenObjective Global";
        public const int CONFIG_VERSION = 1;

        public override void OnApplicationStart()
        {
            Setup_DefaultConfigs();
            Setup_DefaultGlobalHandlers();
            //Setup_DefaultReplicator();

            ObjectiveSimpleLoader.Setup();
        }

        public override void OnModSettingsApplied()
        {
            ConfigUtil.SetupLocalConfig();

            if (!ConfigUtil.HasLocalPath)
                return;

            if(ConfigUtil.TryGetLocalConfig<LocalConfigDTO>("ObjectivePrefs.json", out var config))
            {
                Logger.Log("=== Global Handler Whitelist ===");
                foreach (var p in config.EnabledModules)
                {
                    Logger.Log("Allowed Plugin: {0}", p);
                }
                CustomObjectiveManager.SetGlobalHandlerWhitelist(config.EnabledModules);
            }
        }

        
        private void Setup_DefaultConfigs()
        {
            if (!MelonPrefs.HasKey(CONFIG_SECTION, "Version"))
            {
                Register_DefaultConfigs();
            }
            else
            {
                var cfgVersion = MelonPrefs.GetInt(CONFIG_SECTION, "Version");
                if(cfgVersion < CONFIG_VERSION)
                {
                    MelonPrefs.SetInt(CONFIG_SECTION, "Version", CONFIG_VERSION);
                    Register_DefaultConfigs();
                }
            }

            Logger.IsGlobalEnabled = MelonPrefs.GetBool(CONFIG_SECTION, "UseLogger");
            Logger.IsVerboseEnabled = MelonPrefs.GetBool(CONFIG_SECTION, "UseLogger_Verbose");
            Logger.IsLogEnabled = MelonPrefs.GetBool(CONFIG_SECTION, "UseLogger_Log");
            Logger.IsWarnEnabled = MelonPrefs.GetBool(CONFIG_SECTION, "UseLogger_Warning");
            Logger.IsErrorEnabled = MelonPrefs.GetBool(CONFIG_SECTION, "UseLogger_Error");
        }

        private void Register_DefaultConfigs()
        {
            MelonPrefs.RegisterInt(CONFIG_SECTION, "Version", CONFIG_VERSION, "DO NOT TOUCH THIS VALUE");
            MelonPrefs.RegisterBool(CONFIG_SECTION, "UseLogger", true, "Use Logger?");
            MelonPrefs.RegisterBool(CONFIG_SECTION, "UseLogger_Verbose", false, "Display Verbose Logs?");
            MelonPrefs.RegisterBool(CONFIG_SECTION, "UseLogger_Log", true, "Display Normal Logs?");
            MelonPrefs.RegisterBool(CONFIG_SECTION, "UseLogger_Warning", true, "Display Warning Logs?");
            MelonPrefs.RegisterBool(CONFIG_SECTION, "UseLogger_Error", true, "Display Error Logs?");
            MelonPrefs.SaveConfig();
        }

        private void Setup_DefaultGlobalHandlers()
        {
            CustomObjectiveManager.AddGlobalHandler<TimedObjectiveHandler>("TimedObjective", CustomObjectiveSettings.MAIN_ONLY);
            //CustomObjectiveManager.AddGlobalHandler<FogControlTerminalHandler>(); later
            //TODO: New Global Handler: Change Light Settings (color / type / or whatever)
            //TODO: New Global Handler: Change Alarm Settings for existing door
            //TODO: New Global Handler: Regen Hibernating Enemies on other zone
        }

        private void Setup_DefaultReplicator()
        {
            FogLevelUtil.Setup();
        }

        public override void OnUpdate()
        {
            GlobalMessage.OnUpdate?.Invoke();
        }

        public override void OnFixedUpdate()
        {
            GlobalMessage.OnFixedUpdate?.Invoke();
        }
    }
}