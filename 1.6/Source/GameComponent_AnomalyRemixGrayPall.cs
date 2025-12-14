using RimWorld;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class GameComponent_AnomalyRemixGrayPall : GameComponent
    {
        public float anomalyThreatsInactiveFraction = 0.08f;
        public float anomalyThreatsActiveFraction = 1f;
        public float grayPallMtbDays = 8f;
        public float grayPallExtraThreatMtbHours = 72f;

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
                if (!Utility.GrayPallActive && Rand.MTBEventOccurs(grayPallMtbDays, 60000f, 1f))
                {
                    StartGrayPall();
                }
                foreach (IIncidentTarget target in Find.Storyteller.AllIncidentTargets)
                {
                    if (Utility.GrayPallActive && Rand.MTBEventOccurs(grayPallExtraThreatMtbHours, 2500f, 1f))
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
                }
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
            }
        }

        public void StartGrayPall()
        {
            GameCondition grayPall = GameConditionMaker.MakeCondition(GameConditionDefOf.GrayPall, (int)(new FloatRange(1f, 3f).RandomInRange * 60000f));
            Find.World.GameConditionManager.RegisterCondition(grayPall);
        }

        public void EndGrayPall()
        {
            Find.World.GameConditionManager.GetActiveCondition<GameCondition_GrayPall>()?.End();
        }
    }
}
