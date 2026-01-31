using UnityEngine;
using Verse;

namespace AnomalyRemixGrayPall
{
    public class AnomalyRemixGrayPallSettings : ModSettings
    {
        public static bool grayPallMessages = false;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled("AnomalyRemixGrayPall_GrayPallMessages".Translate(), ref grayPallMessages);

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref grayPallMessages, "grayPallMessages", false);
        }
    }
}
