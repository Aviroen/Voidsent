using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Voidsent.Monsters;
using StardewValley.BellsAndWhistles;
using StardewUI.Framework;

namespace Voidsent
{
    public class Voidsent : Mod
    {
        internal static IViewEngine viewEngine = null!;
        internal static string viewAssetPrefix = null!;
        internal static IFtmApi ftmApi = null!;

        public override void Entry(IModHelper helper)
        {
            viewAssetPrefix = $"Mods/{ModManifest.UniqueID}/Views";

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
        }
        public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ftmApi = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager")!;
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!;
            viewEngine.RegisterSprites($"Mods/{ModManifest.UniqueID}/Sprites", "assets/sprites");
            viewEngine.RegisterViews(viewAssetPrefix, "assets/views");
            GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_Boat",TileActions.BoatPassage);
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
            if (Game1.getLocationFromName($"{ModManifest.UniqueID}_ArtificialBeach") is GameLocation loc)
            {
                // set the IsOutdoor thing in the map props.
                loc.ignoreDebrisWeather.Value = true;
                loc.ignoreOutdoorLighting.Value = true;
                //loc.critters.Add(new Crow((int)v.X, (int)v.Y));
            }
            else
            {
                this.Monitor.Log($"Huh, can't find my loction, that's weird.");
            }
        }
        /*
        
        */
    }
}
