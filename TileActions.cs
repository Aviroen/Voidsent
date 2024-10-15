using StardewValley;
using Microsoft.Xna.Framework;

namespace Voidsent;
internal class TileActions
{

    public static bool BoatPassage(GameLocation location, string[] args, Farmer farmer, Point point)
    {
        var locationData = Game1.getLocationFromName("Aviroen.Voidsent_ArtificialBeach").GetData();
        if (locationData is not null &&
            locationData.CustomFields is not null &&
            locationData.CustomFields.TryGetValue("Aviroen.Voidsent_Boat", out string? customString)) 
        {
            if (bool.TryParse(customString, result: out bool stringActivated) && stringActivated)
            {
                return true;
            }
        }
        // stop trying other interactions
        return true;
    }

}
