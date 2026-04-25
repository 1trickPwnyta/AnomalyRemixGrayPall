using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class GenStep_GrayPallSource : GenStep_Scatterer
    {
        public override int SeedPart => 718838504;

        protected override bool CanScatterAt(IntVec3 loc, Map map)
        {
            if (!base.CanScatterAt(loc, map))
            {
                return false;
            }
            if (!loc.Standable(map))
            {
                return false;
            }
            if (loc.Roofed(map))
            {
                return false;
            }
            if (!map.reachability.CanReachMapEdge(loc, TraverseParms.For(TraverseMode.PassDoors, avoidPersistentDanger: true)))
            {
                return false;
            }
            ThingDef def = Utility.ominousOpeningDef;
            CellRect cellRect = new CellRect(loc.x - def.size.x / 2, loc.z - def.size.z / 2, def.size.x, def.size.z);
            if (!cellRect.FullyContainedWithin(new CellRect(0, 0, map.Size.x, map.Size.z)))
            {
                return false;
            }
            foreach (IntVec3 c2 in cellRect)
            {
                TerrainDef terrainDef = map.terrainGrid.TerrainAt(c2);
                if (!terrainDef.affordances.Contains(TerrainAffordanceDefOf.Heavy))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
        {
            ThingDef def = Utility.ominousOpeningDef;
            CellRect rect = new CellRect(loc.x - def.size.x / 2, loc.z - def.size.z / 2, def.size.x, def.size.z);
            ResolveParams rp = default;
            BaseGen.globalSettings.map = map;
            rp.rect = rect;
            BaseGen.symbolStack.Push("ominous_opening", rp);
            rp.faction = Faction.OfPlayer;
            BaseGen.symbolStack.Push("ensureCanReachMapEdge", rp);
            rp.clearFillageOnly = true;
            rp.clearRoof = true;
            BaseGen.symbolStack.Push("clear", rp);
            BaseGen.Generate();
        }
    }
}
