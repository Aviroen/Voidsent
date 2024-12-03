﻿using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;
using HarmonyLib;
using Voidsent.Patches;
using StardewValley.Locations;

namespace Voidsent
{
    public class Voidsent : Mod
    {
        //internal static IViewEngine viewEngine = null!;
        //internal static string viewAssetPrefix = null!;
        /* iterate a list of all npcs with {{ModId}}_ to check per 8 heart and flag //might not need this
         * IncrementStat custom stat Aviroen.VoidsentCP_Fighters
         * use c# to check how many that is
         * divide the boss health 2% per int
         */
        internal static IFtmApi ftmApi = null!;
        private IViewEngine viewEngine = null!;
        internal static IModHelper ModHelper { get; set; } = null!;
        internal static IMonitor ModMonitor { get; set; } = null!;
        internal static Harmony Harmony { get; set; } = null!;
        internal static IManifest Manifest { get; set; } = null!;
        private List<Wisp>? _wisps;
        List<string> locNames = new()
    {
        "Aviroen.VoidsentCP_ArtificialBeach",
        "Aviroen.VoidsentCP_Commonwealth",
        "Aviroen.VoidsentCP_CrimsonGrove",
        "Aviroen.VoidsentCP_Grandberg",
        "Aviroen.VoidsentCP_Grove",
        "Aviroen.VoidsentCP_Morabyr",
        "Aviroen.VoidsentCP_Outlands",
        "Aviroen.VoidsentCP_Boat",
        "Aviroen.VoidsentCP_EdelweissHouse",
        "Aviroen.VoidsentCP_EdelweissAttic",
        "Aviroen.VoidsentCP_EdelweissBasement",
    };
        public static List<GameLocation> myLocations = new();
        public override void Entry(IModHelper helper)
        {
            //viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);
            Manifest = ModManifest;

            Harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ShowLockedDoorMessage)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Prefix)));
            //Harmony.PatchAll();
        }
        public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ftmApi = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager")!;
            //viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!;
            //viewEngine.RegisterSprites($"Mods/{ModManifest.UniqueID}/Sprites", "Assets/Sprites");
            //viewEngine.RegisterViews($"Mods/{ModManifest.UniqueID}/Views", "Assets/Views");
            GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_Boat", TileActions.BoatPassage);

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
            for (int i = 0; i < locNames.Count; i++)
            {
                var location = locNames[i];
                if (Game1.getLocationFromName(location) is GameLocation loc)
                {
                    //loc.treatAsOutdoors.Value = true;
                    loc.ignoreDebrisWeather.Value = false;
                    loc.ignoreOutdoorLighting.Value = true;
                    //loc.critters.Add(new Crow((int)v.X, (int)v.Y));
                    //FIND THE EYEBLINKING FROM FARMCAVE TO ADD TO THE CRIMSONGROVE/GROVE
                    //fireflies
                    //other ambient wiggly lights they're probably wisps
                }
                else
                {
                    this.Monitor.Log($"Huh, can't find my loction, that's weird.");
                }
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            myLocations.Clear();
            foreach (var l in locNames)
            {
                myLocations.Add(Game1.getLocationFromName(l));
            }
        }
    }
}