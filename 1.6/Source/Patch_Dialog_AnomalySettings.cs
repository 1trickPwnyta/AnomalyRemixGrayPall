using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(Dialog_AnomalySettings))]
    [HarmonyPatch("DrawPlaystyles")]
    public static class Patch_Dialog_AnomalySettings_DrawPlaystyles
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsList = instructions.ToList();
            int index = instructionsList.FindIndex(i => i.opcode == OpCodes.Stloc_2);
            instructionsList.InsertRange(index + 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, 2),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, typeof(Patch_Dialog_AnomalySettings_DrawPlaystyles).Method(nameof(CheckGrayPallAnomalyPlaystyleOnly)))
            });
            return instructionsList;
        }

        private static void CheckGrayPallAnomalyPlaystyleOnly(ref bool playstyleNotDisabledByScenario, AnomalyPlaystyleDef playstyle)
        {
            playstyleNotDisabledByScenario &= !Utility.ScenarioActive || playstyle == AnomalyPlaystyleDefOf.GrayPall;
        }
    }

    [HarmonyPatch(typeof(Dialog_AnomalySettings))]
    [HarmonyPatch("DrawExtraSettings")]
    public static class Patch_Dialog_AnomalySettings_DrawExtraSettings
    {
        public static void Postfix(AnomalyPlaystyleDef ___anomalyPlaystyleDef, Listing_Standard ___listing)
        {
            if (___anomalyPlaystyleDef == AnomalyPlaystyleDefOf.GrayPall)
            {
                GameComponent_AnomalyRemixGrayPall comp = Utility.GameComp;
                ___listing.Label("AnomalyRemixGrayPall_AnomalyThreatsInactive_Label".Translate() + ": " + comp.anomalyThreatsInactiveFraction.ToStringPercent() + " - " + Dialog_AnomalySettings.GetFrequencyLabel(comp.anomalyThreatsInactiveFraction), tipSignal: "AnomalyRemixGrayPall_AnomalyThreatsInactive_Info".Translate());
                comp.anomalyThreatsInactiveFraction = ___listing.Slider(comp.anomalyThreatsInactiveFraction, 0f, 1f);
                ___listing.Label("AnomalyRemixGrayPall_AnomalyThreatsActive_Label".Translate() + ": " + comp.anomalyThreatsActiveFraction.ToStringPercent() + " - " + Dialog_AnomalySettings.GetFrequencyLabel(comp.anomalyThreatsActiveFraction), tipSignal: "AnomalyRemixGrayPall_AnomalyThreatsActive_Info".Translate());
                comp.anomalyThreatsActiveFraction = ___listing.Slider(comp.anomalyThreatsActiveFraction, 0f, 1f);
                ___listing.Label("AnomalyRemixGrayPall_GrayPallMtbDays_Label".Translate() + ": " + comp.grayPallMtbDays.ToString("F1") + " - " + comp.grayPallMtbDays.GetGrayPallMtbDaysLabel(), tipSignal: "AnomalyRemixGrayPall_GrayPallMtbDays_Info".Translate());
                comp.grayPallMtbDays = ___listing.Slider(comp.grayPallMtbDays, 1f, 60f);
                comp.grayPallMaxTimeBetween = Mathf.Max(comp.grayPallMaxTimeBetween, comp.grayPallMtbDays * 2f);
            }
        }
    }
}
