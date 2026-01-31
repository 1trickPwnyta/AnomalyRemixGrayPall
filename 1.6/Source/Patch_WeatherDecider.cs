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
        public static void Postfix(Map ___map, ref WeatherDef __result)
        {
            if (Utility.PlaystyleActive)
            {
                if (__result == WeatherDefOf.GrayPall)
                {
                    List<GameCondition> conditions = typeof(WeatherDecider).Field("allConditionsTmp").GetValue(null) as List<GameCondition>;
                    foreach (GameCondition condition in conditions.Reverse<GameCondition>())
                    {
                        WeatherDef weather = condition.ForcedWeather();
                        if (weather != null && !weather.canOccurAsRandomForcedEvent && weather != WeatherDefOf.GrayPall)
                        {
                            __result = weather; // All other non randomly occuring forced weather to override gray pall, such as blood rain
                            return;
                        }
                    }
                }
                else if (__result == null || __result.canOccurAsRandomForcedEvent)
                {
                    if (Utility.GrayPallActive)
                    {
                        __result = WeatherDefOf.GrayPall; // Gray pall overrides any weather that can occur randomly
                    }
                }
            }
        }
    }
}
