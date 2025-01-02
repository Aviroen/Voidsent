using StardewValley;
using Microsoft.Xna.Framework;
using static StardewValley.DelayedAction;
using StardewModdingAPI;

namespace Voidsent;

/// - Aviroen.Voidsent_BoatRide <location> <X> <Y> [mailflag]
/// grabbed directly from https://github.com/Mushymato/MiscMapActionsProperties/blob/main/MiscMapActionsProperties/Framework/Tile/HoleWarp.cs :smile:
internal class TileActions
{
    internal static readonly string TileAction_BoatRide = $"{Voidsent.Manifest}_BoatRide";
    
    internal static void Register()
    {
        GameLocation.RegisterTileAction(TileAction_BoatRide, (location, args, farmer, tile) => BoatPassage(location, args, farmer));
        GameLocation.RegisterTouchAction(TileAction_BoatRide, (location, args, farmer, tile) => BoatPassage(location, args, farmer));
    }
    public static bool BoatPassage(GameLocation location, string[] args, Farmer farmer)
    { //beach -> boat -> boat transition map of timer -> port morabyr

        return true;
    }

}
