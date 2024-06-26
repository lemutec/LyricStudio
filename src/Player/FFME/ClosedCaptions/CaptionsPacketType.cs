﻿namespace FFME.ClosedCaptions;

/// <summary>
/// Defines Closed-Captioning Packet types.
/// </summary>
public enum CaptionsPacketType
{
    /// <summary>
    /// The unrecognized packet type.
    /// </summary>
    Unrecognized,

    /// <summary>
    /// The null pad packet type.
    /// </summary>
    NullPad,

    /// <summary>
    /// The XDS class packet type.
    /// </summary>
    XdsClass,

    /// <summary>
    /// The misc command packet type.
    /// </summary>
    Command,

    /// <summary>
    /// The text packet type.
    /// </summary>
    Text,

    /// <summary>
    /// The mid row packet type.
    /// </summary>
    MidRow,

    /// <summary>
    /// The preamble packet type.
    /// </summary>
    Preamble,

    /// <summary>
    /// The color packet type.
    /// </summary>
    Color,

    /// <summary>
    /// The charset packet type.
    /// </summary>
    PrivateCharset,

    /// <summary>
    /// The tabs packet type
    /// Section B.4 Tab Offsets.
    /// </summary>
    Tabs
}
