using HarmonyLib;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class AnomalyRemixGrayPallMod : Mod
    {
        public const string PACKAGE_ID = "anomalyremixgraypall.1trickPwnyta";
        public const string PACKAGE_NAME = "Anomaly Remix Gray Pall";

        public AnomalyRemixGrayPallMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(PACKAGE_ID);
            harmony.PatchAll();

            Log.Info("Ready.");
        }
    }
}
