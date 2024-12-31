using StardewValley;
using Microsoft.Xna.Framework;
using static StardewValley.DelayedAction;
using StardewModdingAPI;
using System.ComponentModel.Design;

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



    //make my own doorcode, with blackjack, and hookers

    /*
you could like. just force teleport them out after a certain amount of time
not sure how you mean? countdown in updateWhenCurrentLocation or OnUpdateTicked, boot them out after a while
i don't remember if delayedaction respects pauses and time passing and such, which you could check for per-tick
delayedactions get checked in UpdateOther it seems which only gets called if the host is not paused. (or if gameMode == 10?)

implement 'safe route' which takes longer
    //fix this later https://github.com/Mushymato/MiscMapActionsProperties/blob/main/%5BCP%5D%20MMAP%20Examples/content.json
        public static void Postfix(InteriorDoor __instance)
        {
            bool flicker = false;
            bool flip = false;
            Layer buildingLayer = __instance.Location.Map.RequireLayer("Buildings");

            Microsoft.Xna.Framework.Rectangle sourceRect = default(Microsoft.Xna.Framework.Rectangle);
            if (__instance.Tile == null)
                return;
            switch (__instance.Tile.TileIndex)
            {
                case 8:
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 48);
                    break;
                case 1:
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 48, 16, 48);
                    break;
                case 2:
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 18);
                    flip = true;
                    break;
                case 3:
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 144, 16, 18);
                    flip = true;
                    break;
            }

            if (__instance.Tile.Properties.ContainsKey("Aviroen.Voidsent_Doors"))
            {
                __instance.Sprite = new TemporaryAnimatedSprite("Maps\\Aviroen.VoidsentCP_Doors", sourceRect, 100f, 4, 1, new Vector2(__instance.Position.X, __instance.Position.Y - 2) * 64f, flicker, flip, (float)((__instance.Position.Y + 1) * 64 - 12) / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f)
                {
                    holdLastFrame = true,
                    paused = true
                };
            }
        }
*/
}
