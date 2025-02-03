﻿using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewUI.Framework;
using HarmonyLib;
using StardewValley.Triggers;
using System.Reflection;

namespace Voidsent
{
    internal class Voidsent : Mod
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
        internal static readonly List<GameLocation> outdoorLocations = [];
        internal static readonly List<GameLocation> indoorLocations = [];
        public override void Entry(IModHelper helper)
        {
            //viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            //helper.Events.Content.AssetRequested += OnAssetRequested;

            helper.ConsoleCommands.Add("lizard", "Spawn lizard.", Lizard.LizardDebugCmd);
            GameLocation.RegisterTileAction("Aviroen.Voidsent_SummonLizard", Lizard.HandleSummonLizard);
            TriggerActionManager.RegisterAction("Aviroen.Voidsent_RandomDialogue", RandomDialogueAction.Action);

            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);
            Manifest = ModManifest;

            Lizard.Initialize(Monitor);
            TentPatch.Initialize(Monitor, ModManifest.UniqueID);
            SocialPagePatch.Initialize(Monitor);
            ProfileMenuPatch.Initialize(Monitor);
            RandomDialogueAction.Initialize(helper.ModRegistry);

            Harmony.PatchAll(Assembly.GetExecutingAssembly());
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
        {//written by atravita
            for (int i = 0; i < outdoorNames.Count; i++)
            {
                var location = outdoorNames[i];
                if (Game1.getLocationFromName(location) is GameLocation loc)
                {
                    //loc.treatAsOutdoors.Value = true;
                    loc.ignoreDebrisWeather.Value = false;
                    loc.ignoreOutdoorLighting.Value = true;
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
        /*
        private string SeasonOverrider()
        {
            if (Game1.stats.DaysPlayed % 56 == 0)
            {
                for (int i = 0; i <= 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            return "Summer";
                        case 1:
                            return "Fall";
                        case 2:
                            return "Winter";
                        case 3:
                            return "Spring";
                    }
                }
            }
            return "Spring";
        }
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            for (int i = 0; i < outdoorNames.Count; i++)
            {
                var location = outdoorNames[i];
                if (e.Name.IsEquivalentTo("Maps/" + outdoorNames[i]))
                {
                    e.Edit(asset =>
                    {
                        IAssetDataForMap editor = asset.AsMap();
                        Map map = editor.Data;

                        map.Properties["SeasonOverride"] = SeasonOverrider();
                    });
                }
            }
        }
        */
    }
}