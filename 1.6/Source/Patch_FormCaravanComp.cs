using HarmonyLib;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(FormCaravanComp))]
    [HarmonyPatch(nameof(FormCaravanComp.AnyActiveThreatNow))]
    [HarmonyPatch(MethodType.Getter)]
    public static class Patch_FormCaravanComp
    {
        public static void Postfix(FormCaravanComp __instance, ref bool __result)
        {
            if (__instance.parent is MapParent m && m.Map?.attackTargetsCache?.TargetsHostileToColony?.Any(t => t.Thing is Pawn p && p.IsInvisibleCaravanThreat()) == true)
            {
                __result = true;
            }
        }
    }
}
