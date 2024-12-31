using StardewModdingAPI;
using StardewValley;

namespace Voidsent;
internal class StupidCustomDoor
{
    internal static readonly string TileAction_CustomDoor = $"{Voidsent.Manifest}_CustomDoor";
    internal static void Register()
    {
        GameLocation.RegisterTileAction(TileAction_CustomDoor, (location, args, farmer, tile) => CustomDoor(location, args, farmer));
        GameLocation.RegisterTouchAction(TileAction_CustomDoor, (location, args, farmer, tile) => CustomDoor(location, args, farmer));
    }

    public static bool CustomDoor(GameLocation location, string[] args, Farmer farmer)
    { 

        return true;
    }
}
