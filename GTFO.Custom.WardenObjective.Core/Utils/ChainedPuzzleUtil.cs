using ChainedPuzzles;
using GameData;
using LevelGeneration;
using System;
using UnityEngine;

namespace GTFO.CustomObjectives.Utils
{
    public static class ChainedPuzzleUtil
    {
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
            var puzzle = ChainedPuzzleManager.CreatePuzzleInstance(block, area, position, parent);

            var context = new T() { Instance = puzzle };
            context.Instance.add_OnPuzzleSolved(new Action(context.OnSolved));

            return context;
        }
    }
}