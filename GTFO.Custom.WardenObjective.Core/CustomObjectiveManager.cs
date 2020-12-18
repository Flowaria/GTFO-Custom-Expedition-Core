using GameData;
using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTFO.CustomObjectives
{
    using HandlerTypeList = List<Type>;
    using HandlerTypeDict = Dictionary<byte, Type>;
    using HandlerSettingDict = Dictionary<Type, CustomObjectiveSettings>;
    using HandlerList = List<CustomObjectiveHandlerBase>;

    public static class CustomObjectiveManager
    {
        private readonly static HandlerList _ActiveHandlers;

        private readonly static HandlerTypeDict _Handlers;
        private readonly static HandlerTypeList _GlobalHandlers;

        private readonly static HandlerSettingDict _HandlerSetting;

        static CustomObjectiveManager()
        {
            _ActiveHandlers = new HandlerList();

            _Handlers = new HandlerTypeDict();
            _GlobalHandlers = new HandlerTypeList();

            _HandlerSetting = new HandlerSettingDict();

            GlobalMessage.OnLevelCleanup += () =>
            {
                UnloadAllHandler();
            };
        }

        /// <summary>
        /// Register Global CustomObjective Handler to Manager
        /// </summary>
        /// <typeparam name="T">Type of Handler (derived from CustomObjecitveHandler)</typeparam>
        public static void AddGlobalHandler<T>() where T : CustomObjectiveHandlerBase, new()
        {
            AddGlobalHandler<T>(CustomObjectiveSettings.ALL_LAYER);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public static void AddGlobalHandler<T>(CustomObjectiveSettings setting) where T : CustomObjectiveHandlerBase, new()
        {
            var type = typeof(T);

            if (type.IsAbstract)
                throw new ArgumentException("You can't use base handler class directly, Use derived class instead.");

            if (_Handlers.ContainsValue(type))
                throw new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");

            if (_GlobalHandlers.Contains(type))
                throw new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");

            _GlobalHandlers.Add(type);
            _HandlerSetting.Add(type, setting);
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

            if (_Handlers.ContainsValue(type))
                throw new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");

            if (_GlobalHandlers.Contains(type))
                throw new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");

            if (_Handlers.ContainsKey(typeID))
            {
                var dupType = _Handlers[typeID].GetType();
                throw new ArgumentException($"typeID: {typeID} is already defined by other plugin\nInfo:\n\t- Name: {dupType.Name}\n- Assembly: {dupType.Assembly.FullName}");
            }

            _Handlers.Add(typeID, type);
            _HandlerSetting.Add(type, setting);
        }

        internal static CustomObjectiveHandlerBase[] FireAllGlobalHandler(LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            var handlerList = new HandlerList();

            foreach (var handler in _GlobalHandlers)
            {
                handlerList.Add(FireHandlerByType(handler, layer, objectiveData));
            }

            return handlerList.ToArray();
        }

        internal static CustomObjectiveHandlerBase FireHandler(byte typeID, LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            if (_Handlers.TryGetValue(typeID, out var type))
            {
                return FireHandlerByType(type, layer, objectiveData);
            }
            else
            {
                throw new ArgumentException($"typeID: {typeID} is not defined, Are you missing some plugin?");
            }
        }

        private static CustomObjectiveHandlerBase FireHandlerByType(Type type, LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            if(!_HandlerSetting.TryGetValue(type, out var setting))
            {
                return null;
            }

            if(!setting.ShouldFire_Internal(layer.m_type, objectiveData))
            {
                return null;
            }

            var handler = Activator.CreateInstance(type) as CustomObjectiveHandlerBase;
            handler.Setup(layer, objectiveData);
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