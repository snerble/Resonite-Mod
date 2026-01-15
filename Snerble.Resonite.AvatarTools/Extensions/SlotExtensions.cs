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

public static class AsyncExtensions
{
    extension(Task)
    {
		/// <summary>
		/// Yields back to the current <see cref="CoroutineManager"/> on the next update.
		/// </summary>
        public static NextUpdate NextUpdate() => default;

        /// <summary>
        /// Yields back to the current <see cref="CoroutineManager"/> after <paramref name="n"/> number of updates have passed.
        /// </summary>
        public static Updates Updates(int n) => new(n);
    }

    extension<T>(Tween<T> tween)
    {
        public Task AsTask()
        {
            if (tween.IsRemoved) return Task.CompletedTask;

            var tcs = new TaskCompletionSource();
            tween.LocalCallback.Value = true;
            tween.OnDoneLocal = tcs.SetResult;
            return tcs.Task;
        }
    }
}