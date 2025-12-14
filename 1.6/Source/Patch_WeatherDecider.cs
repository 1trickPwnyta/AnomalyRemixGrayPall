using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(WeatherDecider))]
    [HarmonyPatch(nameof(WeatherDecider.ForcedWeather))]
    [HarmonyPatch(MethodType.Getter)]
    public static class Patch_WeatherDecider
    {
        public static void Postfix(ref WeatherDef __result)
        {
            if (Utility.PlaystyleActive && __result == WeatherDefOf.GrayPall)
            {
                List<GameCondition> conditions = typeof(WeatherDecider).Field("allConditionsTmp").GetValue(null) as List<GameCondition>;
                foreach (GameCondition condition in conditions.Reverse<GameCondition>())
                {
                    WeatherDef weather = condition.ForcedWeather();
                    if (weather != null && weather != WeatherDefOf.GrayPall)
                    {
                        __result = weather;
                        return;
                    }
                }
            }
        }
    }
}
