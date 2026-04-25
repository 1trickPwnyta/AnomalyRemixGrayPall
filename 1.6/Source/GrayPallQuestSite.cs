using RimWorld.Planet;

namespace AnomalyRemixGrayPall
{
    public abstract class GrayPallQuestSite : MapParent
    {
        public abstract CaravanEnterMode EnterMode { get; }

        public abstract string EntryLetterLabel { get; }

        public abstract GlobalTargetInfo LookTarget { get; }

        public abstract string GetEntryLetterText(Caravan caravan);
    }
}
