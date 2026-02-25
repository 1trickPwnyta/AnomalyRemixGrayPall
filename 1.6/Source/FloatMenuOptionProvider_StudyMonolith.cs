using RimWorld;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class FloatMenuOptionProvider_StudyMonolith : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (!base.TargetThingValid(thing, context))
            {
                return false;
            }
            Building_GrayPallMonolithBase monolith = thing as Building_GrayPallMonolithBase;
            if (monolith == null)
            {
                return false;
            }
            if (!context.ValidSelectedPawns.Contains(monolith.interactorPawn))
            {
                return false;
            }
            return true;
        }

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            Building_GrayPallMonolithBase monolith = clickedThing as Building_GrayPallMonolithBase;
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("AnomalyRemixGrayPall_FloatMenuOptionStudyMonolith".Translate(monolith.LabelCap), monolith.StartStudying), monolith.interactorPawn, monolith);
        }
    }
}
