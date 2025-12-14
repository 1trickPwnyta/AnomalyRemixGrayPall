#if DEBUG
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace AnomalyRemixGrayPall
{

    public static class DevTools
    {
        [DebugAction(AnomalyRemixGrayPallMod.PACKAGE_NAME, "Start gray pall", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        private static void StartGrayPall()
        {
            if (!EnsurePlaystyleActive()) return;
            Utility.GameComp.StartGrayPall();
        }

        [DebugAction(AnomalyRemixGrayPallMod.PACKAGE_NAME, "End gray pall", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        private static void EndGrayPall()
        {
            if (!EnsurePlaystyleActive()) return;
            Utility.GameComp.EndGrayPall();
        }

        [DebugAction(AnomalyRemixGrayPallMod.PACKAGE_NAME, "Force storyteller major incident", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        private static void ForceStorytellerMajorIncident()
        {
            IIncidentTarget target;
            if (WorldRendererUtility.WorldRendered)
            {
                target = Find.World;
            }
            else
            {
                target = Find.CurrentMap;
            }
            List<FiringIncident> incidents = new List<FiringIncident>();
            int i = 0;
            do
            {
                incidents.Clear();
                foreach (StorytellerComp comp in Find.Storyteller.storytellerComps)
                {
                    incidents.AddRange(Find.Storyteller.MakeIncidentsForInterval(comp, new System.Collections.Generic.List<IIncidentTarget>() { target }));
                }
            } while (!incidents.Any(fi => fi.def.category == IncidentCategoryDefOf.ThreatBig && fi.def.Worker.CanFireNow(fi.parms)) && i++ < 10000);
            if (i < 10000)
            {
                foreach (FiringIncident inc in incidents)
                {
                    Find.Storyteller.TryFire(inc);
                }
            }
            else
            {
                Messages.Message("No major incidents were generated. Try again.", MessageTypeDefOf.RejectInput, false);
            }
        }

        private static bool EnsurePlaystyleActive()
        {
            if (!Utility.PlaystyleActive)
            {
                Messages.Message("Gray Pall Anomaly playstyle is not active.", MessageTypeDefOf.RejectInput, false);
                return false;
            }
            return true;
        }
    }
}
#endif