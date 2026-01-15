using System.Reflection;
using FrooxEngine;
using HarmonyLib;
using Snerble.ResoAvatarTools;
using Snerble.Utilities;

namespace Snerble.Resonite.AvatarTools.Shortcuts;

[HarmonyPatch]
public static class ShortcutConfigurator
{
    private record ShortcutDescriptor(Type ShortcutType, Type ContextType, Func<object, IShortcut> Factory, int Order)
    {
        public bool TargetsComponent { get; } = typeof(Component).IsAssignableFrom(ContextType);
        public bool TargetsSlot { get; } = typeof(Slot) == ContextType;
    }

    private record ShortcutAction(ShortcutDescriptor Descriptor, object Context)
    {
		public IShortcut CreateInstance() => Descriptor.Factory(Context);
    }

    private static readonly List<ShortcutDescriptor> Shortcuts = [];

	public static void RegisterShortcuts()
	{
		var assembly = typeof(AvatarToolsMod).Assembly;

		foreach (var type in assembly.GetTypes().Where(x => !x.IsAbstract))
        {
            Shortcuts.AddRange(GetShortcutDescriptors(type));
        }

		Shortcuts.Sort((x, y) => x.Order.CompareTo(y.Order));
		Shortcuts.ForEach(x => Log.Info($"Registered {x.ShortcutType.FullName} as a context shortcut for {x.ContextType.GetFullName()}"));
	}

	public static void UnregisterShortcuts() => Shortcuts.Clear();

	private static IEnumerable<ShortcutDescriptor> GetShortcutDescriptors(Type type)
    {
		// Only IShortcut subclasses
        if (!typeof(IShortcut).IsAssignableFrom(type)) yield break;

		// Public constructors only
        var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        foreach (var ctor in ctors)
        {
            // Constructor with 1 argument
            if (ctor.GetParameters() is not { Length: 1 } parameters) continue;

            // Single parameter must be an IWorldElement
            var param = parameters[0];
			if (!typeof(IWorldElement).IsAssignableFrom(param.ParameterType)) continue;
			
            yield return new(type, param.ParameterType, arg => (IShortcut)ctor.Invoke([arg]), 0);
        }
	}

	[HarmonyPatch(typeof(DevTool), "GenerateMenuItems"), HarmonyPostfix]
	internal static void AddGenerateToggleMenu(DevTool __instance, InteractionHandler tool, ContextMenu menu)
	{
		if (Shortcuts.Count == 0) return;

		var handler = __instance.ActiveHandler;
		var held = handler.Grabber.GrabbedObjects.FirstOrDefault();

		if (held == null) return;

		List<ShortcutAction> actions = [];

		// Add from held reference proxy
		if (held.Slot.GetComponent<ReferenceProxy>() is { } syncRef)
		{
			actions.AddRange(
				from shortcut in Shortcuts
				where shortcut.ContextType.IsInstanceOfType(syncRef.Reference.Target)
				select new ShortcutAction(shortcut, syncRef.Reference.Target));

			// Find shortcuts that can run on the slot's components
			if (syncRef.Reference.Target is Slot heldSlot)
			{
				actions.AddRange(
					from shortcut in Shortcuts where shortcut.TargetsComponent
					from component in heldSlot.EnumerateComponents(shortcut.ContextType)
					select new ShortcutAction(shortcut, component));
			}
		}
		// Add from held object
		else
		{
			actions.AddRange(
				from shortcut in Shortcuts where shortcut.TargetsSlot
				select new ShortcutAction(shortcut, held.Slot));

			actions.AddRange(
				from shortcut in Shortcuts where shortcut.TargetsComponent
				from component in held.Slot.EnumerateComponents(shortcut.ContextType)
				select new ShortcutAction(shortcut, component));
		}

		foreach (var action in actions.OrderBy(x => x.Descriptor.Order))
        {
            var shortcut = action.CreateInstance();
			var menuItem = menu.AddItem(shortcut.Name, (Uri)null!, shortcut.Color);
			menuItem.Button.LocalPressed += (_, _) => shortcut.Run();
		}
	}
}