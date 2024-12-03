using HarmonyLib;
using StardewValley.TokenizableStrings;
using StardewValley;

namespace Voidsent.Patches;
[HarmonyPatch]
public class HarmonyPatches
{
    //[HarmonyPatch(typeof(GameLocation), "ShowLockedDoorMessage")]
    //many thanks to rokugin for helping set this one up
    public static bool Prefix(GameLocation __instance, string[] action)
    {
        if (Voidsent.myLocations.Contains(__instance))
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
}
