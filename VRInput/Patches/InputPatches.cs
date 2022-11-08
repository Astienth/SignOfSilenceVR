using System;
using HarmonyLib;
using UnityEngine;

namespace SignOfSilenceVR
{
    class InputPatches
    {
        [HarmonyPatch]
        class AcceptGamepad
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Donteco.GamepadManager), "Start")]
            public static void PostFix()
            {
                Donteco.GamepadManager.AcceptGamepad = true;
                Traverse.Create<Donteco.GamepadManager>().Property("GamepadConnected").SetValue(true);
            }
        }        

       [HarmonyPatch]
        class DontShowGamepadNotif
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Donteco.NotificationView), "Start")]
            public static bool PreFix()
            {
                return false;
            }
        }
        [HarmonyPatch]
        class GamepadInput
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Donteco.InputSystem), "GamepadInput")]
            public static void PostFix(ref Vector3 movement, ref Vector2 direction)
            {
                if (VRInputManager.rotateLeft)
                {
                    direction = new Vector2(-1,0);
                    VRInputManager.rotateLeft = false;
                }

                if (VRInputManager.rotateRight)
                {
                    direction = new Vector2(1,0);
                    VRInputManager.rotateRight = false;
                }

                if (VRInputManager.axisMove != Vector2.zero)
                {
                    movement = new Vector3(VRInputManager.axisMove.x, 0.0f, VRInputManager.axisMove.y);
                    VRInputManager.axisMove = Vector2.zero;
                }
            }
        }
    }
}