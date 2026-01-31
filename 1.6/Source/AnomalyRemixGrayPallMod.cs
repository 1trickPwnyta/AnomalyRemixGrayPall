using HarmonyLib;
using UnityEngine;
using Verse;

namespace AnomalyRemixGrayPall
{
    [StaticConstructorOnStartup]
    public static class AnomalyRemixGrayPallInitializer
    {
        static AnomalyRemixGrayPallInitializer()
        {
            AnomalyRemixGrayPallMod.Settings = AnomalyRemixGrayPallMod.Mod.GetSettings<AnomalyRemixGrayPallSettings>();
        }
    }

    public class AnomalyRemixGrayPallMod : Mod
    {
        public const string PACKAGE_ID = "anomalyremixgraypall.1trickPwnyta";
        public const string PACKAGE_NAME = "Anomaly Remix: Gray Pall";

        public static AnomalyRemixGrayPallMod Mod;
        public static AnomalyRemixGrayPallSettings Settings;

        public AnomalyRemixGrayPallMod(ModContentPack content) : base(content)
        {
            Mod = this;

            var harmony = new Harmony(PACKAGE_ID);
            harmony.PatchAll();

            Log.Info("Ready.");
        }

        public override string SettingsCategory() => PACKAGE_NAME;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            AnomalyRemixGrayPallSettings.DoSettingsWindowContents(inRect);
        }
    }
}
