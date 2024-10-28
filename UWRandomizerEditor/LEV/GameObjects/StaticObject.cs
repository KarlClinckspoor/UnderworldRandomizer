namespace UWRandomizerEditor.LEV.GameObjects;

/// <summary>
/// This is essentially the same as GameObject, but I decided to have it separate for semantic reasons.
/// </summary>
public class StaticObject : GameObject
{
    public override bool ReconstructBuffer()
    {
        return true;
    }

    public StaticObject(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
        if (buffer.Length != FixedBufferLength)
        {
            throw new ArgumentException(
                $"Length of buffer ({buffer.Length}) is incompatible with GameObject length ({FixedBufferLength})");
        }
        buffer.CopyTo(BasicInfoBuffer, 0);
        IdxAtObjectArray = idxAtObjArray;
    }

    public StaticObject(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray)
        : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray) { }
    
    protected StaticObject() { }
}