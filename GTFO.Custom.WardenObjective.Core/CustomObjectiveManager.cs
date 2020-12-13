using GameData;
using GTFO.CustomObjectives.Inject.Global;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO.CustomObjectives
{
    using HandlerList = List<CustomObjectiveHandler>;
    using HandlerDict = Dictionary<byte, Type>;
    using HandlerTypeList = List<Type>;

    public static class CustomObjectiveManager
    {
        private readonly static HandlerList _ActiveHandlers;
        private readonly static HandlerDict _Handlers;
        private readonly static HandlerTypeList _GlobalHandlers;

        static CustomObjectiveManager()
        {
            _ActiveHandlers = new HandlerList();
            _Handlers = new HandlerDict();
            _GlobalHandlers = new HandlerTypeList();

            GlobalMessage.OnLevelCleanup += () =>
            {
                UnloadAllHandler();
            };
        }

        /// <summary>
        /// Register Global CustomObjective Handler to Manager
        /// </summary>
        /// <typeparam name="T">Type of Handler (derived from CustomObjecitveHandler)</typeparam>
        public static void AddGlobalHandler<T>() where T : CustomObjectiveHandler, new()
        {
            var type = typeof(T);

            if (type.IsAbstract)
                throw new ArgumentException("You can't use base handler class directly, Use derived class instead.");

            if(_GlobalHandlers.Contains(type))
                throw new ArgumentException($"You can't add same type of handler multiple times\n- type: {type.Name}");

            _GlobalHandlers.Add(type);
        }

        /// <summary>
        /// Register CustomObjective Handler to Manager
        /// </summary>
        /// <typeparam name="T">Type of Handler (derived from CustomObjecitveHandler)</typeparam>
        /// <param name="typeID">Type ID of Handler</param>
        public static void AddHandler<T>(byte typeID) where T : CustomObjectiveHandler, new()
        {
            var type = typeof(T);

            if (type.IsAbstract)
                throw new ArgumentException("You can't use base handler class directly, Use derived class instead.");

            if (Enum.IsDefined(typeof(eWardenObjectiveType), typeID))
                throw new ArgumentException($"typeID: {typeID} is already defined inside default eWardenObjectiveType");

            if (_Handlers.ContainsKey(typeID))
            {
                var dupType = _Handlers[typeID].GetType();
                throw new ArgumentException($"typeID: {typeID} is already defined by other plugin\nInfo:\n\t- Name: {dupType.Name}\n- Assembly: {dupType.Assembly.FullName}");
            }

            _Handlers.Add(typeID, type);
        }

        internal static CustomObjectiveHandler[] FireAllGlobalHandler(LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            var handlerList = new HandlerList();

            foreach(var handler in _GlobalHandlers)
            {
                handlerList.Add(FireHandlerByType(handler, layer, objectiveData));
            }

            return handlerList.ToArray();
        }

        internal static CustomObjectiveHandler FireHandler(byte typeID, LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            if (_Handlers.ContainsKey(typeID))
            {
                var type =  _Handlers[typeID];

                return FireHandlerByType(type, layer, objectiveData);
            }
            else
            {
                throw new ArgumentException($"typeID: {typeID} is not defined, Are you missing some plugin?");
            }
        }

        private static CustomObjectiveHandler FireHandlerByType(Type type, LG_Layer layer, WardenObjectiveDataBlock objectiveData)
        {
            var handler = Activator.CreateInstance(type) as CustomObjectiveHandler;
            handler.Setup(layer, objectiveData);

            _ActiveHandlers.Add(handler);

            return handler;
        }

        internal static void UnloadAllHandler()
        {
            foreach(var handler in _ActiveHandlers)
            {
                handler.Unload();
            }

            _ActiveHandlers.Clear();
        }

        internal static void UnloadHandler(CustomObjectiveHandler handler)
        {
            if(_ActiveHandlers.Contains(handler))
                _ActiveHandlers.Remove(handler);

            handler.Unload();
        }
    }
}
