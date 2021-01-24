using ChainedPuzzles;
using CustomExpeditions.Messages;
using CustomExpeditions.Utility.Attributes;
using GameData;
using LevelGeneration;
using System;
using UnityEngine;

namespace CustomExpeditions.Utils
{
    using Terminal = LG_ComputerTerminal;
    using SecurityDoor = LG_SecurityDoor;
    using CPContext = ChainedPuzzleContext;
    using CPBlock = ChainedPuzzleDataBlock;

    [StaticConstructorAutorun]
    public static class ChainedPuzzleUtil
    {
        private static bool IsBuildDone = false;
        private static Action BuildQueue;

        static ChainedPuzzleUtil()
        {
            GlobalMessage.OnBuildDone += () =>
            {
                IsBuildDone = true;
                BuildQueue?.Invoke();
            };

            GlobalMessage.OnLevelCleanup += () =>
            {
                IsBuildDone = false;
                BuildQueue = null;
            };
        }

        #region Object Specific Overrides

        public static CPContext SetupDoor(uint id, SecurityDoor door)
            => SetupDoorBase<CPContext>(ToBlock(id), door);

        public static CPContext SetupDoor(CPBlock block, SecurityDoor door)
            => SetupDoorBase<CPContext>(block, door);

        public static T SetupDoor<T>(uint id, SecurityDoor door) where T : CPContext, new()
            => SetupDoorBase<T>(ToBlock(id), door);

        public static T SetupDoor<T>(CPBlock block, SecurityDoor door) where T : CPContext, new()
            => SetupDoorBase<T>(block, door);

        private static T SetupDoorBase<T>(CPBlock block, SecurityDoor door) where T : CPContext, new()
        {
            Vector3 position = door.m_bioScanAlign.position;
            if (!PhysicsUtil.SlamPos(ref position, Vector3.down, 4.0f, LayerManager.MASK_LEVELGEN, false, 0f, 1.0f))
            {
                return null;
            }

            return SetupBase<T>(block, door.Gate.ProgressionSourceArea, position, door.transform);
        }


        public static CPContext SetupTerminal(uint id, Terminal terminal)
            => SetupTerminalBase<CPContext>(ToBlock(id), terminal);

        public static CPContext SetupTerminal(CPBlock block, Terminal terminal)
            => SetupTerminalBase<CPContext>(block, terminal);

        public static T SetupTerminal<T>(uint id, Terminal terminal) where T : CPContext, new()
            => SetupTerminalBase<T>(ToBlock(id), terminal);

        public static T SetupTerminal<T>(CPBlock block, Terminal terminal) where T : CPContext, new()
            => SetupTerminalBase<T>(block, terminal);

        private static T SetupTerminalBase<T>(CPBlock block, Terminal terminal) where T : CPContext, new()
        {
            return SetupBase<T>(block, terminal.SpawnNode.m_area, terminal.m_wardenObjectiveSecurityScanAlign.position, terminal.m_wardenObjectiveSecurityScanAlign);
        }

        #endregion



        #region General Overrides

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="id">Block ID to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(uint id, LG_Area area, Transform parent) where T : CPContext, new()
            => SetupBase<T>(ToBlock(id), area, parent.position, parent);


        public static CPContext Setup(uint id, LG_Area area, Transform parent)
            => SetupBase<CPContext>(ToBlock(id), area, parent.position, parent);

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="block">Block to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(CPBlock block, LG_Area area, Transform parent) where T : CPContext, new()
            => SetupBase<T>(block, area, parent.position, parent);

        public static CPContext Setup(CPBlock block, LG_Area area, Transform parent)
            => SetupBase<CPContext>(block, area, parent.position, parent);

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="id">Block ID to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="position">Initial Position for spawn</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(uint id, LG_Area area, Vector3 position, Transform parent) where T : CPContext, new()
            => SetupBase<T>(ToBlock(id), area, position, parent);


        public static CPContext Setup(uint id, LG_Area area, Vector3 position, Transform parent)
            => SetupBase<CPContext>(ToBlock(id), area, position, parent);

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="block">Block to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="position">Initial Position for spawn</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(CPBlock block, LG_Area area, Vector3 position, Transform parent) where T : CPContext, new()
            => SetupBase<T>(block, area, position, parent);

        public static CPContext Setup(CPBlock block, LG_Area area, Vector3 position, Transform parent)
            => SetupBase<CPContext>(block, area, position, parent);

        #endregion

        private static T SetupBase<T>(CPBlock block, LG_Area area, Vector3 position, Transform parent) where T : CPContext, new()
        {
            var context = new T() { };

            if (IsBuildDone)
            {
                CreatePuzzleInstance();
            }
            else
            {
                BuildQueue += CreatePuzzleInstance;
            }

            return context;

            void CreatePuzzleInstance()
            {
                context.Instance = ChainedPuzzleManager.CreatePuzzleInstance(block, area, position, parent);
                context.Instance.add_OnPuzzleSolved(new Action(context.Solved_Internal));
                context.Setup(position);
            }
        }

        public static bool IsValidID(uint id) => CPBlock.HasBlock(id);

        private static CPBlock ToBlock(uint id) => CPBlock.GetBlock(id);

        public static bool TryGetBlock(uint id, out CPBlock block)
        {
            if(IsValidID(id))
            {
                block = ToBlock(id);
                return true;
            }

            block = null;
            return false;
        }
    }
}