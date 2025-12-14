using RimWorld;
using Verse;

namespace AnomalyRemixGrayPall
{
    public static class Utility
    {
        public static bool PlaystyleActive => Find.Storyteller.difficulty.AnomalyPlaystyleDef == AnomalyPlaystyleDefOf.GrayPall;

        public static GameComponent_AnomalyRemixGrayPall GameComp => Current.Game.GetComponent<GameComponent_AnomalyRemixGrayPall>();

        public static bool GrayPallActive => Find.World.GameConditionManager.ConditionIsActive(GameConditionDefOf.GrayPall);

        public static string GetGrayPallMtbDaysLabel(this float mtbDays)
        {
            if (mtbDays > 30f)
            {
                return "AnomalyRemixGrayPall_MtbRare".Translate();
            }
            if (mtbDays > 15f)
            {
                return "AnomalyRemixGrayPall_MtbUncommon".Translate();
            }
            if (mtbDays > 7f)
            {
                return "AnomalyRemixGrayPall_MtbCommon".Translate();
            }
            return "AnomalyRemixGrayPall_MtbVeryCommon".Translate();
        }

        public static string GetGrayPallExtraThreatMtbHoursLabel(this float mtbHours)
        {
            if (mtbHours > 48f)
            {
                return "AnomalyRemixGrayPall_MtbUncommon".Translate();
            }
            if (mtbHours > 24f)
            {
                return "AnomalyRemixGrayPall_MtbCommon".Translate();
            }
            return "AnomalyRemixGrayPall_MtbVeryCommon".Translate();
        }
    }
}
