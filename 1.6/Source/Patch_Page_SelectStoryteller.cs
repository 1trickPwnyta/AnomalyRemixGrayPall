using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace AnomalyRemixGrayPall
{
    [HarmonyPatch(typeof(Page_SelectStoryteller))]
    [HarmonyPatch(nameof(Page_SelectStoryteller.PreOpen))]
    public static class Patch_Page_SelectStoryteller_PreOpen
    {
        public static void Prefix(Difficulty ___difficultyValues)
        {
            if (Utility.ScenarioActive)
            {
                ___difficultyValues.AnomalyPlaystyleDef = AnomalyPlaystyleDefOf.GrayPall;
            }
        }
    }

    [HarmonyPatch(typeof(Page_SelectStoryteller))]
    [HarmonyPatch("CanDoNext")]
    public static class Patch_Page_SelectStoryteller_CanDoNext
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsList = instructions.ToList();
            int index = instructionsList.FindIndex(i => i.LoadsField(typeof(Page_SelectStoryteller).Field("difficultyValues")));
            instructionsList.InsertRange(index + 1, new[]
            {
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, typeof(Patch_Page_SelectStoryteller_CanDoNext).Method(nameof(CheckGrayPallAnomalyPlaystyleOnly)))
            });
            return instructionsList;
        }

        public static void CheckGrayPallAnomalyPlaystyleOnly(Difficulty values)
        {
            if (Utility.ScenarioActive)
            {
                values.AnomalyPlaystyleDef = AnomalyPlaystyleDefOf.GrayPall;
            }
        }
    }
}
