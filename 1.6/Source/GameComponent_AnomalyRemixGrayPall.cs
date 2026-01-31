using RimWorld;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class GameComponent_AnomalyRemixGrayPall : GameComponent
    {
        public float anomalyThreatsInactiveFraction = 0f;
        public float anomalyThreatsActiveFraction = 1f;
        public float grayPallMtbDays = 8f;
        public float grayPallExtraThreatMtbHours = 72f;
        public float grayPallMinTimeBetween = 3f;
        public float grayPallMaxTimeBetween = 20f;
        private int nextGrayPallEndTick;
        private int lastGrayPallEndTick;

        public GameComponent_AnomalyRemixGrayPall()
        {
        }

        public GameComponent_AnomalyRemixGrayPall(Game _) : this()
        {
        }

        public override void GameComponentTick()
        {
            if (Utility.PlaystyleActive)
            {
                int ticks = Find.TickManager.TicksGame;
                if (ticks > 900000 && !Utility.GrayPallActive)
                {
                    if (ticks - grayPallMaxTimeBetween * 60000 > lastGrayPallEndTick || (ticks - grayPallMinTimeBetween * 60000 > lastGrayPallEndTick && Rand.MTBEventOccurs(grayPallMtbDays, 60000f, 1f)))
                    {
                        StartGrayPall();
                    }
                }
                if (Utility.GrayPallActive)
                {
                    if (ticks >= nextGrayPallEndTick)
                    {
                        EndGrayPall();
                    }
                    foreach (IIncidentTarget target in Find.Storyteller.AllIncidentTargets)
                    {
                        if (Rand.MTBEventOccurs(grayPallExtraThreatMtbHours, 2500f, 1f))
                        {
                            DoIncident(target);
                        }
                    }
                }
            }
        }

        private void DoIncident(IIncidentTarget target)
        {
            IncidentCategoryDef category = Rand.Bool ? IncidentCategoryDefOf.ThreatBig : IncidentCategoryDefOf.ThreatSmall;
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(category, target);
            Storyteller.AnomalyIncidents.Where(i => i.Worker.CanFireNow(parms)).TryRandomElementByWeight(i => i.baseChance, out IncidentDef incident);
            if (incident == null)
            {
                category = category == IncidentCategoryDefOf.ThreatBig ? IncidentCategoryDefOf.ThreatSmall : IncidentCategoryDefOf.ThreatBig;
                parms = StorytellerUtility.DefaultParmsNow(category, target);
                Storyteller.AnomalyIncidents.Where(i => i.Worker.CanFireNow(parms)).TryRandomElementByWeight(i => i.baseChance, out incident);
            }
            if (incident != null)
            {
                incident.Worker.TryExecute(parms);
            }
        }

        public override void ExposeData()
        {
            if (Utility.PlaystyleActive)
            {
                Scribe_Values.Look(ref anomalyThreatsInactiveFraction, "anomalyThreatsInactiveFraction");
                Scribe_Values.Look(ref anomalyThreatsActiveFraction, "anomalyThreatsActiveFraction");
                Scribe_Values.Look(ref grayPallMtbDays, "grayPallMtbDays");
                Scribe_Values.Look(ref grayPallExtraThreatMtbHours, "grayPallExtraThreatMtbHours");
                Scribe_Values.Look(ref grayPallMinTimeBetween, "grayPallMinTimeBetween");
                Scribe_Values.Look(ref grayPallMaxTimeBetween, "grayPallMaxTimeBetween");
                Scribe_Values.Look(ref nextGrayPallEndTick, "nextGrayPallEndTick");
                Scribe_Values.Look(ref lastGrayPallEndTick, "lastGrayPallEndTick");
            }
        }

        public void StartGrayPall()
        {
            if (!Utility.GrayPallActive)
            {
                GameCondition grayPall = GameConditionMaker.MakeCondition(GameConditionDefOf.GrayPall);
                grayPall.Permanent = true;
                grayPall.forceDisplayAsDuration = true;
                Find.World.GameConditionManager.RegisterCondition(grayPall);
                nextGrayPallEndTick = Find.TickManager.TicksGame + (int)(new FloatRange(1f, 3f).RandomInRange * 60000f);
                if (AnomalyRemixGrayPallSettings.grayPallMessages)
                {
                    Messages.Message("AnomalyRemixGrayPall_GrayPallStarted".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }
        }

        public void EndGrayPall()
        {
            if (Utility.GrayPallActive)
            {
                Find.World.GameConditionManager.GetActiveCondition<GameCondition_GrayPall>()?.End();
                lastGrayPallEndTick = Find.TickManager.TicksGame;
                if (AnomalyRemixGrayPallSettings.grayPallMessages)
                {
                    Messages.Message("AnomalyRemixGrayPall_GrayPallEnded".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }
        }
    }
}
