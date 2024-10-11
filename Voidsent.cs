using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Voidsent.Monsters;

namespace Voidsent
{
    public class Voidsent : Mod
    {
        private void SetMyLocationFlags()
        {
            if (Game1.getLocationFromName("Aviroen.Voidsent_ArtificialBeach") is GameLocation loc)
            {
                // set the IsOutdoor thing in the map props.
                loc.ignoreDebrisWeather.Value = true;
                loc.ignoreOutdoorLighting.Value = true;
            }
            else
            {
                this.Monitor.Log($"Huh, can't find my loction, that's weird.");
            }
        }
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags(); //netbools
            helper.Events.GameLoop.SaveCreated += (_, _) => SetMyLocationFlags(); //netbools
        }
        public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager");
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
        /*
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Maps/Aviroen.Voidsent_ArtificialBeach"))
            {
                e.Edit(asset =>
                {
                    IAssetDataForMap editor = asset.AsMap();
                    Map map = editor.Data;

                    // your code here
                    ArtificialBeach artificialBeach = (ArtificialBeach)Game1.getLocationFromName("Aviroen.Voidsent_ArtificialBeach");
                });
            }
        } 
        from atra
        private void SetMyLocationFlags()
{
    if (Game1.getLocationFromName("Aviroen.Voidsent_ArtificialBeach") is GameLocation loc)
    {  
        // set the IsOutdoor thing in the map props.
        loc.ignoreDebrisWeather.Value = true;
        loc.ignoreOutdoorLighting.Value = true; 
    }
    else
    {
        this.Monitor.Log($"Huh, can't find my loction, that's weird.");
    }
}

public override void Entry(IModHelper helper)
{
    helper.GameLoop.SaveLoaded += (_, _) => SetMyLocationFlags();
    helper.GameLooop.SaveCreated += (_, _) => SetMyLocationFlags();
}
        */
    }
}
