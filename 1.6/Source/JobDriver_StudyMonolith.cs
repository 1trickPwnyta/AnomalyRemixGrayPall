using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AnomalyRemixGrayPall
{
    public class JobDriver_StudyMonolith : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            
            Toil findAdjacentCell = Toils_General.Do(() =>
            {
                IntVec3 cell = SocialInteractionUtility.GetAdjacentInteractionCell(pawn, TargetThingA, job.playerForced);
                pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, cell);
                job.targetB = cell;
            });

            Toil goToAdjacentCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            int studyTicks = Mathf.RoundToInt(600f / pawn.GetStatValue(StatDefOf.ResearchSpeed));
            Toil studyToil = Toils_General.WaitWith(TargetIndex.A, studyTicks, useProgressBar: true, face: TargetIndex.A);
            studyToil.AddPreTickAction(() =>
            {
                pawn.skills.Learn(SkillDefOf.Intellectual, 0.1f);
            });
            studyToil.activeSkill = () => SkillDefOf.Intellectual;

            Toil finishInteraction = ToilMaker.MakeToil("Interaction finish");
            finishInteraction.initAction = Utility.GameComp.IncrementMonolithStudyProgress;

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return findAdjacentCell;
            yield return goToAdjacentCell;
            yield return studyToil;
            yield return finishInteraction;
            yield return Toils_Jump.Jump(findAdjacentCell);
        }

        public override bool? IsSameJobAs(Job j) => j.targetA == TargetThingA;
    }
}
