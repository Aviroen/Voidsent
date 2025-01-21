using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace Voidsent;

[HarmonyPatch(typeof(StardewValley.Object))]
[HarmonyPatch(nameof(StardewValley.Object.placementAction))]
public static class TentPatch
{
    private static IMonitor Monitor { get; set; }
    private static string ModID { get; set; }

    internal static void Initialize(IMonitor monitor, string modID)
    {
        Monitor = monitor;
        ModID = modID;
    }

    static bool LocationCheck(GameLocation loc)
    {
        var location = loc.GetData();
        return location.CustomFields is not null && 
               location.CustomFields.TryGetValue($"{ModID}_UnsafeForTent", out string unsafeForTent) && 
               unsafeForTent == "True";
    }

    static void ShowMessage()
    {
        Game1.showRedMessage("It's not safe to sleep here.");
    }
    
    /*
     * Original IL:
     *
     ldloc.0
     ldfld class StardewValley.GameLocation StardewValley.Object/'<>c__DisplayClass401_0'::location
     brfalse.s label1
     
     ldloc.0
     ...

     *
     * Patched IL:
     *
     ldloc.0
     ldfld StardewValley.GameLocation StardewValley.<>c__DisplayClass401_0::location
     brfalse.s label1
     
     ldloc.0
     ldfld StardewValley.GameLocation StardewValley.<>c__DisplayClass401_0::location
     call static System.Boolean Voidsent.TentPatch::LocationCheck(StardewValley.GameLocation loc)
     brfalse.s label2
     call static System.Void Voidsent.TentPatch::ShowMessage()
     ldc.i4.0
     ret
     
     [label2] ldloc.0
     ...
     *
     * Original C#:
     * 
     if (location == null || !location.IsOutdoors) ...
     *
     * Patched C# (approximately):
     * 
     if (location != null && LocationCheck(location))
     {
         ShowMessage();
         return false;
     }
     if (location == null || !location.IsOutdoors) ...
     */
    static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        try
        {
            var codeMatcher = new CodeMatcher(instructions, generator);
            codeMatcher
                .MatchStartForward(new CodeMatch(OpCodes.Ldstr, "(O)TentKit"))
                .MatchStartForward(new CodeMatch(OpCodes.Brtrue));
            
            var label = (Label)codeMatcher.Operand;
            codeMatcher
                .SearchForward(instruction => instruction.labels.Contains(label))
                .MatchStartForward(new CodeMatch(OpCodes.Ldfld));
            
            var field = codeMatcher.Operand;
            codeMatcher
                .MatchStartForward(new CodeMatch(OpCodes.Ldloc_0))
                .CreateLabel(out Label skiplabel)
                .Insert(new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldfld, field),
                    CodeInstruction.Call(static (GameLocation loc) => LocationCheck(loc)), // GameLocation : bool
                    new CodeInstruction(OpCodes.Brfalse_S, skiplabel),
                    CodeInstruction.Call(static () => ShowMessage()), // void
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ret));

            return codeMatcher.InstructionEnumeration();
        }
        catch (Exception e)
        {
            Monitor.Log(e.Message, LogLevel.Error);
        }

        return null;
    }
}