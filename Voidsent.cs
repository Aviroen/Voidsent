using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.SpecialOrders;
using StardewValley.Triggers;
using Voidsent.Framework;
using Voidsent.Framework.TileActions;
using Voidsent.Patches;
using Voidsent.Integration;

namespace Voidsent
{
    internal class Voidsent : Mod
    {
        internal static IModHelper ModHelper { get; set; } = null!;
        internal static IMonitor ModMonitor { get; set; } = null!;
        internal static Harmony Harmony { get; set; } = null!;
        internal static IManifest Manifest { get; set; } = null!;
        public override void Entry(IModHelper helper)
        {
            if (!this.ValidateInstall())
                return;

            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);
            Manifest = ModManifest;

            helper.Events.Player.Warped += OnWarp;
            helper.ConsoleCommands.Add("lizard", "Spawn lizard.", Lizard.LizardDebugCmd);
            GameLocation.RegisterTileAction("Aviroen.Voidsent_SummonLizard", Lizard.HandleSummonLizard);
            GameLocation.RegisterTileAction("Aviroen.VoidsentBoard", VSSpecialOrderBoard.OpenVSBoard);
            //TriggerActionManager.RegisterAction("Aviroen.Voidsent_RandomDialogue", RandomDialogueAction.Action);
            //Event.RegisterCommand(currentCustomEventScript);

            Lizard.Initialize(Monitor);
            TentPatch.Initialize(Monitor, ModManifest);
            SocialPagePatch.Initialize(Monitor);
            ProfileMenuPatch.Initialize(Monitor);
            RandomDialogueAction.Initialize(helper.ModRegistry);
            VSSpecialOrderBoard.Initialize(Monitor, helper);

            Harmony.Patch(original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.IsTimedQuest)),
            postfix: new HarmonyMethod(typeof(SpecialOrderPatch), nameof(SpecialOrderPatch.IsTimedQuest_Postfix)));
            // patch to make special order time never decrease
            Harmony.Patch(original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.GetDaysLeft)),
            postfix: new HarmonyMethod(typeof(SpecialOrderPatch), nameof(SpecialOrderPatch.GetDaysLeft_Postfix)));

            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnWarp(object? sender, WarpedEventArgs e)
        {
            if (e.NewLocation == Game1.getLocationFromName("Aviroen.VoidsentCP_Morabyr")) 
            {
                //VoidsentEvents.Introduction();
            }
        }

        public void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            //var ftmApi = Helper.ModRegistry.GetApi<IFtmApi>("Esca.FarmTypeManager")!;
        }
        private bool ValidateInstall()
        {
            IModInfo? contentPack = this.Helper.ModRegistry.Get("Aviroen.VoidsentCP");
            IModInfo? contentCheck = this.Helper.ModRegistry.Get("ApryllForever.PolyamorySweetLove");
            if (contentPack is null)
            {
                this.Monitor.Log("Voidsent is missing the Content Pack. Please delete and reinstall the mod to fix.", LogLevel.Error);
                return false;
            }
            if (contentCheck != null)
            {
                this.Monitor.Log("Voidsent is incompatible with Polyamory Sweet. Voidsent will not be loaded.", LogLevel.Error);
                return false;
            }
            return true;
        }
    }
}