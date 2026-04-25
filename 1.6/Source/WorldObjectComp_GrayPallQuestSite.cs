using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class WorldObjectComp_GrayPallQuestSite : WorldObjectComp
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan) => CaravanArrivalActionUtility.GetFloatMenuOptions(() => true, () => new CaravanArrivalAction_VisitGrayPallQuestSite(parent as GrayPallQuestSite), "AnomalyRemixGrayPall_InvestigateLocation".Translate(parent.Label), caravan, parent.Tile, parent);
    }
}
