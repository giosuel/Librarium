using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Librarium.Binding;
using UnityEngine;
using Random = System.Random;

namespace Librarium;

public static class Utils
{
    /// <summary>
    ///     Tries to find value in a dictionary by key. If the key does not exist,
    ///     a new value of type T is created, indexed in the dictionary with the given key and returned.
    ///     Basically a helper function to emulate a default dictionary.
    /// </summary>
    public static T DictionaryGetOrNew<T>(IDictionary<string, T> map, string key) where T : new()
    {
        if (map.TryGetValue(key, out var list)) return list;
        return map[key] = new T();
    }

    public static void ToggleGameObjects(IEnumerable<GameObject> list, bool isOn)
    {
        foreach (var obj in list.Where(obj => obj)) obj.SetActive(isOn);
    }

    /// <summary>
    ///     Clones a random number generator. The new generator will produce the same sequence of numbers as the original.
    /// </summary>
    public static Random CloneRandom(Random random)
    {
        var cloned = new Random();

        // The seed array needs to be deep-copied since arrays are referenced types
        var seedArray = Reflection.Get<Random, int[]>(random, "_seedArray");
        Reflection.Set(cloned, "_seedArray", seedArray.ToArray());

        Reflection.CopyField(random, cloned, "_inextp");
        Reflection.CopyField(random, cloned, "_inext");

        return cloned;
    }

    /// <summary>
    ///     Attempts to invoke a callback.
    ///     If the callback throws a <see cref="NullReferenceException" /> returns default.
    /// </summary>
    /// <param name="callback"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T InvokeDefaultOnNull<T>(Func<T> callback)
    {
        try
        {
            return callback.Invoke();
        }
        catch (NullReferenceException)
        {
            return default;
        }
    }

    public static int ToggleLayerInMask(int layerMask, int layer)
    {
        if ((layerMask & (1 << layer)) != 0)
        {
            return layerMask & ~(1 << layer);
        }

        return layerMask | (1 << layer);
    }

    public static int ToggleLayersInMask(int layerMask, params int[] layers)
    {
        return layers.Aggregate(layerMask, ToggleLayerInMask);
    }

    public static bool RunSafe(Action action, string logTitle = null)
    {
        return RunSafe(action, out _, logTitle);
    }

    /// <summary>
    ///     Runs a function, catches all exceptions and returns a boolean with the status.
    ///     ///
    /// </summary>
    /// <param name="action"></param>
    /// <param name="exception"></param>
    /// <param name="logTitle"></param>
    /// <returns></returns>
    private static bool RunSafe(Action action, out Exception exception, string logTitle = null)
    {
        try
        {
            action.Invoke();
            exception = null;
            return true;
        }
        catch (Exception e)
        {
            exception = e;
            if (logTitle != null)
            {
                Log.LogBlock(
                    exception.StackTrace.Split('\n').Select(line => line.Trim()).ToList(),
                    title: $"[ERR] {logTitle}: {exception.Message}"
                );
            }

            return false;
        }
    }

    public abstract class Bindings
    {
        /// <summary>
        /// Adds or removes a value to / from a set in a binding and updates the binding.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="key">The key of the object to add to or remove from the set.</param>
        /// <param name="isOn">Whether the value should be present in the updated set.</param>
        public static void ToggleSet<T>(IBinding<HashSet<T>> binding, T key, bool isOn)
        {
            if (isOn)
            {
                binding.Value.Add(key);
            }
            else
            {
                binding.Value.Remove(key);
            }

            binding.Set(binding.Value);
        }
    }

    public abstract class Transpiling
    {
        public static IEnumerable<CodeInstruction> SkipWaitingForSeconds(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (i >= 2
                    && codes[i].opcode == OpCodes.Stfld
                    && codes[i - 1].opcode == OpCodes.Newobj
                    && codes[i - 2].opcode == OpCodes.Ldc_R4
                   )
                {
                    codes[i - 2].operand = 0f;
                }
            }

            return codes.AsEnumerable();
        }
    }
}