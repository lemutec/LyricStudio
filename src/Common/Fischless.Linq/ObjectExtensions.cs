﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fischless.Linq;

/// <summary>
/// Extensions for all types.
/// </summary>
public static class ObjectExtensions
{
    // Base class of adapter of weak event handler.
    abstract class BaseWeakEventHandlerAdapter<THandler> : IDisposable where THandler : Delegate
    {
        // Fields.
        private readonly EventInfo eventInfo;

        private readonly THandler handlerStub;
        private readonly WeakReference<THandler> handlerRef;
        private int isDisposed;
        private readonly SynchronizationContext? syncContext;
        private readonly object target;

        // Constructor.
        protected BaseWeakEventHandlerAdapter(object target, string eventName, THandler handler)
        {
            this.eventInfo = target.GetType().GetEvent(eventName) ?? throw new ArgumentException($"Cannot find event '{eventName}' in {target.GetType().Name}.");
            // ReSharper disable VirtualMemberCallInConstructor
            this.handlerStub = this.CreateEventHandlerStub();
            // ReSharper restore VirtualMemberCallInConstructor
            this.handlerRef = new(handler);
            this.syncContext = SynchronizationContext.Current;
            this.target = target;
            this.eventInfo.AddEventHandler(target, this.handlerStub);
        }

        // Called to create stub of event handler.
        protected abstract THandler CreateEventHandlerStub();

        // Dispose.
        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
                return;
            if (this.syncContext != null && this.syncContext != SynchronizationContext.Current)
            {
                try
                {
                    this.syncContext.Post(_ => this.eventInfo.RemoveEventHandler(target, this.handlerStub), null);
                    return;
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                { }
                // ReSharper restore EmptyGeneralCatchClause
            }
            this.eventInfo.RemoveEventHandler(target, this.handlerStub);
        }

        // Invoke event handler.
        protected void InvokeEventHandler(params object?[] args)
        {
            if (this.handlerRef.TryGetTarget(out var handler))
                handler.DynamicInvoke(args);
            else
                this.Dispose();
        }
    }

    // Adapter of weak event handler.
    class WeakActionEventHandlerAdapter : BaseWeakEventHandlerAdapter<Action>
    {
        // Constructor.
        public WeakActionEventHandlerAdapter(object target, string eventName, Action handler) : base(target, eventName, handler)
        { }

        /// <inheritdoc/>.
        protected override Action CreateEventHandlerStub() =>
            this.OnEventReceived;

        // Entry of event handler.
        void OnEventReceived() =>
            this.InvokeEventHandler();
    }

    // Adapter of weak event handler.
    class WeakActionEventHandlerAdapter<TArg> : BaseWeakEventHandlerAdapter<Action<TArg>>
    {
        // Constructor.
        public WeakActionEventHandlerAdapter(object target, string eventName, Action<TArg> handler) : base(target, eventName, handler)
        { }

        /// <inheritdoc/>.
        protected override Action<TArg> CreateEventHandlerStub() =>
            this.OnEventReceived;

        // Entry of event handler.
        void OnEventReceived(TArg arg) =>
            this.InvokeEventHandler(arg);
    }

    // Adapter of weak event handler.
    class WeakActionEventHandlerAdapter<TArg1, TArg2> : BaseWeakEventHandlerAdapter<Action<TArg1, TArg2>>
    {
        // Constructor.
        public WeakActionEventHandlerAdapter(object target, string eventName, Action<TArg1, TArg2> handler) : base(target, eventName, handler)
        { }

        /// <inheritdoc/>.
        protected override Action<TArg1, TArg2> CreateEventHandlerStub() =>
            this.OnEventReceived;

        // Entry of event handler.
        void OnEventReceived(TArg1 arg1, TArg2 arg2) =>
            this.InvokeEventHandler(arg1, arg2);
    }

    // Adapter of weak event handler.
    class WeakActionEventHandlerAdapter<TArg1, TArg2, TArg3> : BaseWeakEventHandlerAdapter<Action<TArg1, TArg2, TArg3>>
    {
        // Constructor.
        public WeakActionEventHandlerAdapter(object target, string eventName, Action<TArg1, TArg2, TArg3> handler) : base(target, eventName, handler)
        { }

        /// <inheritdoc/>.
        protected override Action<TArg1, TArg2, TArg3> CreateEventHandlerStub() =>
            this.OnEventReceived;

        // Entry of event handler.
        void OnEventReceived(TArg1 arg1, TArg2 arg2, TArg3 arg3) =>
            this.InvokeEventHandler(arg1, arg2, arg3);
    }

    // Adapter of weak event handler.
    class WeakEventHandlerAdapter : BaseWeakEventHandlerAdapter<EventHandler>
    {
        // Constructor.
        public WeakEventHandlerAdapter(object target, string eventName, EventHandler handler) : base(target, eventName, handler)
        { }

        /// <inheritdoc/>.
        protected override EventHandler CreateEventHandlerStub() =>
            this.OnEventReceived;

        // Entry of event handler.
        void OnEventReceived(object? sender, EventArgs e) =>
            this.InvokeEventHandler(sender, e);
    }

    // Adapter of weak event handler.
    class WeakEventHandlerAdapter<TArgs> : BaseWeakEventHandlerAdapter<EventHandler<TArgs>> where TArgs : EventArgs
    {
        // Constructor.
        public WeakEventHandlerAdapter(object target, string eventName, EventHandler<TArgs> handler) : base(target, eventName, handler)
        { }

        /// <inheritdoc/>.
        protected override EventHandler<TArgs> CreateEventHandlerStub() =>
            this.OnEventReceived;

        // Entry of event handler.
        void OnEventReceived(object? sender, TArgs e) =>
            this.InvokeEventHandler(sender, e);
    }

    /// <summary>
    /// Add weak event handler.
    /// </summary>
    /// <param name="target"><see cref="object"/>.</param>
    /// <param name="eventName">Name of event.</param>
    /// <param name="handler">Event handler.</param>
    /// <returns><see cref="IDisposable"/> which represents added weak event handler. You can call <see cref="IDisposable.Dispose"/> to remove weak event handler explicitly.</returns>
    public static IDisposable AddWeakEventHandler(this object target, string eventName, EventHandler handler) =>
        new WeakEventHandlerAdapter(target, eventName, handler);

    /// <summary>
    /// Add weak event handler.
    /// </summary>
    /// <param name="target"><see cref="object"/>.</param>
    /// <param name="eventName">Name of event.</param>
    /// <param name="handler">Event handler.</param>
    /// <returns><see cref="IDisposable"/> which represents added weak event handler. You can call <see cref="IDisposable.Dispose"/> to remove weak event handler explicitly.</returns>
    public static IDisposable AddWeakEventHandler<TArgs>(this object target, string eventName, EventHandler<TArgs> handler) where TArgs : EventArgs =>
        new WeakEventHandlerAdapter<TArgs>(target, eventName, handler);

    /// <summary>
    /// Add weak event handler.
    /// </summary>
    /// <param name="target"><see cref="object"/>.</param>
    /// <param name="eventName">Name of event.</param>
    /// <param name="handler">Event handler.</param>
    /// <returns><see cref="IDisposable"/> which represents added weak event handler. You can call <see cref="IDisposable.Dispose"/> to remove weak event handler explicitly.</returns>
    public static IDisposable AddWeakEventHandler(this object target, string eventName, Action handler) =>
        new WeakActionEventHandlerAdapter(target, eventName, handler);

    /// <summary>
    /// Add weak event handler.
    /// </summary>
    /// <param name="target"><see cref="object"/>.</param>
    /// <param name="eventName">Name of event.</param>
    /// <param name="handler">Event handler.</param>
    /// <returns><see cref="IDisposable"/> which represents added weak event handler. You can call <see cref="IDisposable.Dispose"/> to remove weak event handler explicitly.</returns>
    public static IDisposable AddWeakEventHandler<TArg>(this object target, string eventName, Action<TArg> handler) =>
        new WeakActionEventHandlerAdapter<TArg>(target, eventName, handler);

    /// <summary>
    /// Add weak event handler.
    /// </summary>
    /// <param name="target"><see cref="object"/>.</param>
    /// <param name="eventName">Name of event.</param>
    /// <param name="handler">Event handler.</param>
    /// <returns><see cref="IDisposable"/> which represents added weak event handler. You can call <see cref="IDisposable.Dispose"/> to remove weak event handler explicitly.</returns>
    public static IDisposable AddWeakEventHandler<TArg1, TArg2>(this object target, string eventName, Action<TArg1, TArg2> handler) =>
        new WeakActionEventHandlerAdapter<TArg1, TArg2>(target, eventName, handler);

    /// <summary>
    /// Add weak event handler.
    /// </summary>
    /// <param name="target"><see cref="object"/>.</param>
    /// <param name="eventName">Name of event.</param>
    /// <param name="handler">Event handler.</param>
    /// <returns><see cref="IDisposable"/> which represents added weak event handler. You can call <see cref="IDisposable.Dispose"/> to remove weak event handler explicitly.</returns>
    public static IDisposable AddWeakEventHandler<TArg1, TArg2, TArg3>(this object target, string eventName, Action<TArg1, TArg2, TArg3> handler) =>
        new WeakActionEventHandlerAdapter<TArg1, TArg2, TArg3>(target, eventName, handler);

    /// <summary>
    /// Perform action on the given value, and return it.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Action to perform on <paramref name="value"/>.</param>
    /// <returns>Value which is same as <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Also<T>(this T value, RefAction<T> action) where T : struct
    {
        action(ref value);
        return value;
    }

    /// <summary>
    /// Perform action on the given value, and return it.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Action to perform on <paramref name="value"/>.</param>
    /// <returns>Value which is same as <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Also<T>(this T? value, RefAction<T?> action) where T : struct
    {
        action(ref value);
        return value;
    }

    /// <summary>
    /// Perform action on the given value, and return it.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Action to perform on <paramref name="value"/>.</param>
    /// <returns>Value which is same as <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Also<T>(this T value, Action<T> action) where T : class?
    {
        action(value);
        return value;
    }

    /// <summary>
    /// Perform asynchronous action on the given value, and return it.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Asynchronous action to perform on <paramref name="value"/>.</param>
    /// <returns>Task of asynchronous action. The result value which is same as <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T> AlsoAsync<T>(this T value, RefInFunc<T, Task> action) where T : struct
    {
        await action(ref value);
        return value;
    }

    /// <summary>
    /// Perform asynchronous action on the given value, and return it.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Asynchronous action to perform on <paramref name="value"/>.</param>
    /// <returns>Task of asynchronous action. The result value which is same as <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> AlsoAsync<T>(this T? value, RefInFunc<T?, Task> action) where T : struct
    {
        await action(ref value);
        return value;
    }

    /// <summary>
    /// Perform asynchronous action on the given value, and return it.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Asynchronous action to perform on <paramref name="value"/>.</param>
    /// <returns>Task of asynchronous action. The result value which is same as <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T> AlsoAsync<T>(this T value, Func<T, Task> action) where T : class?
    {
        await action(value);
        return value;
    }

    /// <summary>
    /// Treat given nullable value as non-null value, or throw <see cref="NullReferenceException"/>.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="obj">Given nullable value.</param>
    /// <returns>Non-null value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AsNonNull<T>([NotNull] this T? obj) where T : class => obj ?? throw new NullReferenceException();

    /// <summary>
    /// Perform action on the given value.
    /// </summary>
    /// <typeparam name="T">Type of given value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Action to perform on <paramref name="value"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Let<T>(this T value, Action<T> action) => action(value);

    /// <summary>
    /// Perform action on the given value, and return a custom value.
    /// </summary>
    /// <typeparam name="T">Type of given value.</typeparam>
    /// <typeparam name="R">Type of return value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Action to perform on <paramref name="value"/>.</param>
    /// <returns>Custom return value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Let<T, R>(this T value, Func<T, R> action) => action(value);

    /// <summary>
    /// Perform action on the given value, and return a reference to custom variable.
    /// </summary>
    /// <typeparam name="T">Type of given value.</typeparam>
    /// <typeparam name="R">Type of return value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Action to perform on <paramref name="value"/>.</param>
    /// <returns>Reference to custom variable.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref R Let<T, R>(this T value, RefOutFunc<T, R> action) => ref action(value);

    /// <summary>
    /// Perform asynchronous action on the given value.
    /// </summary>
    /// <typeparam name="T">Type of given value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Asynchronous action to perform on <paramref name="value"/>.</param>
    /// <returns>Task of asynchronous action.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task LetAsync<T>(this T value, Func<T, Task> action) => action(value);

    /// <summary>
    /// Perform asynchronous action on the given value, and return a custom value.
    /// </summary>
    /// <typeparam name="T">Type of given value.</typeparam>
    /// <typeparam name="R">Type of return value.</typeparam>
    /// <param name="value">Given value.</param>
    /// <param name="action">Asynchronous action to perform on <paramref name="value"/>.</param>
    /// <returns>Task of asynchronous action.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<R> LetAsync<T, R>(this T value, Func<T, Task<R>> action) => action(value);

    /// <summary>
    /// Acquire lock on given object and perform action before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="action">Action to perform.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Lock<T>(this T obj, Action<T> action) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            action(obj);
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and generate value before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="func">Function to generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Lock<T, R>(this T obj, Func<R> func) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            return func();
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and generate reference to value before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="func">Function to generate reference to value.</param>
    /// <returns>Generated reference to value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref R Lock<T, R>(this T obj, RefOutFunc<R> func) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            return ref func();
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and generate value before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="func">Function to generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Lock<T, R>(this T obj, Func<T, R> func) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            return func(obj);
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and generate reference to value before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="func">Function to generate reference to value.</param>
    /// <returns>Generated reference to value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref R Lock<T, R>(this T obj, RefOutFunc<T, R> func) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            return ref func(obj);
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and perform asynchronous action before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="action">Action to perform.</param>
    /// <returns>Task of asynchronous action to generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task LockAsync<T>(this T obj, Func<T, Task> action) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            await action(obj);
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and generate value asynchronously before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="func">Function to generate value.</param>
    /// <returns>Task of generating value asynchronously.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<R> LockAsync<T, R>(this T obj, Func<Task<R>> func) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            return await func();
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Acquire lock on given object and generate value asynchronously before releasing lock.
    /// </summary>
    /// <typeparam name="T">Type of given object.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="obj">Object to acquire lock on.</param>
    /// <param name="func">Function to generate value.</param>
    /// <returns>Task of generating value asynchronously.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<R> LockAsync<T, R>(this T obj, Func<T, Task<R>> func) where T : class
    {
        Monitor.Enter(obj);
        try
        {
            return await func(obj);
        }
        finally
        {
            Monitor.Exit(obj);
        }
    }

    /// <summary>
    /// Try casting given object to target type then perform the action.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="action">Action to perform if object can be casted to target type.</param>
    /// <typeparam name="T">Target type.</typeparam>
    /// <returns>True if object can be casted to target type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryCastAndRun<T>(this object? obj, Action<T> action)
    {
        if (obj is T target)
        {
            action(target);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Try casting given object to target type then generate a value.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="func">Function to generate value if object can be casted to target type.</param>
    /// <typeparam name="T">Target type.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R? TryCastAndRun<T, R>(this object? obj, Func<T, R> func)
    {
        if (obj is T target)
            return func(target);
        return default;
    }

    /// <summary>
    /// Try casting given object to target type then perform an asynchronous action.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="action">Action to perform if object can be casted to target type.</param>
    /// <typeparam name="T">Target type.</typeparam>
    /// <returns>Task of performing action. The result is True if object can be casted to target type.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<bool> TryCastAndRunAsync<T>(this object? obj, Func<T, Task> action)
    {
        if (obj is T target)
        {
            await action(target);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Try casting given object to target type then generate a value asynchronously.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="func">Function to generate value if object can be casted to target type.</param>
    /// <typeparam name="T">Target type.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <returns>Task of generating value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<R> TryCastAndRunAsync<T, R>(this object? obj, Func<T, Task<R>> func)
    {
        if (obj is T target)
            return func(target);
#pragma warning disable CS8604
        return Task.FromResult<R>(default);
#pragma warning restore CS8604
    }
}
