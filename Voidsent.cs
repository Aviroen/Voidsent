using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;
using HarmonyLib;
using StardewValley.TokenizableStrings;

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
    };
    static List<GameLocation> myLocations = new();
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
        for (int i = 0; i < locNames.Count; i++)
        {
            var location = locNames[i];
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

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        myLocations.Clear();
        foreach (var l in locNames)
        {
            myLocations.Add(Game1.getLocationFromName(l));
        }
    }
    //many thanks to rokugin for helping set this one up
    [HarmonyPatch(typeof(GameLocation), "ShowLockedDoorMessage")]
    public static bool Prefix(GameLocation __instance, string[] action)
    {
        if (myLocations.Contains(__instance))
        {
            Gender ownerGender = Gender.Female;
            string[] ownerNames = new string[(action.Length == 2) ? 1 : 2];
            for (int i = 0; i < ownerNames.Length; i++)
            {
                string ownerKey = action[i + 1];
                NPC owner = Game1.getCharacterFromName(ownerKey);
                if (owner != null)
                {
                    ownerNames[i] = owner.displayName;
                    ownerGender = owner.Gender;
                    continue;
                }
                if (NPC.TryGetData(ownerKey, out var data))
                {
                    ownerNames[i] = TokenParser.ParseText(data.DisplayName);
                    ownerGender = data.Gender;
                    continue;
                }
            }
            string lockedDoorMessage = ((ownerNames.Length <= 1) ? Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + ((ownerGender == Gender.Male) ? "Male" : (ownerGender == Gender.Female) ? "Female" : "Undefined"), ownerNames[0]) : Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Couple", ownerNames[0], ownerNames[1]));

            Game1.drawObjectDialogue(lockedDoorMessage);
            return false;
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