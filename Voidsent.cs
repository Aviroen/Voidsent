using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;
using HarmonyLib;

namespace Voidsent;
public class Voidsent : Mod
{
    //internal static IViewEngine viewEngine = null!;
    //internal static string viewAssetPrefix = null!;
    internal static IFtmApi ftmApi = null!;
    private IViewEngine viewEngine = null!;
    internal static IModHelper ModHelper { get; set; } = null!;
    internal static IMonitor ModMonitor { get; set; } = null!;
    internal static Harmony Harmony { get; set; } = null!;
    internal static IManifest Manifest { get; set; } = null!;

    static List<string> voidsentLocations = new()
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

    public override void Entry(IModHelper helper)
    {
        //viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
        helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools

        ModHelper = helper;
        ModMonitor = Monitor;
        Harmony = new Harmony(ModManifest.UniqueID);
        Manifest = ModManifest;

        Harmony.PatchAll();
    }
    public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        ftmApi = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager")!;
        viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!;
        viewEngine.RegisterSprites($"Mods/{ModManifest.UniqueID}/Sprites", "Assets/Sprites");
        viewEngine.RegisterViews($"Mods/{ModManifest.UniqueID}/Views", "Assets/Views");
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
        for (int i = 0; i < voidsentLocations.Count; i++)
        {
            var location = voidsentLocations[i];
            if (Game1.getLocationFromName(location) is GameLocation loc)
            {
                // set the IsOutdoor thing in the map props.
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

    [HarmonyPatch(typeof(GameLocation), "ShowLockedDoorMessage")]
    public static bool Prefix()
    {
        foreach (var location in voidsentLocations)
        {
            if (Game1.currentLocation is location)
            {
                return false;
            }
        }
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