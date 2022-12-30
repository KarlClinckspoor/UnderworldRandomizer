namespace UWRandomizerEditor.Interfaces;

/// <summary>
/// Interface that consolidates the behavior of object that contain Buffers. These objects should have a byte buffer,
/// and a method to reconstruct it, meaning if any property was changed (e.g., Door lock status), then the buffer has to
/// be updated.
/// </summary>
public interface IBufferObject
{
    public byte[] Buffer { get; protected set; }
    public bool ReconstructBuffer();
}