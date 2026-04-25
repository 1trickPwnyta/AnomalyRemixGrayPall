using RimWorld.BaseGen;
using Verse;

namespace AnomalyRemixGrayPall
{
    [StaticConstructorOnStartup]
    public class SymbolResolver_Ominous_Opening : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            IntVec3 cell = rp.rect.CenterCell;
            Thing opening = ThingMaker.MakeThing(Utility.ominousOpeningDef);
            GenSpawn.Spawn(opening, cell, BaseGen.globalSettings.map);
        }
    }
}
