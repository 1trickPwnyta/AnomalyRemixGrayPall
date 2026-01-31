using HarmonyLib;
using RimWorld;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(HorrorRelaxTransition))]
    [HarmonyPatch(nameof(HorrorRelaxTransition.IsTransitionSatisfied))]
    public static class Patch_HorrorRelaxTransition
    {
        public static void Postfix(ref bool __result)
        {
            if (Utility.GrayPallActive)
            {
                __result = true;
            }
        }
    }
}
