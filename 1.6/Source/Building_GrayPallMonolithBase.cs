using RimWorld;
using Verse;
using Verse.Sound;

namespace AnomalyRemixGrayPall
{
    public abstract class Building_GrayPallMonolithBase : Building, IThingGlower
    {
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref interactorPawn, "interactorPawn");
            Scribe_Values.Look(ref spawnLetterTick, "spawnLetterTick");
        }
    }
}
