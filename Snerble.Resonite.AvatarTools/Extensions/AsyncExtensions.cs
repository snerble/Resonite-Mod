using FrooxEngine;

namespace Snerble.Resonite.AvatarTools.Extensions;

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