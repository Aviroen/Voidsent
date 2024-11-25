using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;

namespace Voidsent;
public class Voidsent : Mod
{
    //internal static IViewEngine viewEngine = null!;
    //internal static string viewAssetPrefix = null!;
    internal static IFtmApi ftmApi = null!;
    private IViewEngine viewEngine = null!;

    public override void Entry(IModHelper helper)
    {
        //viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
        helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
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
        if (Game1.getLocationFromName($"{ModManifest.UniqueID}_CrimsonGrove") is GameLocation loc)
        {
            // set the IsOutdoor thing in the map props.
            loc.ignoreDebrisWeather.Value = false;
            loc.ignoreOutdoorLighting.Value = true;
            //loc.critters.Add(new Crow((int)v.X, (int)v.Y));
        }
        else
        {
            this.Monitor.Log($"Huh, can't find my loction, that's weird.");
        }
    }
    /*
    you could like. just force teleport them out after a certain amount of time
    not sure how you mean? countdown in updateWhenCurrentLocation or OnUpdateTicked, boot them out after a while
    i don't remember if delayedaction respects pauses and time passing and such, which you could check for per-tick
    delayedactions get checked in UpdateOther it seems which only gets called if the host is not paused. (or if gameMode == 10?)

    implement 'safe route' which takes longer
    */
}