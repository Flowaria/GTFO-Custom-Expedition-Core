using GameData;
using GTFO.CustomObjectives.HandlerBase;
using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTFO.CustomObjectives
{
    using HandlerList = List<CustomObjectiveHandlerBase>;
    using HandlerTypeDict = Dictionary<byte, HandlerTypeContainer>;
    using HandlerTypeList = List<HandlerTypeContainer>;

    internal class HandlerTypeContainer
    {
        public Type BaseType;
        public CustomObjectiveSettings Setting;
        public string GUID;

        public CustomObjectiveHandlerBase CreateInstance()
        {
            return Activator.CreateInstance(BaseType) as CustomObjectiveHandlerBase;
        }
    }

    public static class CustomObjectiveManager
    {
        private readonly static HandlerList _ActiveHandlers;

        private readonly static HandlerTypeDict _Handlers;
        private readonly static HandlerTypeList _GlobalHandlers;

        private static string[] _AllowedGlobalHandlers;

        static CustomObjectiveManager()
        {
            _ActiveHandlers = new HandlerList();

            _Handlers = new HandlerTypeDict();
            _GlobalHandlers = new HandlerTypeList();

            GlobalMessage.OnLevelCleanup += () =>
            {
                UnloadAllHandler();
            };
        }

        internal static void SetGlobalHandlerWhitelist(string[] handlerGUIDs)
        {
            _AllowedGlobalHandlers = handlerGUIDs;
        }

        /// <summary>
        /// Register Global CustomObjective Handler to Manager
        /// </summary>
        /// <typeparam name="T">Type of Handler (derived from CustomObjecitveHandler)</typeparam>
        public static void AddGlobalHandler<T>(string GUID) where T : CustomObjectiveHandlerBase, new()
        {
            AddGlobalHandler<T>(GUID, CustomObjectiveSettings.ALL_LAYER);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public static void AddGlobalHandler<T>(string GUID, CustomObjectiveSettings setting) where T : CustomObjectiveHandlerBase, new()
        {
            var type = typeof(T);

            if (type.IsAbstract)
                throw new ArgumentException("You can't use base handler class directly, Use derived class instead.");

            if (IsTypeRegistered(type, out var exception))
                throw exception;

            if (IsGUIDRegistered(GUID, out exception))
                throw exception;

            _GlobalHandlers.Add(new HandlerTypeContainer()
            {
                BaseType = type,
                Setting = setting,
                GUID = GUID
            });

            Logger.Verbose("Global Handler Added: {0}", type.Name);
        }

        /// <summary>
        /// Register CustomObjective Handler to Manager
        /// </summary>
        /// <typeparam name="T">Type of Handler (derived from CustomObjecitveHandler)</typeparam>
        /// <param name="typeID">Type ID of Handler</param>
        public static void AddHandler<T>(byte typeID) where T : CustomObjectiveHandlerBase, new()
        {
            AddHandler<T>(typeID, CustomObjectiveSettings.ALL_LAYER);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeID"></param>
        /// <param name="setting"></param>
        public static void AddHandler<T>(byte typeID, CustomObjectiveSettings setting) where T : CustomObjectiveHandlerBase, new()
        {
            var type = typeof(T);

            if (type.IsAbstract)
                throw new ArgumentException("You can't use base handler class directly, Use derived class instead.");

            if (Enum.IsDefined(typeof(eWardenObjectiveType), typeID))
                throw new ArgumentException($"typeID: {typeID} is already defined inside default eWardenObjectiveType");

            if (IsTypeRegistered(type, out var exception))
                throw exception;

            if (_Handlers.ContainsKey(typeID))
            {
                var dupType = _Handlers[typeID].GetType();
                throw new ArgumentException($"typeID: {typeID} is already defined by other plugin\nInfo:\n\t- Name: {dupType.Name}\n- Assembly: {dupType.Assembly.FullName}");
            }

            _Handlers.Add(typeID, new HandlerTypeContainer()
            {
                BaseType = type,
                Setting = setting
            });

            Logger.Verbose("Handler Added: {0} (TypeID: {1})", type.Name, typeID);
        }

        private static bool IsTypeRegistered(Type type, out ArgumentException exception)
        {
            if (_Handlers.Any(x => x.Value?.BaseType?.Equals(type) ?? false))
            {
                exception = new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");
                return true;
            }

            if (_GlobalHandlers.Any(x => x.BaseType?.Equals(type) ?? false))
            {
                exception = new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");
                return true;
            }

            exception = null;
            return false;
        }

        private static bool IsGUIDRegistered(string GUID, out ArgumentException exception)
        {
            if(_GlobalHandlers.Any(x => x.GUID?.Equals(GUID, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                exception = new ArgumentException($"GUID ({GUID}) is already registered");
                return true;
            }

            exception = null;
            return false;
        }

        internal static CustomObjectiveHandlerBase[] FireAllGlobalHandler(LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            var handlerList = new HandlerList();

            foreach (var handler in _GlobalHandlers)
            {
                if (!_AllowedGlobalHandlers.Any(x=>x.Equals(handler.GUID, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var handlerInstance = FireHandlerByContainer(handler, layer, objectiveData, isGlobalHandler: true);

                if(handlerInstance != null)
                    handlerList.Add(handlerInstance);
            }

            return handlerList.ToArray();
        }

        internal static CustomObjectiveHandlerBase FireHandler(byte typeID, LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            if (_Handlers.TryGetValue(typeID, out var handler))
            {
                return FireHandlerByContainer(handler, layer, objectiveData);
            }
            else
            {
                throw new ArgumentException($"typeID: {typeID} is not defined, Are you missing some plugin?");
            }
        }

        private static CustomObjectiveHandlerBase FireHandlerByContainer(HandlerTypeContainer handlerContainer, LG_Layer layer, WardenObjectiveDataBlock objectiveData, bool isGlobalHandler = false)
        {
            if (handlerContainer.Setting == null)
            {
                return null;
            }

            if (!handlerContainer.Setting.ShouldFire_Internal(layer.m_type, objectiveData))
            {
                return null;
            }

            var handler = handlerContainer.CreateInstance();
            handler.Setup(layer, objectiveData, isGlobalHandler);
            handler.HandlerGUID = new Guid().ToString();

            _ActiveHandlers.Add(handler);

            return handler;
        }

        internal static void UnloadAllHandler()
        {
            foreach (var handler in _ActiveHandlers)
            {
                handler.Unload();
            }

            _ActiveHandlers.Clear();
        }

        internal static void UnloadHandler(CustomObjectiveHandlerBase handler)
        {
            //Direct search is now allowed, we need to find it by GUID
            var index = _ActiveHandlers.FindIndex(x => x.HandlerGUID == handler.HandlerGUID);

            if (index != -1)
                _ActiveHandlers.RemoveAt(index);

            handler.Unload();
        }
    }
}