using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace Voidsent;

internal static class GuidHelper
{
    public static Guid NewGuid()
    {
        return Guid.NewGuid();
    }
}

internal sealed class ModEntry : Mod
{
    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        helper.ConsoleCommands.Add("lizard", "Spawn lizard.", this.Lizard);
        GameLocation.RegisterTileAction("Aviroen.Voidsent_SummonLizard", this.HandleSummonLizard);
        
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

    private void Lizard(string command, string[] args)
    {
        if (!ArgUtility.TryGetOptionalInt(args, 0, out var tileX, out var error, Game1.player.TilePoint.X, "int tileX") || !ArgUtility.TryGetOptionalInt(args, 0, out var tileY, out error, Game1.player.TilePoint.Y, "int tileY"))
        {
            Monitor.Log($"Invalid argument: {command}", LogLevel.Warn);
        }
        else
        {
            var horse = new Horse(GuidHelper.NewGuid(), tileX, tileY);
            Game1.currentLocation.characters.Add(horse);
            horse.Sprite.LoadTexture("Animals\\Aviroen.VoidsentCP_Komodo", syncTextureName: false);
        }
    }
}
