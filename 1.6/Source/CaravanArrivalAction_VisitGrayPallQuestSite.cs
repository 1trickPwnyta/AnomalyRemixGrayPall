using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class CaravanArrivalAction_VisitGrayPallQuestSite : CaravanArrivalAction
    {
        private GrayPallQuestSite target;

        public CaravanArrivalAction_VisitGrayPallQuestSite(GrayPallQuestSite target)
        {
            this.target = target;
        }

        public override string Label => "AnomalyRemixGrayPall_InvestigateLocation".Translate(target.Label);

        public override string ReportString => "AnomalyRemixGrayPall_InvestigatingLocation".Translate(target.Label);

        public override void Arrived(Caravan caravan)
        {
            if (!target.HasMap)
            {
                LongEventHandler.QueueLongEvent(() => DoArrivalAction(caravan), "GeneratingMapForNewEncounter", false, null);
            }
            else
            {
                DoArrivalAction(caravan);
            }
        }

        private void DoArrivalAction(Caravan caravan)
        {
            bool firstArrival = !target.HasMap;
            if (firstArrival)
            {
                target.SetFaction(Faction.OfPlayer);
            }
            Map map = GetOrGenerateMapUtility.GetOrGenerateMap(target.Tile, null);
            CaravanEnterMapUtility.Enter(caravan, map, target.EnterMode, CaravanDropInventoryMode.UnloadIndividually);
            if (firstArrival)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                Find.LetterStack.ReceiveLetter(target.EntryLetterLabel, target.GetEntryLetterText(caravan), LetterDefOf.NeutralEvent, target.LookTarget, quest: Utility.ScenarioQuest);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref target, "target");
        }
    }
}
