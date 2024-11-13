namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Potion: StaticObject, IEnchantable<PotionEnchantment>
{
    public Potion(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public Potion(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }

    public bool IsEnchanted { get; private set; }

    public ushort SpecialIdx
    {
        get => QuantityOrSpecialLinkOrSpecialProperty;
        // TODO: Should't 'LinkSpecial' be 'QuantityOrSpecialLinkOrSpecialProperty'?
        set => LinkSpecial = (ushort)Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
    }

    public void AddEnchantment(PotionEnchantment enchant)
    {
        EnchantmentNumber = ((int)enchant - 256);
        IsEnchanted = true;
    }

    public void ForceAddEnchantment(int value)
    {
        EnchantmentNumber = value;
        IsEnchanted = true;
    }
    
    public IdentificationStatus IDStatus
    {
        get => (IdentificationStatus)(Heading & 0b111);
        set => Heading = (byte) Utils.SetBits(Heading, (byte)value, 0b111, 0);
    }

    public void RemoveEnchantment()
    {
        IsEnchanted = false;
        EnchantmentNumber = 0; // TODO: which value? 0? -1?
    }

    public PotionEnchantment Enchantment => IsEnchanted ? (PotionEnchantment)SpellValue : PotionEnchantment.Nothing;

    public int SpellValue
    {
        get
        {
            if (EnchantmentNumber < 53) //spell effects
                return EnchantmentNumber + 256;
            if (EnchantmentNumber >= 67 && EnchantmentNumber < 70) //Maze, frog, halluc
                return EnchantmentNumber + 144;
            return -1; // TODO: why -1?
        }
    }

    public int EnchantmentNumber
    {
        get => SpecialIdx - 512;
        set => SpecialIdx = (ushort)(value + 512);
    }
}

public enum PotionEnchantment
{
    Light = 256,
    ResistBlows = 257,
    MagicArrow = 258,
    CreateFood = 259,
    Stealth = 260,
    Leap = 261,
    Curse = 262,
    SlowFall = 263,
    LesserHeal = 264,
    DetectMonster = 265,
    CauseFear = 266,
    RuneofWarding = 267,
    Speed = 268,
    Conceal = 269,
    NightVision = 270,
    ElectricalBolt = 271,
    StrengthenDoor = 272,
    ThickSkin = 273,
    WaterWalk = 274,
    Heal = 275,
    Levitate = 276,
    Poison = 277,
    Flameproof = 278,
    RemoveTrap = 279,
    Fireball = 280,
    SmiteUndead = 281,
    NameEnchantment = 282,
    MissileProtection = 283,
    Open = 284,
    CurePoison = 285,
    GreaterHeal = 286,
    SheetLightning = 287,
    GateTravel = 288,
    Paralyze = 289,
    Daylight = 290,
    Telekinesis = 291,
    Fly = 292,
    Ally = 293,
    SummonMonster = 294,
    Invisibility = 295,
    Confusion = 296,
    Reveal = 297,
    IronFlesh = 298,
    Tremor = 299,
    RoamingSight = 300,
    FlameWind = 301,
    FreezeTime = 302,
    Armageddon = 303,
    MassParalyze = 304,
    Acid = 305,
    LocalTeleport = 306,
    ManaBoost = 307,
    RestoreMana = 308,
    TheFrog = 211,
    MazeNavigation = 212,
    Hallucination = 213,
    Nothing = 0
}
