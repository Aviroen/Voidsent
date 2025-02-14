using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;

namespace Voidsent.Framework
{
    internal static class AssetManager
    {
        internal static IManifest Manifest { get; set; } = null!;
        static Dictionary<string, Models> outdoorLocations = null!;
        static Dictionary<string, Models> indoorLocations = null!;

        public static Dictionary<string, Models> outdoorData
        {
            get
            {
                if (outdoorLocations == null)
                {
                    outdoorLocations = Game1.content.Load<Dictionary<string, Models>>("Aviroen.Voidsent/outLocations");
                }
                return outdoorLocations;
            }
        }
    }
}
