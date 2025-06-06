using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley.Triggers;
using System.Reflection;
using StardewValley.SpecialOrders;
using Voidsent.Integration;
using Voidsent.Patches;
using Voidsent.Framework.TileActions;
using Voidsent.Framework;
using System.Reflection.Metadata.Ecma335;

namespace Voidsent
{
    internal class Voidsent : Mod
    {
        internal static IFtmApi ftmApi = null!;
        internal static IModHelper ModHelper { get; set; } = null!;
        internal static IMonitor ModMonitor { get; set; } = null!;
        internal static Harmony Harmony { get; set; } = null!;
        internal static IManifest Manifest { get; set; } = null!;
        readonly List<string> outdoorNames = new List<string>
                {
        "Aviroen.VoidsentCP_ArtificialBeach",
        "Aviroen.VoidsentCP_Commonwealth",
        "Aviroen.VoidsentCP_CrimsonGrove",
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
        internal static readonly List<GameLocation> outdoorLocations = [];
        internal static readonly List<GameLocation> indoorLocations = [];
        public override void Entry(IModHelper helper)
        {
            if (!this.ValidateInstall())
                return;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            //helper.Events.Content.AssetRequested += OnAssetRequested;

            helper.ConsoleCommands.Add("lizard", "Spawn lizard.", Lizard.LizardDebugCmd);
            GameLocation.RegisterTileAction("Aviroen.Voidsent_SummonLizard", Lizard.HandleSummonLizard);
            //GameLocation.RegisterTileAction("Aviroen.Voidsent", Pathoschild.OnCentralAction);
            GameLocation.RegisterTileAction("Aviroen.VoidsentBoard", VSSpecialOrderBoard.OpenVSBoard);
            TriggerActionManager.RegisterAction("Aviroen.Voidsent_RandomDialogue", RandomDialogueAction.Action);

            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);
            Manifest = ModManifest;

            Lizard.Initialize(Monitor);
            TentPatch.Initialize(Monitor, ModManifest);
            SocialPagePatch.Initialize(Monitor);
            ProfileMenuPatch.Initialize(Monitor);
            RandomDialogueAction.Initialize(helper.ModRegistry);
            //Pathoschild.Initialize(helper.ModRegistry, ModManifest.UniqueID);
            VSSpecialOrderBoard.Initialize(Monitor, helper);

            
            Harmony.Patch(original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.IsTimedQuest)),
            postfix: new HarmonyMethod(typeof(SpecialOrderPatch), nameof(SpecialOrderPatch.IsTimedQuest_Postfix)));
            // patch to make special order time never decrease
            Harmony.Patch(original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.GetDaysLeft)),
            postfix: new HarmonyMethod(typeof(SpecialOrderPatch), nameof(SpecialOrderPatch.GetDaysLeft_Postfix)));

            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ftmApi = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager")!;
            
        }
        private bool ValidateInstall()
        {
            IModInfo? contentPack = this.Helper.ModRegistry.Get("Aviroen.VoidsentCP");
            IModInfo? contentCheck = this.Helper.ModRegistry.Get("ApryllForever.PolyamorySweetLove");
            if (contentPack is null)
            {
                this.Monitor.Log("VoidsentCP is missing the Content Pack. Please delete and reinstall the mod to fix.", LogLevel.Error);
                return false;
            }
            if (contentCheck != null)
            {
                this.Monitor.Log("Voidsent is incompatible with Polyamory Sweet. Voidsent will not be loaded.", LogLevel.Error);
                return false;
            }
            return true;
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
        {//written by atravita
            for (int i = 0; i < outdoorNames.Count; i++)
            {
                var location = outdoorNames[i];
                if (Game1.getLocationFromName(location) is GameLocation loc)
                {
                    //loc.treatAsOutdoors.Value = true;
                    loc.ignoreDebrisWeather.Value = false;
                    loc.ignoreOutdoorLighting.Value = false;
                    loc.tryToAddCritters();
                    loc.IsOutdoors = false;

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
        {//written by rokugin
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

    }
}