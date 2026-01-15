using FrooxEngine;

namespace Snerble.Resonite.AvatarTools.Extensions;

public static class SlotExtensions
{
	public static Slot FindOrAddSlot(this Slot slot, Predicate<Slot> filter, Action<Slot> configureNew)
	{
		var child = slot.FindChild(filter);
		if (child != null!) return child;

		child = slot.AddSlot();
		configureNew(child);
		return child;
	}

	public static Slot FindOrAddSlot(this Slot slot, string name, Action<Slot>? configureNew = null)
	{
		var child = slot.FindChild(name);
		if (child != null!) return child;

		child = slot.AddSlot(name);
		configureNew?.Invoke(child);
		return child;
	}
}