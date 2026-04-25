using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnomalyRemixGrayPall
{
    [StaticConstructorOnStartup]
    public class GameComponent_AnomalyRemixGrayPall : GameComponent
    {
        private static readonly ThingDef monolithDefA = ThingDef.Named("GrayPallMonolithA");
        private static readonly ThingDef monolithDefB = ThingDef.Named("GrayPallMonolithB");
        private static readonly LargeBuildingSpawnParms monolithSpawnParmsA;
        private static readonly LargeBuildingSpawnParms monolithSpawnParmsB;
        private static readonly WorldObjectDef monolithBWorldObjectDef = DefDatabase<WorldObjectDef>.GetNamed("GrayPallMonolithB");
        private static readonly WorldObjectDef grayPallSourceWorldObjectDef = DefDatabase<WorldObjectDef>.GetNamed("GrayPallSource");

        static GameComponent_AnomalyRemixGrayPall()
        {
            LargeBuildingSpawnParms parms = new LargeBuildingSpawnParms
            {
                attemptSpawnLocationType = SpawnLocationType.Outdoors,
                attemptNotUnderBuildings = true,
                canSpawnOnImpassable = false,
                allowFogged = false,
                ignoreTerrainAffordance = true
            };

            monolithSpawnParmsA = parms;
            monolithSpawnParmsA.thingDef = monolithDefA;
            monolithSpawnParmsA.minDistanceToColonyBuilding = 30f;
            monolithSpawnParmsA.maxDistanceToColonyBuilding = 50f;

            monolithSpawnParmsB = parms;
            monolithSpawnParmsB.thingDef = monolithDefB;
            monolithSpawnParmsB.minDistanceToColonyBuilding = 45f;
            monolithSpawnParmsB.maxDistanceToColonyBuilding = 75f;
        }

        public float anomalyThreatsInactiveFraction = 0f;
        public float anomalyThreatsActiveFraction = 1f;
        public float grayPallMtbDays = 8f;
        public float grayPallExtraThreatMtbHours = 72f;
        public float grayPallMinTimeBetween = 3f;
        public float grayPallMaxTimeBetween = 20f;

        private int nextGrayPallEndTick;
        private int lastGrayPallEndTick;
        private bool introPhase = true;
        private Map monolithSpawnMap;
        private IntVec3 monolithSpawnCell;
        private Building_GrayPallMonolithBase monolithSpawn;
        private int monolithDespawnTick;
        private int monolithDespawnLetterTick;
        private int monolithStudyProgress;

        public GameComponent_AnomalyRemixGrayPall()
        {
        }

        public GameComponent_AnomalyRemixGrayPall(Game _) : this()
        {
        }

        public override void GameComponentTick()
        {
            int ticks = Find.TickManager.TicksGame;

            if (Utility.PlaystyleActive)
            {
                if (ticks > 900000 && !Utility.GrayPallActive)
                {
                    if (ticks - grayPallMaxTimeBetween * 60000 > lastGrayPallEndTick || (ticks - grayPallMinTimeBetween * 60000 > lastGrayPallEndTick && Rand.MTBEventOccurs(grayPallMtbDays, 60000f, 1f)))
                    {
                        StartGrayPall(Utility.MonolithBMap != null ? new FloatRange?(new FloatRange(2f, 4f)) : null);
                    }
                }
                if (Utility.GrayPallActive)
                {
                    if (ticks >= nextGrayPallEndTick && Utility.GrayPallSourceMap == null)
                    {
                        EndGrayPall();
                    }
                    if (!Utility.ScenarioActive || !introPhase)
                    {
                        foreach (IIncidentTarget target in Find.Storyteller.AllIncidentTargets)
                        {
                            if (Rand.MTBEventOccurs(grayPallExtraThreatMtbHours, 2500f, 1f))
                            {
                                Log.Debug("DO INCIDENT");
                                DoIncident(target);
                            }
                        }
                    }
                }
            }

            if (Utility.ScenarioActive)
            {
                if (introPhase && !Utility.GrayPallActive && Find.Maps.Any(m => MapValidForMonolithSpawn(m)))
                {
                    StartFirstGrayPall();
                }
                if (CanSpawnMonolith())
                {
                    PlanMonolithSpawn();
                }
                if (ticks % 60 == 0 && Utility.GrayPallActive && monolithSpawnMap != null && monolithSpawnCell.IsValid)
                {
                    int radiusCells = GenRadial.NumCellsInRadius(10f);
                    bool spawned = false;
                    for (int i = 0; i < radiusCells && !spawned; i++)
                    {
                        IntVec3 c = monolithSpawnCell + GenRadial.RadialPattern[i];
                        if (c.InBounds(monolithSpawnMap))
                        {
                            foreach (Pawn pawn in c.GetThingList(monolithSpawnMap).OfType<Pawn>())
                            {
                                if (!spawned && pawn.IsColonistPlayerControlled && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight) && pawn.Awake() && GenSight.LineOfSight(pawn.Position, monolithSpawnCell, monolithSpawnMap))
                                {
                                    SpawnMonolith(pawn);
                                    spawned = true;
                                }
                            }
                        }
                    }
                }
                if (monolithDespawnLetterTick > 0 && ticks >= monolithDespawnLetterTick)
                {
                    monolithDespawnLetterTick = 0;
                    Find.LetterStack.ReceiveLetter("AnomalyRemixGrayPall_MonolithDespawnLetterLabel".Translate(), "AnomalyRemixGrayPall_MonolithDespawnLetterText".Translate(), LetterDefOf.NeutralEvent, null, quest: Utility.ScenarioQuest);
                }
            }
        }

        private IncidentDef GetRandomAnomalyIncident(IIncidentTarget target, out IncidentParms parms)
        {
            IncidentCategoryDef category = Rand.Bool ? IncidentCategoryDefOf.ThreatBig : IncidentCategoryDefOf.ThreatSmall;
            IncidentParms defaultParms = StorytellerUtility.DefaultParmsNow(category, target);
            Storyteller.AnomalyIncidents.Where(i => i.Worker.CanFireNow(defaultParms)).TryRandomElementByWeight(i => i.baseChance, out IncidentDef incident);
            if (incident == null)
            {
                category = category == IncidentCategoryDefOf.ThreatBig ? IncidentCategoryDefOf.ThreatSmall : IncidentCategoryDefOf.ThreatBig;
                defaultParms = StorytellerUtility.DefaultParmsNow(category, target);
                Storyteller.AnomalyIncidents.Where(i => i.Worker.CanFireNow(defaultParms)).TryRandomElementByWeight(i => i.baseChance, out incident);
            }
            parms = defaultParms;
            return incident;
        }

        private void DoIncident(IIncidentTarget target, IncidentDef forcedIncident = null, int delayTicks = 0)
        {
            IncidentDef incidentDef;
            IncidentParms parms;
            if (forcedIncident != null)
            {
                incidentDef = forcedIncident;
                parms = StorytellerUtility.DefaultParmsNow(forcedIncident.category, target);
                parms.forced = true;
            }
            else
            {
                incidentDef = GetRandomAnomalyIncident(target, out parms);
            }
            if (incidentDef != null)
            {
                Find.Storyteller.incidentQueue.Add(incidentDef, Find.TickManager.TicksGame + delayTicks, parms);
            }
        }

        public override void ExposeData()
        {
            if (Utility.PlaystyleActive)
            {
                Scribe_Values.Look(ref anomalyThreatsInactiveFraction, "anomalyThreatsInactiveFraction");
                Scribe_Values.Look(ref anomalyThreatsActiveFraction, "anomalyThreatsActiveFraction");
                Scribe_Values.Look(ref grayPallMtbDays, "grayPallMtbDays");
                Scribe_Values.Look(ref grayPallExtraThreatMtbHours, "grayPallExtraThreatMtbHours");
                Scribe_Values.Look(ref grayPallMinTimeBetween, "grayPallMinTimeBetween");
                Scribe_Values.Look(ref grayPallMaxTimeBetween, "grayPallMaxTimeBetween");
                Scribe_Values.Look(ref nextGrayPallEndTick, "nextGrayPallEndTick");
                Scribe_Values.Look(ref lastGrayPallEndTick, "lastGrayPallEndTick");
            }

            if (Utility.ScenarioActive)
            {
                Scribe_Values.Look(ref introPhase, "introPhase");
                Scribe_References.Look(ref monolithSpawnMap, "monolithSpawnMap");
                Scribe_Values.Look(ref monolithSpawnCell, "monolithSpawnCell");
                Scribe_References.Look(ref monolithSpawn, "monolith");
                Scribe_Values.Look(ref monolithDespawnTick, "monolithDespawnTick");
                Scribe_Values.Look(ref monolithDespawnLetterTick, "monolithDespawnLetterTick");
                Scribe_Values.Look(ref monolithStudyProgress, "monolithStudyProgress");
            }
        }

        private void StartFirstGrayPall()
        {
            StartGrayPall(new FloatRange(0.7f, 1.3f), false);
            PlanMonolithSpawn();
            IncidentDef introIncident = IncidentDef.Named("SightstealerArrival");
            DoIncident(Find.RandomPlayerHomeMap, introIncident, new IntRange(0, 15000).RandomInRange);
        }

        public void StartGrayPall(FloatRange? durationRangeDays = null, bool allowMessage = true)
        {
            if (!Utility.GrayPallActive)
            {
                GameCondition grayPall = GameConditionMaker.MakeCondition(GameConditionDefOf.GrayPall);
                grayPall.Permanent = true;
                grayPall.forceDisplayAsDuration = true;
                Find.World.GameConditionManager.RegisterCondition(grayPall);
                nextGrayPallEndTick = Find.TickManager.TicksGame + (int)((durationRangeDays ?? new FloatRange(1f, 3f)).RandomInRange * 60000f);
                if (allowMessage && AnomalyRemixGrayPallSettings.grayPallMessages)
                {
                    Messages.Message("AnomalyRemixGrayPall_GrayPallStarted".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }
        }

        public void EndGrayPall()
        {
            if (Utility.GrayPallActive)
            {
                Find.World.GameConditionManager.GetActiveCondition<GameCondition_GrayPall>()?.End();
                lastGrayPallEndTick = Find.TickManager.TicksGame;
                if (AnomalyRemixGrayPallSettings.grayPallMessages)
                {
                    Messages.Message("AnomalyRemixGrayPall_GrayPallEnded".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }
            introPhase = false;
        }

        private bool CanSpawnMonolith()
        {
            if (!Utility.GrayPallActive)
            {
                return false;
            }
            if (monolithSpawnMap != null || monolithSpawn != null)
            {
                return false;
            }
            if (Find.TickManager.TicksGame < monolithDespawnTick + 30000)
            {
                return false;
            }
            if (Utility.MonolithBWorldObject != null && Utility.MonolithBMap == null)
            {
                return false;
            }
            return true;
        }

        private bool MapValidForMonolithSpawn(Map map) => map.IsPlayerHome && !map.IsPocketMap && map.mapPawns.ColonistsSpawnedCount > 0;

        private void PlanMonolithSpawn()
        {
            Map map = Utility.MonolithBMap;
            if (map == null || !MapValidForMonolithSpawn(map))
            {
                Find.Maps.TryRandomElement(m => MapValidForMonolithSpawn(m), out map);
            }
            if (map != null)
            {
                LargeBuildingSpawnParms parms = MonolithSpawnParmsForMap(map);
                if (!LargeBuildingCellFinder.TryFindCell(out IntVec3 cell, map, parms))
                {
                    parms.minDistanceToColonyBuilding = 0f;
                    parms.maxDistanceToColonyBuilding = 0f;
                    LargeBuildingCellFinder.TryFindCell(out cell, map, parms, forceRecalculate: true);
                }
                if (cell.IsValid)
                {
                    monolithSpawnMap = map;
                    monolithSpawnCell = cell;
                    Log.Debug(cell); // TODO Remove this
                }
            }
        }

        private void SpawnMonolith(Pawn pawn)
        {
            Thing thing = null;
            ThingDef monolithDef = MonolithDefForMap(monolithSpawnMap);
            if (GenSpawn.CanSpawnAt(monolithDef, monolithSpawnCell, monolithSpawnMap, Rot4.North))
            {
                if (GenSpawn.TrySpawn(monolithDef, monolithSpawnCell, monolithSpawnMap, Rot4.North, out thing, WipeMode.VanishOrMoveAside))
                {
                    Building_GrayPallMonolithBase monolith = thing as Building_GrayPallMonolithBase;
                    monolith.interactorPawn = pawn;
                    monolithSpawn = monolith;
                    monolithSpawnMap = null;
                    monolithSpawnCell = IntVec3.Invalid;
                }
            }
            if (thing == null)
            {
                PlanMonolithSpawn();
            }
        }

        public void DespawnMonolith(bool successful = false)
        {
            if (monolithSpawn != null)
            {
                monolithSpawn.DeSpawn();
                monolithDespawnTick = Find.TickManager.TicksGame;
                if (!successful)
                {
                    monolithDespawnLetterTick = monolithDespawnTick + 60;
                }
                monolithSpawn = null;
            }
        }

        public void IncrementMonolithStudyProgress()
        {
            monolithStudyProgress++;
            Comp_MonolithStudyProgress comp = monolithSpawn.GetComp<Comp_MonolithStudyProgress>();
            comp.Notify_StudyLevel(monolithStudyProgress);
            if (monolithStudyProgress >= comp.Props.studyRequired)
            {
                if (Utility.MonolithBMap != null)
                {
                    // TODO maxDist should be int.MaxValue
                    DiscoverWorldObject(grayPallSourceWorldObjectDef, 10, TileFinderMode.Near, "AnomalyRemixGrayPall_GrayPallSourceDiscoveryLetter_Label".Translate(), "AnomalyRemixGrayPall_GrayPallSourceDiscoveryLetter_Text".Translate(monolithSpawn.interactorPawn.Named("PAWN"), monolithSpawn.Named("MONOLITH")), monolithBWorldObjectDef);
                }
                else
                {
                    // TODO maxDist should be int.MaxValue
                    DiscoverWorldObject(monolithBWorldObjectDef, 10, TileFinderMode.Furthest, "AnomalyRemixGrayPall_MonolithBDiscoveryLetter_Label".Translate(), "AnomalyRemixGrayPall_MonolithBDiscoveryLetter_Text".Translate(monolithSpawn.interactorPawn.Named("PAWN"), monolithSpawn.Named("MONOLITH")));
                }
                DespawnMonolith(true);
                ResetMonolithStudyProgress();
            }
        }

        public void ResetMonolithStudyProgress()
        {
            monolithStudyProgress = 0;
        }

        private void DiscoverWorldObject(WorldObjectDef def, int maxDist, TileFinderMode mode, string letterLabel, string letterText, WorldObjectDef defToRemoveFromQuest = null)
        {
            WorldObject worldObject = WorldObjectMaker.MakeWorldObject(def);
            TileFinder.TryFindNewSiteTile(out PlanetTile tile, maxDist: maxDist, tileFinderMode: mode);
            if (tile != null)
            {
                worldObject.Tile = tile;
                Find.World.worldObjects.Add(worldObject);
                List<GlobalTargetInfo> targets = Utility.ScenarioQuest.GetFirstPartOfType<QuestPart_LookTargets>().targets;
                targets.Add(worldObject);
                targets.RemoveWhere(t => t.HasWorldObject && t.WorldObject.def == defToRemoveFromQuest);
                Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, worldObject, quest: Utility.ScenarioQuest);
            }
            else
            {
                Log.Debug("Failed to find planet tile for " + def + ".");
            }
        }

        private ThingDef MonolithDefForMap(Map map) => map == Utility.MonolithBMap ? monolithDefB : monolithDefA;

        private LargeBuildingSpawnParms MonolithSpawnParmsForMap(Map map) => map == Utility.MonolithBMap ? monolithSpawnParmsB : monolithSpawnParmsA;
    }
}
