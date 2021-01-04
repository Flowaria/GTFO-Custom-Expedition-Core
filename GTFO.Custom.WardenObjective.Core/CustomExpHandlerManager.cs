using CustomExpeditions.HandlerBase;
using CustomExpeditions.Messages;
using GameData;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomExpeditions
{
    using HandlerList = List<CustomExpHandlerBase>;
    using HandlerTypeDict = Dictionary<byte, HandlerTypeContainer>;
    using HandlerTypeList = List<HandlerTypeContainer>;

    internal class HandlerTypeContainer
    {
        public Type BaseType;
        public CustomExpSettings Setting;
        public string GUID;

        public CustomExpHandlerBase CreateInstance()
        {
            return Activator.CreateInstance(BaseType) as CustomExpHandlerBase;
        }
    }

    public static class CustomExpHandlerManager
    {
        private readonly static HandlerList _ActiveHandlers;

        private readonly static HandlerTypeDict _Handlers;
        private readonly static HandlerTypeList _GlobalHandlers;

        private static string[] _AllowedGlobalHandlers = new string[0];

        static CustomExpHandlerManager()
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
        public static void AddGlobalHandler<T>(string GUID) where T : CustomExpHandlerBase, new()
        {
            AddGlobalHandler<T>(GUID, CustomExpSettings.ALL_LAYER);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public static void AddGlobalHandler<T>(string GUID, CustomExpSettings setting) where T : CustomExpHandlerBase, new()
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
        public static void AddHandler<T>(byte typeID) where T : CustomExpHandlerBase, new()
        {
            AddHandler<T>(typeID, CustomExpSettings.ALL_LAYER);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeID"></param>
        /// <param name="setting"></param>
        public static void AddHandler<T>(byte typeID, CustomExpSettings setting) where T : CustomExpHandlerBase, new()
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
            if (_GlobalHandlers.Any(x => x.GUID?.Equals(GUID, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                exception = new ArgumentException($"GUID ({GUID}) is already registered");
                return true;
            }

            exception = null;
            return false;
        }

        internal static CustomExpHandlerBase[] FireAllGlobalHandler(LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            var handlerList = new HandlerList();

            foreach (var handler in _GlobalHandlers)
            {
                if (!_AllowedGlobalHandlers.Any(x => !string.IsNullOrEmpty(handler.GUID) && x.Equals(handler.GUID, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var handlerInstance = FireHandlerByContainer(handler, layer, objectiveData, isGlobalHandler: true);

                if (handlerInstance != null)
                    handlerList.Add(handlerInstance);
            }

            return handlerList.ToArray();
        }

        internal static CustomExpHandlerBase FireHandler(byte typeID, LG_Layer layer, WardenObjectiveDataBlock objectiveData)
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

        private static CustomExpHandlerBase FireHandlerByContainer(HandlerTypeContainer handlerContainer, LG_Layer layer, WardenObjectiveDataBlock objectiveData, bool isGlobalHandler = false)
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

        internal static void UnloadHandler(CustomExpHandlerBase handler)
        {
            //Direct search is now allowed, we need to find it by GUID
            var index = _ActiveHandlers.FindIndex(x => x.HandlerGUID == handler.HandlerGUID);

            if (index != -1)
                _ActiveHandlers.RemoveAt(index);

            handler.Unload();
        }
    }
}