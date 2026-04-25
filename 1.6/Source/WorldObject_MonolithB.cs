using RimWorld.Planet;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class WorldObject_MonolithB : GrayPallQuestSite
    {
        public override CaravanEnterMode EnterMode => CaravanEnterMode.Center;

        public override string EntryLetterLabel => "AnomalyRemixGrayPall_MonolithBArrivalLetter_Label".Translate();

        public override GlobalTargetInfo LookTarget => new GlobalTargetInfo(Map.Center, Map);

        public override string GetEntryLetterText(Caravan caravan) => "AnomalyRemixGrayPall_MonolithBArrivalLetter_Text".Translate(caravan.LabelShortCap);

        public override void Notify_MyMapRemoved(Map map)
        {
            base.Notify_MyMapRemoved(map);
            Utility.GameComp.ResetMonolithStudyProgress();
        }
    }
}
