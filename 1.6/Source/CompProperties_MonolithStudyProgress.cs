using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class CompProperties_MonolithStudyProgress : CompProperties
    {
        public int studyRequired;
        public List<StudyNote> studyNotes = new List<StudyNote>();

        public CompProperties_MonolithStudyProgress()
        {
            compClass = typeof(Comp_MonolithStudyProgress);
        }
    }
}
