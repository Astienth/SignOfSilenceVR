using System;
using HarmonyLib;

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
    }
}