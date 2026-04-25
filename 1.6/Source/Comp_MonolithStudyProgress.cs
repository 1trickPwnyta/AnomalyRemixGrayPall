using RimWorld;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class Comp_MonolithStudyProgress : ThingComp
    {
        private Building_GrayPallMonolithBase Monolith => parent as Building_GrayPallMonolithBase;

        public CompProperties_MonolithStudyProgress Props => props as CompProperties_MonolithStudyProgress;

        public void Notify_StudyLevel(int level)
        {
            StudyNote note = Props.studyNotes.FirstOrDefault(n => n.threshold == level);
            if (note != null)
            {
                Find.LetterStack.ReceiveLetter(note.label, note.text.Formatted(Monolith.interactorPawn.Named("PAWN")), LetterDefOf.PositiveEvent, parent, quest: Utility.ScenarioQuest);
            }
        }
    }
}
