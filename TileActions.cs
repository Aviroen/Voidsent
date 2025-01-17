using StardewValley;
using Microsoft.Xna.Framework;
using static StardewValley.DelayedAction;
using StardewModdingAPI;
using StardewValley.Characters;

namespace Voidsent;

/// - Aviroen.Voidsent_BoatRide <location> <X> <Y> [mailflag]
/// grabbed directly from https://github.com/Mushymato/MiscMapActionsProperties/blob/main/MiscMapActionsProperties/Framework/Tile/HoleWarp.cs :smile:
internal class TileActions
{
    internal static readonly string TileAction_BoatRide = $"{Voidsent.Manifest}_BoatRide";
    internal static readonly string TileAction_HandleSummonLizard = $"{Voidsent.Manifest}_SummonLizard";
    
    internal static void Register()
    {
        GameLocation.RegisterTileAction(TileAction_BoatRide, (location, args, farmer, tile) => BoatPassage(location, args, farmer));
        GameLocation.RegisterTouchAction(TileAction_BoatRide, (location, args, farmer, tile) => BoatPassage(location, args, farmer));
    }
    public static bool BoatPassage(GameLocation location, string[] args, Farmer farmer)
    { //beach -> boat -> boat transition map of timer -> port morabyr

        return true;
    }

    private bool HandleSummonLizard(GameLocation location, string[] args, Farmer player, Point point)
    {
        string displayPrice = Utility.getNumberWithCommas(1000);
        location.createQuestionDialogue(
            $"Rent a lizard for {displayPrice}g?",
            location.createYesNoResponses(),
            (Farmer who, string answer) => LizardAnswer(who, answer, location, point),
            null
            );
        return true;
    }

    private void LizardAnswer(Farmer who, string answer, GameLocation loc, Point point)
    {
        if (answer == "Yes")
        {
            if (Game1.player.Money < 1000)
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            }
            else
            {
                Game1.player.Money -= 1000;
                var horse = new Horse(GuidHelper.NewGuid(), point.X + 1, point.Y);
                loc.characters.Add(horse);
                horse.Sprite.LoadTexture("Animals\\Aviroen.VoidsentCP_Komodo", syncTextureName: false);
                if (Game1.player.IsBusyDoingSomething())
                {
                    Game1.actionsWhenPlayerFree.Add(() => horse.checkAction(who, loc));
                }
                else
                {
                    horse.checkAction(who, loc);
                }
            }
        }
    }
}
