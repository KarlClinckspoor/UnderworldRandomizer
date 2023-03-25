namespace UWRandomizerEditor.Interfaces;

/// <summary>
/// Interface that consolidates the behavior of object that contain Buffers. These objects should have a byte buffer,
/// and a method to reconstruct it, meaning if any property was changed (e.g., Door lock status), then the buffer has to
/// be updated.
/// </summary>
public interface IBufferObject
{
    /// <summary>
    /// Byte array containing the bytes that, when interpreted, compose the object. It is used both when loading and object
    /// properties are extracted, and when saving, when member buffers are concatenated to reform the original file buffer.
    /// </summary>
    public byte[] Buffer { get; protected set; }
    public bool ReconstructBuffer();
}