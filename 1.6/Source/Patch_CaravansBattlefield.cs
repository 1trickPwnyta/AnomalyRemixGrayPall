using HarmonyLib;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(CaravansBattlefield))]
    [HarmonyPatch("CheckWonBattle")]
    public static class Patch_CaravansBattlefield
    {
        public static bool Prefix(CaravansBattlefield __instance)
        {
            if (__instance.Map?.attackTargetsCache?.TargetsHostileToColony?.Any(t => t.Thing is Pawn p && p.IsInvisibleCaravanThreat()) == true)
            {
                return false;
            }
            return true;
        }
    }
}
