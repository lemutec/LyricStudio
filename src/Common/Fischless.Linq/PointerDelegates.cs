namespace Fischless.Linq;

/// <summary>
/// Action on pointer of unmanaged type.
/// </summary>
/// <param name="pointer">Pointer.</param>
/// <typeparam name="T">Type of value.</typeparam>
public unsafe delegate void PointerAction<T>(T* pointer) where T : unmanaged;

/// <summary>
/// Action on pointers of unmanaged type.
/// </summary>
/// <param name="pointer1">1st pointer.</param>
/// <param name="pointer2">2nd pointer.</param>
/// <typeparam name="T1">Type of 1st value.</typeparam>
/// <typeparam name="T2">Type of 2nd value.</typeparam>
public unsafe delegate void PointerAction<T1, T2>(T1* pointer1, T2* pointer2) where T1 : unmanaged where T2 : unmanaged;

/// <summary>
/// Function on pointer of unmanaged type.
/// </summary>
/// <param name="pointer">Pointer.</param>
/// <typeparam name="T">Type of value.</typeparam>
/// <typeparam name="R">Type of returned value.</typeparam>
public unsafe delegate R PointerInFunc<T, out R>(T* pointer) where T : unmanaged;

/// <summary>
/// Function on pointers of unmanaged type.
/// </summary>
/// <param name="pointer1">1st pointer.</param>
/// <param name="pointer2">2nd pointer.</param>
/// <typeparam name="T1">Type of 1st value.</typeparam>
/// <typeparam name="T2">Type of 2nd value.</typeparam>
/// <typeparam name="R">Type of returned value.</typeparam>
public unsafe delegate R PointerInFunc<T1, T2, out R>(T1* pointer1, T2* pointer2) where T1 : unmanaged where T2 : unmanaged;

/// <summary>
/// Function to return pointer of unmanaged type.
/// </summary>
/// <typeparam name="R">Type of returned value.</typeparam>
public unsafe delegate R* PointerOutFunc<R>() where R : unmanaged;
