using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(StorytellerUI))]
    [HarmonyPatch("DrawCustomLeft")]
    public static class Patch_StorytellerUI
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsList = instructions.ToList();
            int index = instructionsList.FindIndex(i => i.LoadsConstant("Difficulty_AnomalyThreatsInactive_Label"));
            Label label = instructionsList[instructionsList.FindIndex(i => i.LoadsConstant("Difficulty_StudyEfficiency_Label")) - 1].labels[0];
            instructionsList.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, typeof(Patch_StorytellerUI).Method(nameof(DoGrayPallOptions))),
                new CodeInstruction(OpCodes.Brtrue_S, label),
                new CodeInstruction(OpCodes.Ldloc_0)
            });
            return instructionsList;
        }

        private static bool DoGrayPallOptions(Listing_Standard listing, Difficulty difficulty)
        {
            if (difficulty.AnomalyPlaystyleDef == AnomalyPlaystyleDefOf.GrayPall)
            {
                GameComponent_AnomalyRemixGrayPall comp = Utility.GameComp;
                PatchUtility_StorytellerUI.DrawCustomDifficultySlider(listing, "AnomalyRemixGrayPall_AnomalyThreatsInactive_Label".Translate(), Dialog_AnomalySettings.GetFrequencyLabel(comp.anomalyThreatsInactiveFraction), "AnomalyRemixGrayPall_AnomalyThreatsInactive_Info".Translate(), ref comp.anomalyThreatsInactiveFraction, ToStringStyle.PercentZero, ToStringNumberSense.Absolute, 0f, 1f);
                PatchUtility_StorytellerUI.DrawCustomDifficultySlider(listing, "AnomalyRemixGrayPall_AnomalyThreatsActive_Label".Translate(), Dialog_AnomalySettings.GetFrequencyLabel(comp.anomalyThreatsActiveFraction), "AnomalyRemixGrayPall_AnomalyThreatsActive_Info".Translate(), ref comp.anomalyThreatsActiveFraction, ToStringStyle.PercentZero, ToStringNumberSense.Absolute, 0f, 1f);
                PatchUtility_StorytellerUI.DrawCustomDifficultySlider(listing, "AnomalyRemixGrayPall_GrayPallMtbDays_Label".Translate(), comp.grayPallMtbDays.GetGrayPallMtbDaysLabel(), "AnomalyRemixGrayPall_GrayPallMtbDays_Info".Translate(), ref comp.grayPallMtbDays, ToStringStyle.FloatOne, ToStringNumberSense.Absolute, 1f, 60f);
                PatchUtility_StorytellerUI.DrawCustomDifficultySlider(listing, "AnomalyRemixGrayPall_GrayPallMinTimeBetween_Label".Translate(), "", "AnomalyRemixGrayPall_GrayPallMtbDays_Info".Translate(), ref comp.grayPallMinTimeBetween, ToStringStyle.FloatOne, ToStringNumberSense.Absolute, 1f, 60f);
                comp.grayPallMaxTimeBetween = Mathf.Max(comp.grayPallMaxTimeBetween, comp.grayPallMinTimeBetween, comp.grayPallMtbDays * 2f);
                PatchUtility_StorytellerUI.DrawCustomDifficultySlider(listing, "AnomalyRemixGrayPall_GrayPallMaxTimeBetween_Label".Translate(), "", "AnomalyRemixGrayPall_GrayPallMtbDays_Info".Translate(), ref comp.grayPallMaxTimeBetween, ToStringStyle.FloatOne, ToStringNumberSense.Absolute, Mathf.Max(comp.grayPallMinTimeBetween, comp.grayPallMtbDays * 2f), 60f);
                PatchUtility_StorytellerUI.DrawCustomDifficultySlider(listing, "AnomalyRemixGrayPall_GrayPallExtraThreatMtbHours_Label".Translate(), comp.grayPallExtraThreatMtbHours.GetGrayPallExtraThreatMtbHoursLabel(), "AnomalyRemixGrayPall_GrayPallExtraThreatMtbHours_Info".Translate(), ref comp.grayPallExtraThreatMtbHours, ToStringStyle.FloatOne, ToStringNumberSense.Absolute, 4f, 72f);
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch]
    public static class PatchUtility_StorytellerUI
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(StorytellerUI).Method("DrawCustomDifficultySlider", new[] { typeof(Listing_Standard), typeof(string), typeof(string), typeof(string), typeof(float).MakeByRefType(), typeof(ToStringStyle), typeof(ToStringNumberSense), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(float) });
        }

        [HarmonyReversePatch]
        public static void DrawCustomDifficultySlider(Listing_Standard listing, string label, string labelSuffix, string tooltip, ref float value, ToStringStyle style, ToStringNumberSense numberSense, float min, float max, float precision = 0.01f, bool reciprocate = false, float reciprocalCutoff = 1000f)
        {
        }
    }
}
