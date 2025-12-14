using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace AnomalyRemixGrayPall
{
    public class IncidentWorker_Ambush_Anomaly : IncidentWorker_Ambush
    {
        private static readonly HashSet<PawnGroupKindDef> allowedGroupKinds = new HashSet<PawnGroupKindDef>()
        {
            PawnGroupKindDefOf.Shamblers,
            PawnGroupKindDefOf.Fleshbeasts,
            PawnGroupKindDefOf.Sightstealers,
            PawnGroupKindDefOf.Metalhorrors,
            PawnGroupKindDefOf.Gorehulks,
            PawnGroupKindDefOf.Devourers,
            PawnGroupKindDefOf.Noctols,
            PawnGroupKindDefOf.SightstealersNoctols,
            PawnGroupKindDefOf.Chimeras
        };

        protected override List<Pawn> GeneratePawns(IncidentParms parms)
        {
            parms.faction = Faction.OfEntities;
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(allowedGroupKinds.Where(g => Faction.OfEntities.def.MinPointsToGeneratePawnGroup(g) < parms.points).RandomElement(), parms);
            return PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
        }

        protected override LordJob CreateLordJob(List<Pawn> generatedPawns, IncidentParms parms)
        {
            return new LordJob_AssaultColony(Faction.OfEntities, canKidnap: false, canTimeoutOrFlee: false, canSteal: false);
        }

        protected override string GetLetterText(Pawn anyPawn, IncidentParms parms)
        {
            Caravan caravan = parms.target as Caravan;
            return def.letterText.Formatted((caravan != null) ? caravan.Name : "yourCaravan".TranslateSimple()).Resolve().CapitalizeFirst();
        }
    }
}
