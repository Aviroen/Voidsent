// SPDX-License-Identifier: MIT
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Triggers;

namespace Voidsent.Integration;

/// <summary>
/// Model of data that will go into global app data.
/// Add more flags/strings by adding more properties.
/// </summary>
public class FlagsModel
{
    public ISemanticVersion Version = new SemanticVersion(1, 0, 0);
    public bool BeenHereBefore { get; set; } = false;
    public string ThePriorYou { get; set; } = "";
}

/// <summary>
/// Content Patcher complex token handler
/// </summary>
/// <param name="reflection"></param>
/// <param name="getFlags"></param>
public class PersistentFlagsCPToken(Func<FlagsModel> getFlags)
{
    private bool hasChanged = false;
    private PropertyInfo[]? flagsProps = null;
    private Dictionary<string, string?> cachedValues = [];

    internal bool SetHasChanged(bool value) => hasChanged = value;

    public bool AllowsInput() => true;

    public bool RequiresInput() => true;

    public bool CanHaveMultipleValues(string? input = null) => false;

    public bool UpdateContext()
    {
        if (hasChanged && getFlags() is FlagsModel flagsModel)
        {
            flagsProps ??= typeof(FlagsModel).GetProperties();
            foreach (var prop in flagsProps)
            {
                if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(string))
                    cachedValues[prop.Name] = prop.GetValue(flagsModel)?.ToString() ?? null;
            }
            hasChanged = false;
            return true;
        }
        return false;
    }

    public bool IsReady() => getFlags() != null;

    public bool TryValidateInput(string? input, [NotNullWhen(false)] out string? error)
    {
        if (input == null)
        {
            error = "Input required";
            return false;
        }
        error = "no error";
        if (getFlags() is FlagsModel flagsModel)
        {
            flagsProps ??= typeof(FlagsModel).GetProperties();
            return flagsProps.Any(prop => prop.Name == input);
        }
        error = $"No flag or string with name {input} found";
        return false;
    }

    public IEnumerable<string> GetValues(string? input)
    {
        if (input == null)
        {
            return [];
        }
        if (cachedValues.TryGetValue(input, out string? flagValue) && flagValue != null)
        {
            return [flagValue];
        }
        return [];
    }
}

/// <summary>
/// Manages persistent tokens.
/// Initialize once in ModEntry.Entry
/// </summary>
public class PersistentFlagsManager
{
    private const string DATA_KEY = "PersistentFlags";
    private string GSQ_PERSISTENT_FLAG => $"{man.UniqueID}_PERSISTENT_FLAG";
    private string GSQ_PERSISTENT_STRING => $"{man.UniqueID}_PERSISTENT_STRING";
    private string Action_SetPersistentFlag => $"{man.UniqueID}_SetPersistentFlag";
    private string Action_SetPersistentString => $"{man.UniqueID}_SetPersistentString";
    private readonly IModHelper helper;
    private readonly IManifest man;
    private readonly IMonitor mon;

    private FlagsModel? flags;

    public FlagsModel GetFlags() => flags ??= Load();

    public PersistentFlagsCPToken? cpToken = null;

    public PersistentFlagsManager(IModHelper helper, IManifest man, IMonitor mon)
    {
        this.helper = helper;
        this.man = man;
        this.mon = mon;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.ConsoleCommands.Add(
            "clear_persistent_flags",
            "Clear all persistent flags.",
            ConsoleClearPersistentFlags
        );
    }

    public FlagsModel Load()
    {
        FlagsModel? data = helper.Data.ReadGlobalData<FlagsModel>(DATA_KEY);
        data ??= new FlagsModel();
        data.Version = man.Version;
        helper.Data.WriteGlobalData(DATA_KEY, data);
        cpToken?.SetHasChanged(true);
        return data;
    }

    public void Save()
    {
        helper.Data.WriteGlobalData(DATA_KEY, GetFlags());
        cpToken?.SetHasChanged(true);
    }

    private void ConsoleClearPersistentFlags(string arg1, string[] arg2)
    {
        if (Context.IsGameLaunched)
        {
            flags = null;
            helper.Data.WriteGlobalData<FlagsModel>(DATA_KEY, null);
            mon.Log("Cleared persistent flags");
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (GetFlags() == null)
        {
            mon.Log("Fatal error: failed to read persistent flags, give up.", LogLevel.Error);
            return;
        }

        // Register GSQ and trigger for read/write

        // PERSISTENT_FLAG -> true/false bool type
        GameStateQuery.Register(GSQ_PERSISTENT_FLAG, PERSISTENT_FLAG);
        TriggerActionManager.RegisterAction(Action_SetPersistentFlag, SetPersistentFlag);

        // PERSISTENT_STRING -> string type
        GameStateQuery.Register(GSQ_PERSISTENT_STRING, PERSISTENT_STRING);
        TriggerActionManager.RegisterAction(Action_SetPersistentString, SetPersistentString);

        // Register content patcher token
        if (helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher") is IContentPatcherAPI cpAPI)
        {
            cpToken = new PersistentFlagsCPToken(GetFlags);
            cpAPI.RegisterToken(man, "Persistent", cpToken);
        }
    }

    private bool PERSISTENT_FLAG(string[] query, GameStateQueryContext context)
    {
        if (
            !ArgUtility.TryGet(
                query,
                1,
                out string pflag,
                out string error,
                allowBlank: false,
                name: "string persistentFlag"
            )
        )
        {
            mon.Log(error, LogLevel.Error);
            return false;
        }
        return helper.Reflection.GetProperty<bool>(GetFlags(), pflag, false)?.GetValue() ?? false;
    }

    private bool PERSISTENT_STRING(string[] query, GameStateQueryContext context)
    {
        if (
            !ArgUtility.TryGet(
                query,
                1,
                out string pFlag,
                out string error,
                allowBlank: false,
                name: "string persistentFlag"
            )
            || !ArgUtility.TryGet(query, 2, out string pString, out error, allowBlank: false, name: "string flagValue")
        )
        {
            mon.Log(error, LogLevel.Error);
            return false;
        }
        if (helper.Reflection.GetProperty<string>(GetFlags(), pFlag, false) is IReflectedProperty<string> prop)
        {
            return prop.GetValue().EqualsIgnoreCase(pString);
        }
        return false;
    }

    private bool SetPersistentFlag(string[] args, TriggerActionContext context, out string error)
    {
        if (
            !ArgUtility.TryGet(args, 1, out string pflag, out error, allowBlank: false, name: "string persistentFlag")
            || !ArgUtility.TryGetBool(args, 2, out bool value, out error, name: "bool value")
        )
        {
            return false;
        }
        if (helper.Reflection.GetProperty<bool>(GetFlags(), pflag, false) is IReflectedProperty<bool> prop)
        {
            prop.SetValue(value);
            Save();
            return true;
        }
        return false;
    }

    private bool SetPersistentString(string[] args, TriggerActionContext context, out string error)
    {
        if (
            !ArgUtility.TryGet(args, 1, out string pflag, out error, allowBlank: false, name: "string persistentFlag")
            || !ArgUtility.TryGet(args, 2, out string value, out error, allowBlank: false, name: "bool value")
        )
        {
            return false;
        }
        if (helper.Reflection.GetProperty<string>(GetFlags(), pflag, false) is IReflectedProperty<string> prop)
        {
            prop.SetValue(value);
            Save();
            return true;
        }
        return false;
    }
}
