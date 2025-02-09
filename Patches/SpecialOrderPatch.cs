using StardewValley.SpecialOrders;
using StardewValley;

namespace Voidsent.Patches
{
    internal class SpecialOrderPatch
    {
        //https://github.com/rokugin/Collector/blob/main/Patches/SpecialOrderPatch.cs#L14
        public static void IsTimedQuest_Postfix(ref bool __result, SpecialOrder __instance)
        {
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
                if (__instance.GetData().CustomFields.TryGetValue("Aviroen.SOQuest", out string? customString))
                {
                    __result = false;
                }
            }
        }
        public static void GetDaysLeft_Postfix(ref int __result, SpecialOrder __instance)
        {
            if (__instance.GetData().CustomFields.TryGetValue("Aviroen.SOQuest", out string? customString))
            {
                __result = 100;
            }
        }
    }
}
