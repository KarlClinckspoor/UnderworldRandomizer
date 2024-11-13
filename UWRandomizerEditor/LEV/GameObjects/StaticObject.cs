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
    
    public override byte[] Buffer
    {
        get
        {
            ReconstructBuffer();
            var temp = new List<byte>();
            temp.AddRange(BasicInfoBuffer);
            return temp.ToArray();
        }
        set => value.CopyTo(BasicInfoBuffer, 0);
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

    public StaticObject Clone() => new StaticObject(this.Buffer, this.IdxAtObjectArray);

    public static StaticObject ZeroedOutStaticObject(ushort idx) => new StaticObject() {IdxAtObjectArray = idx};
}