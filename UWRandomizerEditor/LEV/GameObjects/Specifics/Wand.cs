namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Wand: StaticObject
{
    private Wand(byte[] buffer, ushort idxAtArray) : base(buffer, idxAtArray) { }
    private Wand(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray) { }

    public Wand(byte[] buffer, ushort idxAtArray, Spell spell) : base(buffer, idxAtArray)
    {
        SpellObject = spell;
        LinkSpecial = SpellObject.IdxAtObjectArray;
    }

    public void SetCharges(byte value)
    {
        SpellObject.Quality = value;
    }

    // What happens if I make a spell point to some other item, like armor?
    public byte SpellCharges
    {
        get => SpellObject.Quality;
        set => SpellObject.Quality = value;
    }

    private GameObject _spellObject;

    public Spell SpellObject
    {
        get => (Spell) _spellObject;
        set => _spellObject = value;
    }

    public void ForceReplaceSpellObject(GameObject obj)
    {
        _spellObject = obj;
    }
}