using System.Reflection;
using Elements.Core;
using HarmonyLib;
using ResoniteHotReloadLib;
using ResoniteModLoader;
using Snerble.ResoAvatarTools;
using Snerble.Resonite.AvatarTools.Shortcuts;
using Snerble.Utilities;
using Snerble.Utilities.Formatting;
using Snerble.Utilities.Reflection;

#pragma warning disable IDE0051

namespace Snerble.Resonite.AvatarTools;

public class AvatarToolsMod : ResoniteMod
{
    public override string Name => "Avatar Tools";
	public override string Author => "snerble";
	public override string Version { get; } = typeof(AvatarToolsMod).Assembly.GetName().Version!.ToString();

	private const string HarmonyId = "nl.snerble.AvatarTools";
    private static AutoHotReloader autoReloader = null!;

	public override void OnEngineInit()
	{
		Msg("Initializing");

        HotReloader.RegisterForHotReload(this);

		Setup();
	}

    private static void BeforeHotReload() => Unload();
    private static void OnHotReload(ResoniteMod _) => Setup();

    private static void Setup()
    {
        autoReloader = new(typeof(AvatarToolsMod));

        ShortcutConfigurator.RegisterShortcuts();
		
		var harmony = new Harmony(HarmonyId);
		harmony.PatchAll();

		Msg("Initialized");
        UniLog.Flush();
    }

	private static void Unload()
    {
		var harmony = new Harmony(HarmonyId);
        harmony.UnpatchAll(HarmonyId);

		ShortcutConfigurator.UnregisterShortcuts();

		Msg("Unloaded");
	}
}

internal class AutoHotReloader
{
    private readonly FileSystemWatcher watcher = null!;
    private readonly Type modType;
    private volatile int debounce;

    public AutoHotReloader(Type modType)
    {
        this.modType = modType;

        var dllPath = modType.Assembly.Location;
        if (dllPath.IsNullOrEmpty()) return;

        var directory = Path.GetDirectoryName(dllPath)!.Replace("\\", "/");
		var name = Path.GetFileName(dllPath);

        if (!directory.EndsWith("/HotReloadMods")) directory = $"{directory}/HotReloadMods";

        watcher = new(directory, name)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

		watcher.Created += OnFileSystemChange;
		watcher.Changed += OnFileSystemChange;

        Log.Info($"[AutoHotReloader] Watching '{directory}/{name}' for changes");
    }

    private void OnFileSystemChange(object sender, FileSystemEventArgs e)
    {
        var id = Interlocked.Increment(ref debounce);

        Log.Info($"[AutoHotReloader] Assembly update detected ({e.Name})");
        Task.Delay(200).ContinueWith(_ =>
        {
            if (id != debounce) return;

            Log.Info($"[AutoHotReloader] Triggering Hot Reload ({modType})");
            HotReloader.HotReload(modType);
        });
    }
}
