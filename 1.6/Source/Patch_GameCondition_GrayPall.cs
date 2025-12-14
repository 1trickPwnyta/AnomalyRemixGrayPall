using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(GameCondition_GrayPall))]
    [HarmonyPatch(nameof(GameCondition_GrayPall.End))]
    public static class Patch_GameCondition_GrayPall
    {
        public static Exception Finalizer(Exception __exception)
        {
            if (Utility.PlaystyleActive)
            {
                foreach (Map map in Find.Maps)
                {
                    map.weatherDecider.StartNextWeather();
                }
                if (__exception is NullReferenceException)
                {
                    return null;
                }
            }
            return __exception;
        }
    }
}
