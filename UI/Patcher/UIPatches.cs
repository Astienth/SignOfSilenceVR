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

        [HarmonyPatch]
        class RaycasterFIx
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Raycaster), "FixedUpdate")]
            public static bool Prefix(Raycaster __instance)
            {
                PlayerHealth health = Traverse.Create(__instance)
                    .Field("playerHealth").GetValue<PlayerHealth>();
                if (health.IsDead)
                {
                    Traverse.Create(__instance)
                    .Field("Target").SetValue((GameObject)null);
                }
                else
                {
                    Transform transComp = Traverse.Create(__instance)
                        .Field("transformComp").GetValue<Transform>();

                    Traverse.Create(__instance)
                    .Property("Hited").SetValue(Physics.Raycast(
                        transComp.position + transComp.forward * __instance.Offset,
                        transComp.forward,
                        out __instance.HitInfo,
                        __instance.Distance,
                         ~(1 << LayerMask.NameToLayer("UI"))
                    ));

                    if (!__instance.Hited)
                    {
                        Traverse.Create(__instance)
                            .Field("Target").SetValue((GameObject)null);
                    }
                    else
                    {
                        Traverse.Create(__instance)
                            .Field("Target").SetValue(__instance.HitInfo.transform.gameObject);
                    }
                }
                return false;
            }
        }
        /*
        [HarmonyPatch]
        class DistanceLog
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(FlashlightController), "AdaptiveIntensity")]
            public static void Postfix(FlashlightController __instance, float distance)
            {
                Logs.WriteWarning("FLASH LIGHT DISTANCE " + distance.ToString());
            }
        }
        */
                
        [HarmonyPatch]
        class DistantRaycasterFIx
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(DistantRaycaster), "FixedUpdate")]
            public static bool Prefix(DistantRaycaster __instance)
            {
                PlayerHealth health = Traverse.Create(__instance)
                    .Field("playerHealth").GetValue<PlayerHealth>();
                if (health.IsDead)
                {
                    Traverse.Create(__instance)
                    .Field("Target").SetValue((GameObject)null);
                }
                else
                {
                    Transform transComp = Traverse.Create(__instance)
                        .Field("transformComp").GetValue<Transform>();

                    Traverse.Create(__instance)
                    .Property("Hited").SetValue(Physics.SphereCast(
                        transComp.position + transComp.forward * __instance.Offset,
                        Traverse.Create(__instance)
                            .Field("radius").GetValue<float>(),
                        transComp.forward,
                        out __instance.HitInfo,
                        Mathf.Infinity,
                        ~(1 << LayerMask.NameToLayer("UI"))
                    ));

                    if (!__instance.Hited)
                    {
                        Traverse.Create(__instance)
                            .Field("Target").SetValue((GameObject)null);
                    }
                    else
                    {
                        Traverse.Create(__instance)
                            .Field("Target").SetValue(__instance.HitInfo.transform.gameObject);
                    }
                }
                return false;
            }
        }
    }
}
