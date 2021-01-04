using ChainedPuzzles;
using CustomExpeditions.Messages;
using GameData;
using LevelGeneration;
using System;
using UnityEngine;

namespace CustomExpeditions.Utils.ChainedPuzzle
{
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

        public static T Setup<T>(uint id, LG_ComputerTerminal terminal) where T : ChainedPuzzleContext, new()
        {
            return Setup<T>(ChainedPuzzleDataBlock.GetBlock(id), terminal.SpawnNode.m_area, terminal.m_wardenObjectiveSecurityScanAlign);
        }

        public static T Setup<T>(ChainedPuzzleDataBlock block, LG_ComputerTerminal terminal) where T : ChainedPuzzleContext, new()
        {
            return Setup<T>(block, terminal.SpawnNode.m_area, terminal.m_wardenObjectiveSecurityScanAlign);
        }

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="id">Block ID to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(uint id, LG_Area area, Transform parent) where T : ChainedPuzzleContext, new()
        {
            return Setup<T>(ChainedPuzzleDataBlock.GetBlock(id), area, parent.position, parent);
        }

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="block">Block to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(ChainedPuzzleDataBlock block, LG_Area area, Transform parent) where T : ChainedPuzzleContext, new()
        {
            return Setup<T>(block, area, parent.position, parent);
        }

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="id">Block ID to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="position">Initial Position for spawn</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(uint id, LG_Area area, Vector3 position, Transform parent) where T : ChainedPuzzleContext, new()
        {
            return Setup<T>(ChainedPuzzleDataBlock.GetBlock(id), area, position, parent);
        }

        /// <summary>
        /// Create ChainedPuzzle Instance
        /// </summary>
        /// <typeparam name="T">ChainedPuzzleContext Type (derived or not)</typeparam>
        /// <param name="block">Block to use for puzzle</param>
        /// <param name="area">Area for chainedpuzzle</param>
        /// <param name="position">Initial Position for spawn</param>
        /// <param name="parent">Parent</param>
        /// <returns></returns>
        public static T Setup<T>(ChainedPuzzleDataBlock block, LG_Area area, Vector3 position, Transform parent) where T : ChainedPuzzleContext, new()
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
            }
        }
    }
}