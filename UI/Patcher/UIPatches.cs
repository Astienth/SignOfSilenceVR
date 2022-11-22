using System;
using UnityEngine;
using Donteco;
using HarmonyLib;

namespace SignOfSilenceVR
{
    public class UIPatches
    {
        [HarmonyPatch]
        class DepthOfField
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(DofController), "Update")]
            public static bool Prefix()
            {
                return false;
            }
        }
    }
}
