using System.Runtime.CompilerServices;
using FrooxEngine;

namespace Snerble.Resonite.AvatarTools.Extensions;

public static class AttachComponentExtensions
{
    // ReSharper disable once InconsistentNaming
    extension(Slot worker)
    {
        /// <inheritdoc cref="ContainerWorker{C}.AttachComponent"/>
        [OverloadResolutionPriority(1)]
        public T AttachComponent<T>(Action<T> beforeAttach) where T : Component, new() => worker.AttachComponent(true, beforeAttach);
    }
}