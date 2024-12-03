using StardewValley;
using Microsoft.Xna.Framework;
using static StardewValley.DelayedAction;

namespace Voidsent;
internal class TileActions
{
    public static bool BoatPassage(GameLocation location, string[] args, Farmer farmer, Point point)
    { //beach -> boat -> boat transition map of timer -> port morabyr
        var locationData = Game1.getLocationFromName("Aviroen.VoidsentCP_ArtificialBeach").GetData();
        if (locationData is not null &&
            locationData.CustomFields is not null &&
            locationData.CustomFields.TryGetValue("Aviroen.VoidsentCP_Boat", out string? customString))
        {
            if (bool.TryParse(customString, result: out bool stringActivated) && stringActivated)
            {
                warpAfterDelay("Aviroen.VoidsentCP_Morabyr", point, 60000);
                return true;
            }
        }
        // stop trying other interactions
        return true;
    }
    /*
you could like. just force teleport them out after a certain amount of time
not sure how you mean? countdown in updateWhenCurrentLocation or OnUpdateTicked, boot them out after a while
i don't remember if delayedaction respects pauses and time passing and such, which you could check for per-tick
delayedactions get checked in UpdateOther it seems which only gets called if the host is not paused. (or if gameMode == 10?)

implement 'safe route' which takes longer
*/
}
