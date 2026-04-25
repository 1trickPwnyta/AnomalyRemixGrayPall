using RimWorld.Planet;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class WorldObject_GrayPallSource : GrayPallQuestSite
    {
        public override CaravanEnterMode EnterMode => CaravanEnterMode.Edge;

        public override string EntryLetterLabel => "AnomalyRemixGrayPall_GrayPallSourceArrivalLetter_Label".Translate();

        public override GlobalTargetInfo LookTarget => Map.listerBuildings.AllBuildingsNonColonistOfDef(Utility.ominousOpeningDef).FirstOrDefault() ?? GlobalTargetInfo.Invalid;

        public override string GetEntryLetterText(Caravan caravan) => "AnomalyRemixGrayPall_GrayPallSourceArrivalLetter_Text".Translate(caravan.LabelShortCap);

        public override void PostMapGenerate()
        {
            base.PostMapGenerate();
            if (!Utility.GrayPallActive)
            {
                Utility.GameComp.StartGrayPall(allowMessage: false);
            }
        }
    }
}
