# Resonite modding tips & tricks
This document outlines various things worth keeping in mind while writing mods for resonite.

## Accessing engine constants
To access global manager instances, look for the `Engine` class. Here you can find the `WorldManager`, `GlobalCoroutineManager`, `LocalDB`, etc.

## Async

There are a few variations on performing async work.

### Task Scheduling
In order to modify the world's data model, you must schedule on the world's `CoroutineManager`. You can pass a world element to the `StartTask` method, which will automatically pop the `Task` if the element is destroyed/removed. You can optionally provide an `IUpdatable` to act as a kind of 'domain' on which to execute schedule continuations. If such an element is destroyed, its tasks cease continuation. If a component is not provided, the task is considered 'global' and will run as long as the world is not destroyed.

#### Shorthands
Instances of the `Worker` class offer `StartTask` overloads, which forward the calls to the world `CoroutineManager` and add additional null handling. Tasks scheduled on detached components are designed to dead-end.  
Using these shorthands also simplifies starting global vs non-global tasks with clear overloads.

### Background tasks
True async background tasks can be scheduled on the regular .NET thread pool, but Resonite also offers the `StartBackgroundTask` method. The async delegate passed to this method does not yield to the world sync context, but unlike `ConfigureAwait(false)`, awaiting the result of `StartBackgroundTask` will once again yield to the world.

It's important to note that tasks that yield to worlds are not guaranteed to run because the world can be destroyed. Critical cleanup operations must happen outside of a call to `StartTask`.

### Coroutines
The classic way of scheduling units of work on an update loop exists in the form of enumerator-based coroutines. These can be scheduled using the `StartCoroutine` method. One-off delegates can also be scheduled with a delay using the `RunInSeconds` or `RunInUpdates` methods.

#### Delays
Coroutines can delay their execution by yielding `Context` objects. These can be created from a handful of static constructors, such as `WaitForNextUpdate()`. The `Context.WaitForSeconds` method is equivalent to a `Task.Delay().ConfigureAwait(false)` followed by an `await default(ToWorld)`. Consider using the `Worker.DelaySeconds` method instead of manually using `Task.Delay`

### Switching contexts
Coroutines have the unique ability to switch context on the fly. They can switch from background scheduling (using a `WorkProcessor`) to sync scheduling (part of the update queue), or even delayed reattaching using a `Job`.

This behavior also exists for tasks in the form of awaiting `default(ToWorld)` and `default(ToBackground)`. For background tasks its fine to use `Task.Run` or the `StartBackgroundTask`, but `ToBackground` matches the coroutines best in terms of behavior.

### Yielding
Yielding to the next frame can be done by awaiting `default(NextUpdate)`. Awaiting a specific number of frames can be done by awaiting `new Updates(frameCount)`.

Awaiting `Task.Yield()` is not identical to awaiting `default(NextUpdate)`. While you'd expect both to yield and their continuations to be picked up next frame, however it seems that posting with 0 updates delay may (or will) cause the `CoroutineManager` to pick up the continuation later on the same frame.

### Jobs
The `Job` class appears to be a custom, task-like object that functions much like a standard `TaskCompletionSource`. It does not offer exception handling or cancellation support, nor does it guarantee that it's result it set.

The exception logging actually refers to the `Job` class as `Task`. It's possible that the `Job` class represents a now redundant implementation of promises.

It is **not** recommended to use `Jobs`, unless required by a library function.

### ThreadWorker
Dedicated background tasks that need more isolation than regular async/tasks can be created using the `WorkProcessor`. This class is a custom thread pool with normal and high-priority thread workers.

Consider using the `WorkProcessor` if you need more CPU resources than usual, or if a workload is particularly IO-intensive and should be moved outside of the main .NET thread pool.