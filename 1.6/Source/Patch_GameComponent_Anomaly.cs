using HarmonyLib;
using RimWorld;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(GameComponent_Anomaly))]
    [HarmonyPatch(nameof(GameComponent_Anomaly.AnomalyThreatFractionNow))]
    [HarmonyPatch(MethodType.Getter)]
    public static class Patch_GameComponent_Anomaly
    {
        public static void Postfix(ref float __result)
        {
            if (Utility.PlaystyleActive)
            {
                __result = Utility.GrayPallActive ? Utility.GameComp.anomalyThreatsActiveFraction : Utility.GameComp.anomalyThreatsInactiveFraction;
            }
        }
    }
}
