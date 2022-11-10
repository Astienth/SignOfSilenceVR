using System;
using UnityEngine;
using Donteco;
using HarmonyLib;

namespace SignOfSilenceVR
{
    public class UIPatches
    {
        /*
        [HarmonyPatch]
        class CharacterMoving
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(PlayerMovementController), "OnMovementInputHandler")]
            public static void Postfix(Vector3 movement)
            {
                isMoving = movement.sqrMagnitude > 0;
            }
        }
        */
    }
}
