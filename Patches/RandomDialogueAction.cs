using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
//written by irocendar
namespace Voidsent.Patches;

public static class RandomDialogueAction
{
    private static IModRegistry ModRegistry { get; set; } = null!;
    public static void Initialize(IModRegistry registry)
    {
        ModRegistry = registry;
    }
    /*
     * Syntax: Aviroen.Voidsent_RandomDialogue <ModID> <NPC> <i18ntokenbase> <suffix_options>
     * e.g. Aviroen.Voidsent_RandomDialogue author.testmod Maru test. {{Range:1,5}},OtherKey
     */
    public static bool Action(string[] args, TriggerActionContext context, out string? error)
    {
        if (args.Length < 5)
        {
            error = $"Invalid number of arguments ({args.Length})";
            return false;
        }

        var mod = ModRegistry.Get(args[1]);
        if (mod == null)
        {
            error = $"Invalid content pack or mod ID {args[1]}";
            return false;
        }
        ITranslationHelper translation;
        if (AccessTools.Property(mod.GetType(), "Mod").GetValue(mod) is Mod rawMod)
            translation = rawMod.Helper.Translation;
        else if (AccessTools.Property(mod.GetType(), "ContentPack").GetValue(mod) is IContentPack rawPack)
            translation = rawPack.Translation;
        else
        {
            error = $"Invalid content pack or mod ID {args[1]}.";
            return false;
        }

        var r = Utility.CreateRandom(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);

        var suffixOptions = args.Skip(4).Join(delimiter: " ").Split(",");
        var selected = args[3] + suffixOptions[r.Next(suffixOptions.Length)].Trim();
        var dialogue = translation.Get(selected).ToString().Replace(@"""", @"\""");

        var action = $"Spiderbuttons.BETAS_SetNewDialogue {args[2]} [EscapedText {dialogue}]";

        var npc = Game1.getCharacterFromName(args[2]);
        if (npc is null)
        {
            error = $"NPC with ID ${args[2]} not found.";
            return false;
        }
        if (Game1.activeClickableMenu is StardewValley.Menus.DialogueBox dialogueBox && dialogueBox.characterDialogue.speaker == npc)
        {
            var dialogueObj = new Dialogue(npc, null, $"this is very hacky#$e#{dialogue}");
            dialogueBox.characterDialogue.dialogues.AddRange(dialogueObj.dialogues.Skip(1));

        }
        else
        {
            npc.setNewDialogue(new Dialogue(npc, null, dialogue), false);
        }
        error = null;
        return true;
    }
}