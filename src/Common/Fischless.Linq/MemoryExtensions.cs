﻿using System;
using System.Runtime.CompilerServices;

namespace Fischless.Linq;

/// <summary>
/// Extensions for <see cref="Memory{T}"/>.
/// </summary>
public static unsafe class MemoryExtensions
{
    /// <summary>
    /// Check whether given character sequence is empty or contains whitespaces only or not.
    /// </summary>
    /// <param name="s">Character sequence to check.</param>
    /// <returns>True if the character sequence is empty or contains whitespaces only.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyOrWhiteSpace(this Memory<char> s) =>
        s.PinAs((char* cPtr) =>
        {
            if (cPtr is null)
                return true;
            var count = s.Length;
            while (count > 0)
            {
                if (!char.IsWhiteSpace(*cPtr))
                    return false;
                ++cPtr;
                --count;
            }
            return true;
        });

    /// <summary>
    /// Check whether given character sequence is empty or contains whitespaces only or not.
    /// </summary>
    /// <param name="s">Character sequence to check.</param>
    /// <returns>True if the character sequence is empty or contains whitespaces only.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptyOrWhiteSpace(this ReadOnlyMemory<char> s) =>
        s.PinAs((char* cPtr) =>
        {
            if (cPtr is null)
                return true;
            var count = s.Length;
            while (count > 0)
            {
                if (!char.IsWhiteSpace(*cPtr))
                    return false;
                ++cPtr;
                --count;
            }
            return true;
        });

    /// <summary>
    /// Check whether given <see cref="Memory{T}"/> contains data or not.
    /// </summary>
    /// <param name="memory"><see cref="Memory{T}"/> to check.</param>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <returns>True if <see cref="Memory{T}"/> contains data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(this Memory<T> memory) =>
        memory.Length > 0;

    /// <summary>
    /// Check whether given <see cref="ReadOnlyMemory{T}"/> contains data or not.
    /// </summary>
    /// <param name="memory"><see cref="ReadOnlyMemory{T}"/> to check.</param>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <returns>True if <see cref="ReadOnlyMemory{T}"/> contains data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(this ReadOnlyMemory<T> memory) =>
        memory.Length > 0;

    /// <summary>
    /// Check whether given character sequence contains at least one non-whitespace or not.
    /// </summary>
    /// <param name="s">Character sequence to check.</param>
    /// <returns>True if the character sequence contains contains at least one non-whitespace.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotWhiteSpace(this Memory<char> s) =>
        s.PinAs((char* cPtr) =>
        {
            if (cPtr is null)
                return false;
            var count = s.Length;
            while (count > 0)
            {
                if (!char.IsWhiteSpace(*cPtr))
                    return true;
                ++cPtr;
                --count;
            }
            return false;
        });

    /// <summary>
    /// Check whether given character sequence contains at least one non-whitespace or not.
    /// </summary>
    /// <param name="s">Character sequence to check.</param>
    /// <returns>True if the character sequence contains contains at least one non-whitespace.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotWhiteSpace(this ReadOnlyMemory<char> s) =>
        s.PinAs((char* cPtr) =>
        {
            if (cPtr is null)
                return false;
            var count = s.Length;
            while (count > 0)
            {
                if (!char.IsWhiteSpace(*cPtr))
                    return true;
                ++cPtr;
                --count;
            }
            return false;
        });

    /// <summary>
    /// Pin given <see cref="Memory{T}"/>, get address of memory and perform action.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <param name="memory"><see cref="Memory{T}"/>.</param>
    /// <param name="action">Method to receive address of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pin<T>(this Memory<T> memory, Action<IntPtr> action)
    {
        using var handle = memory.Pin();
        action(new IntPtr(handle.Pointer));
    }

    /// <summary>
    /// Pin given list of <see cref="Memory{T}"/>, get addresses of memory and perform action.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <param name="memoryList">List of <see cref="Memory{T}"/>.</param>
    /// <param name="action">Method to receive addresses of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pin<T1, T2>(this (Memory<T1>, Memory<T2>) memoryList, Action<IntPtr, IntPtr> action)
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        action(new IntPtr(handle1.Pointer), new IntPtr(handle2.Pointer));
    }

    /// <summary>
    /// Pin given <see cref="Memory{T}"/>, get address of memory and generate a value.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memory"><see cref="Memory{T}"/>.</param>
    /// <param name="func">Function to receive address of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Pin<T, R>(this Memory<T> memory, Func<IntPtr, R> func)
    {
        using var handle = memory.Pin();
        return func(new IntPtr(handle.Pointer));
    }

    /// <summary>
    /// Pin given list of <see cref="Memory{T}"/>, get addresses of memory and generate a value.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memoryList">List of <see cref="Memory{T}"/>.</param>
    /// <param name="func">Function to receive addresses of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Pin<T1, T2, R>(this (Memory<T1>, Memory<T2>) memoryList, Func<IntPtr, IntPtr, R> func)
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        return func(new IntPtr(handle1.Pointer), new IntPtr(handle2.Pointer));
    }

    /// <summary>
    /// Pin given <see cref="ReadOnlyMemory{T}"/>, get address of memory and perform action.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <param name="memory"><see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="action">Method to receive address of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pin<T>(this ReadOnlyMemory<T> memory, Action<IntPtr> action)
    {
        using var handle = memory.Pin();
        action(new IntPtr(handle.Pointer));
    }

    /// <summary>
    /// Pin given list of <see cref="ReadOnlyMemory{T}"/>, get addresses of memory and perform action.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <param name="memoryList">List of <see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="action">Method to receive addresses of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pin<T1, T2>(this (ReadOnlyMemory<T1>, ReadOnlyMemory<T2>) memoryList, Action<IntPtr, IntPtr> action)
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        action(new IntPtr(handle1.Pointer), new IntPtr(handle2.Pointer));
    }

    /// <summary>
    /// Pin given <see cref="ReadOnlyMemory{T}"/>, get address of memory and generate a value.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memory"><see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="func">Function to receive address of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Pin<T, R>(this ReadOnlyMemory<T> memory, Func<IntPtr, R> func)
    {
        using var handle = memory.Pin();
        return func(new IntPtr(handle.Pointer));
    }

    /// <summary>
    /// Pin given list of <see cref="ReadOnlyMemory{T}"/>, get addresses of memory and generate a value.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memoryList">List of <see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="func">Function to receive addresses of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R Pin<T1, T2, R>(this (ReadOnlyMemory<T1>, ReadOnlyMemory<T2>) memoryList, Func<IntPtr, IntPtr, R> func)
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        return func(new IntPtr(handle1.Pointer), new IntPtr(handle2.Pointer));
    }

    /// <summary>
    /// Pin given <see cref="Memory{T}"/>, get address of memory and perform action.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <typeparam name="TPtr">Type of pointer of pinned memory element.</typeparam>
    /// <param name="memory"><see cref="Memory{T}"/>.</param>
    /// <param name="action">Method to receive address of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PinAs<T, TPtr>(this Memory<T> memory, PointerAction<TPtr> action) where TPtr : unmanaged
    {
        using var handle = memory.Pin();
        action((TPtr*)handle.Pointer);
    }

    /// <summary>
    /// Pin given list of <see cref="Memory{T}"/>, get addresses of memory and perform action.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <typeparam name="TPtr1">Type of 1st pointer of pinned memory element.</typeparam>
    /// <typeparam name="TPtr2">Type of 2nd pointer of pinned memory element.</typeparam>
    /// <param name="memoryList">List of <see cref="Memory{T}"/>.</param>
    /// <param name="action">Method to receive addresses of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PinAs<T1, T2, TPtr1, TPtr2>(this (Memory<T1>, Memory<T2>) memoryList, PointerAction<TPtr1, TPtr2> action) where TPtr1 : unmanaged where TPtr2 : unmanaged
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        action((TPtr1*)handle1.Pointer, (TPtr2*)handle2.Pointer);
    }

    /// <summary>
    /// Pin given <see cref="Memory{T}"/>, get address of memory and generate a value.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <typeparam name="TPtr">Type of pointer of pinned memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memory"><see cref="Memory{T}"/>.</param>
    /// <param name="func">Function to receive address of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R PinAs<T, TPtr, R>(this Memory<T> memory, PointerInFunc<TPtr, R> func) where TPtr : unmanaged
    {
        using var handle = memory.Pin();
        return func((TPtr*)handle.Pointer);
    }

    /// <summary>
    /// Pin given list of <see cref="Memory{T}"/>, get addresses of memory and generate a value.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <typeparam name="TPtr1">Type of 1st pointer of pinned memory element.</typeparam>
    /// <typeparam name="TPtr2">Type of 2nd pointer of pinned memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memoryList">List of <see cref="Memory{T}"/>.</param>
    /// <param name="func">Function to receive addresses of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R PinAs<T1, T2, TPtr1, TPtr2, R>(this (Memory<T1>, Memory<T2>) memoryList, PointerInFunc<TPtr1, TPtr2, R> func) where TPtr1 : unmanaged where TPtr2 : unmanaged
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        return func((TPtr1*)handle1.Pointer, (TPtr2*)handle2.Pointer);
    }

    /// <summary>
    /// Pin given <see cref="ReadOnlyMemory{T}"/>, get address of memory and perform action.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <typeparam name="TPtr">Type of pointer of pinned memory element.</typeparam>
    /// <param name="memory"><see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="action">Method to receive address of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PinAs<T, TPtr>(this ReadOnlyMemory<T> memory, PointerAction<TPtr> action) where TPtr : unmanaged
    {
        using var handle = memory.Pin();
        action((TPtr*)handle.Pointer);
    }

    /// <summary>
    /// Pin given list of <see cref="ReadOnlyMemory{T}"/>, get addresses of memory and perform action.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <typeparam name="TPtr1">Type of 1st pointer of pinned memory element.</typeparam>
    /// <typeparam name="TPtr2">Type of 2nd pointer of pinned memory element.</typeparam>
    /// <param name="memoryList">List of <see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="action">Method to receive addresses of memory and perform action.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PinAs<T1, T2, TPtr1, TPtr2>(this (ReadOnlyMemory<T1>, ReadOnlyMemory<T2>) memoryList, PointerAction<TPtr1, TPtr2> action) where TPtr1 : unmanaged where TPtr2 : unmanaged
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        action((TPtr1*)handle1.Pointer, (TPtr2*)handle2.Pointer);
    }

    /// <summary>
    /// Pin given <see cref="ReadOnlyMemory{T}"/>, get address of memory and generate a value.
    /// </summary>
    /// <typeparam name="T">Type of memory element.</typeparam>
    /// <typeparam name="TPtr">Type of pointer of pinned memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memory"><see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="func">Function to receive address of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R PinAs<T, TPtr, R>(this ReadOnlyMemory<T> memory, PointerInFunc<TPtr, R> func) where TPtr : unmanaged
    {
        using var handle = memory.Pin();
        return func((TPtr*)handle.Pointer);
    }

    /// <summary>
    /// Pin given list of <see cref="ReadOnlyMemory{T}"/>, get addresses of memory and generate a value.
    /// </summary>
    /// <typeparam name="T1">Type of 1st memory element.</typeparam>
    /// <typeparam name="T2">Type of 2nd memory element.</typeparam>
    /// <typeparam name="TPtr1">Type of 1st pointer of pinned memory element.</typeparam>
    /// <typeparam name="TPtr2">Type of 2nd pointer of pinned memory element.</typeparam>
    /// <typeparam name="R">Type of generated value.</typeparam>
    /// <param name="memoryList">List of <see cref="ReadOnlyMemory{T}"/>.</param>
    /// <param name="func">Function to receive addresses of memory and generate value.</param>
    /// <returns>Generated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static R PinAs<T1, T2, TPtr1, TPtr2, R>(this (ReadOnlyMemory<T1>, ReadOnlyMemory<T2>) memoryList, PointerInFunc<TPtr1, TPtr2, R> func) where TPtr1 : unmanaged where TPtr2 : unmanaged
    {
        using var handle1 = memoryList.Item1.Pin();
        using var handle2 = memoryList.Item2.Pin();
        return func((TPtr1*)handle1.Pointer, (TPtr2*)handle2.Pointer);
    }
}
