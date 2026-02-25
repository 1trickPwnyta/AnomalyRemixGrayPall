using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AnomalyRemixGrayPall
{
    [StaticConstructorOnStartup]
    public abstract class Building_GrayPallMonolithBase : Building, IThingGlower
    {
        private static readonly Texture2D studyIcon = ContentFinder<Texture2D>.Get("UI/Icons/Study");
        public static readonly JobDef studyMonolithJobDef = DefDatabase<JobDef>.GetNamed("AnomalyRemixGrayPall_StudyMonolith");

        public Pawn interactorPawn;
        private int spawnLetterTick;

        public abstract bool ShouldBeLitNow();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                spawnLetterTick = Find.TickManager.TicksGame + 60;
                TargetInfo info = new TargetInfo(Position, Map);
                EffecterDefOf.MonolithLevelChanged.Spawn().Trigger(info, info);
                SoundDef.Named("VoidMonolith_ActivatedL0L1").PlayOneShot(this);
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            TargetInfo info = new TargetInfo(Position, Map);
            EffecterDefOf.ImpactDustCloud.Spawn().Trigger(info, info);
            SoundDef.Named("VoidMonolith_Gleaming").PlayOneShot(this);
            base.DeSpawn(mode);
        }

        public override void TickRare()
        {
            base.TickRare();
            if (spawnLetterTick > 0 && Find.TickManager.TicksGame > spawnLetterTick)
            {
                Find.LetterStack.ReceiveLetter(LabelCap, "AnomalyRemixGrayPall_MonolithLetterText".Translate(interactorPawn.Named("PAWN")), LetterDefOf.NeutralEvent);
                spawnLetterTick = 0;
            }
            if (!Utility.GrayPallActive || interactorPawn == null || !interactorPawn.Spawned || interactorPawn.Map != Map || !interactorPawn.Awake() || interactorPawn.Position.DistanceToSquared(Position) > 100)
            {
                Utility.GameComp.DespawnMonolith();
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            yield return new Command_Action()
            {
                defaultLabel = "AnomalyRemixGrayPall_CommandActionStudyMonolith".Translate(interactorPawn.Named("PAWN")),
                defaultDesc = "AnomalyRemixGrayPall_CommandActionStudyMonolithDesc".Translate(interactorPawn.Named("PAWN")),
                icon = studyIcon,
                action = () =>
                {
                    StartStudying();
                    SoundDefOf.Click.PlayOneShot(null);
                },
                Disabled = interactorPawn?.Map != Map
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref interactorPawn, "interactorPawn");
            Scribe_Values.Look(ref spawnLetterTick, "spawnLetterTick");
        }

        public void StartStudying()
        {
            Job job = JobMaker.MakeJob(studyMonolithJobDef, this);
            interactorPawn.jobs.StartJob(job, JobCondition.InterruptForced);
        }
    }
}
