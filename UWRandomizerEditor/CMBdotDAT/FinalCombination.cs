namespace UWRandomizerEditor.CMBdotDAT;

public class FinalCombination : ItemCombination
{
    /// <summary>
    /// A final combination, by itself, is always valid. The file itself should check if it's in last place.
    /// </summary>
    /// <returns></returns>
    public override bool IsValidItemCombination() => true;

    public FinalCombination() : base(new FinalDescriptor(), new FinalDescriptor(), new FinalDescriptor())
    {
    }
}
