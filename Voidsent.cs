using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewUI.Framework;
using HarmonyLib;
using StardewValley.Characters;

namespace Voidsent
{
    internal static class GuidHelper
    {
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
    internal  class Voidsent : Mod
    {
        //internal static IViewEngine viewEngine = null!;
        //internal static string viewAssetPrefix = null!;
        /* iterate a list of all npcs with {{ModId}}_ to check per 8 heart and flag //might not need this
         * IncrementStat custom stat Aviroen.VoidsentCP_Fighters
         * use c# to check how many that is
         * divide the boss health 2% per int
         */
        internal static IFtmApi ftmApi = null!;
        //private IViewEngine viewEngine = null!;
        internal static IModHelper ModHelper { get; set; } = null!;
        internal static IMonitor ModMonitor { get; set; } = null!;
        internal static Harmony Harmony { get; set; } = null!;
        internal static IManifest Manifest { get; set; } = null!;
        readonly List<string> outdoorNames = new List<string>
                {
        "Aviroen.VoidsentCP_ArtificialBeach",
        "Aviroen.VoidsentCP_Commonwealth",
        "Aviroen.VoidsentCP_CrimsonGrove",
        "Aviroen.VoidsentCP_Grandberg",
        "Aviroen.VoidsentCP_Grove",
        "Aviroen.VoidsentCP_Morabyr",
        "Aviroen.VoidsentCP_Outlands",
        "Aviroen.VoidsentCP_Boat",
            };
        readonly List<string> indoorNames = new List<string>
                {
        "Aviroen.VoidsentCP_EdelweissHouse",
        "Aviroen.VoidsentCP_EdelweissAttic",
        "Aviroen.VoidsentCP_EdelweissBasement",
            };
        internal static readonly List<GameLocation> outdoorLocations = new();
        internal static readonly List<GameLocation> indoorLocations = new();
        public override void Entry(IModHelper helper)
        {
            //viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.ConsoleCommands.Add("lizard", "Spawn lizard.", Lizard);
            GameLocation.RegisterTileAction("Aviroen.Voidsent_SummonLizard", this.HandleSummonLizard);


            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);
            Manifest = ModManifest;

            TileActions.Register();
        }
        public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ftmApi = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager")!;
            //viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!;
            //viewEngine.RegisterSprites($"Mods/{ModManifest.UniqueID}/Sprites", "Assets/Sprites");
            //viewEngine.RegisterViews($"Mods/{ModManifest.UniqueID}/Views", "Assets/Views");
            
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.F5)
            {
                //
            }
        }
        private void SetMyLocationFlags()
        {
            for (int i = 0; i < outdoorNames.Count; i++)
            {
                var location = outdoorNames[i];
                if (Game1.getLocationFromName(location) is GameLocation loc)
                {
                    //loc.treatAsOutdoors.Value = true;
                    loc.ignoreDebrisWeather.Value = false;
                    loc.ignoreOutdoorLighting.Value = true;
                    loc.tryToAddCritters();
                    //loc.critters.Add(new Crow((int)v.X, (int)v.Y));
                    //FIND THE EYEBLINKING FROM FARMCAVE TO ADD TO THE CRIMSONGROVE/GROVE
                    //fireflies
                    //other ambient wiggly lights they're probably wisps
                }
                else
                {
                    this.Monitor.Log($"Huh, can't find my location, that's weird.");
                }
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            indoorLocations.Clear();
            foreach (var l in indoorNames)
            {
                indoorLocations.Add(Game1.getLocationFromName(l));
            }
            outdoorLocations.Clear();
            foreach (var k in outdoorNames)
            {
                outdoorLocations.Add(Game1.getLocationFromName(k));
            }
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
    /* IL_0202: stloc.s 10 //match for this
     * new method that takes SocialEntry socialEntry (loc, match) check for return new special identifier
     * then new stloc.s before \/
     * IL_048e: call string StardewValley.Game1::parseText(string, class [MonoGame.Framework]Microsoft.Xna.Framework.Graphics.SpriteFont, int32)
     */
}