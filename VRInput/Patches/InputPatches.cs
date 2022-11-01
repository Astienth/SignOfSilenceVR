using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Profiling;

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
                var gamepadConnected = Traverse.Create<Donteco.GamepadManager>().Method("MakeFoo").GetValue<Donteco.GamepadManager>();
                Traverse.Create(gamepadConnected).Property("GamepadConnected").SetValue(true);
            }
        }

        [HarmonyPatch]
        class DontShowGamepadNotif
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Donteco.NotificationView), "Start")]
            public static bool PreFix()
            {
                return false;
            }
        }
    }
}