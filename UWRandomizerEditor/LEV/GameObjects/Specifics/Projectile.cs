namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Projectile: QuantityGameObject
{
    public Projectile(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Projectile(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}

public class SpecialProjectile : Projectile
{
    public SpecialProjectile(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public SpecialProjectile(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}