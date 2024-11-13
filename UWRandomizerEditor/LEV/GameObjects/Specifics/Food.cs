namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Food: QuantityGameObject
{
    public Food(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Food(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }

    public FoodFreshness Freshness
    {
        get
        {
            // TODO: test this out.
            return Quality switch
            {
                < 1 * 0b111111 / 5 => FoodFreshness.Worst,
                < 2 * 0b111111 / 5 => FoodFreshness.Bad,
                < 3 * 0b111111 / 5 => FoodFreshness.Medium,
                < 4 * 0b111111 / 5 => FoodFreshness.Good,
                _ => FoodFreshness.Best
            };
        }
        set
        {
           // TODO: fill in
           // Convert freshness enum to quality
           // set the variable
           Quality = (byte) value;
        }
    }

}

// TODO: fill in
public enum FoodFreshness: byte
{
    Worst,
    Bad,
    Medium,
    Good,
    Best
}