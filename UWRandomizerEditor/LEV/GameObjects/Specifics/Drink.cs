namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Drink : StaticObject, IEnchantable<DrinkEnchantment>
{
    public Drink(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public Drink(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }

    public bool IsEnchanted { get; private set; }

    public ushort SpecialIdx
    {
        get => QuantityOrSpecialLinkOrSpecialProperty;
        // TODO: Should't 'LinkSpecial' be 'QuantityOrSpecialLinkOrSpecialProperty'?
        set => LinkSpecial = (ushort)Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
    }

    public IdentificationStatus IDStatus
    {
        get => (IdentificationStatus)(Heading & 0b111);
        set => Heading = (byte) Utils.SetBits(Heading, (byte)value, 0b111, 0);
    }

    public void AddEnchantment(DrinkEnchantment enchant)
    {
        EnchantmentNumber = ((int)enchant - 256);
        IsEnchanted = true;
    }

    public void ForceAddEnchantment(int value)
    {
        EnchantmentNumber = value;
        IsEnchanted = true;
    }

    public void RemoveEnchantment()
    {
        IsEnchanted = false;
        EnchantmentNumber = 0; // TODO: which value? 0? -1?
    }

    public DrinkEnchantment Enchantment => IsEnchanted ? (DrinkEnchantment)SpellValue : DrinkEnchantment.Nothing;

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

public enum DrinkEnchantment
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

// public int GetEnchantment()
// {
//     int e = Special - 512;
//     if(IsWeapon())
//     {
//         if (e >= 192)
//             return e + 256;
//     }
//     else if(IsArmour() || IsBauble() || IsShield())
//     {
//         if (e >= 192 && e < 208)
//             return e + 272;
//         //if (e >= 208)
//         //	return e + 178;
//     }
//     else if(IsPotion() || IsScroll() || IsSpell() || IsDrink())
//     {
//         if (e < 53) //spell effects
//             return e + 256;
//         else if (e >= 67 && e < 70)	//Maze, frog, halluc
//             return e + 144;
//         else
//             return -1;
//     }
//     else if (IsWand())
//     {
//         return e;
//     }
//
//     return Special;
// }

// public static string GetEnchantmentName(int id)
// {
// 	if (id >= 512)
// 		id -= 512;
//
// 	switch (id)
// 	{
// 		case 0: return "Darkness";
// 		case 1: return "Burning Match";
// 		case 2: return "Candlelight";
// 		case 3: return "Light";
// 		case 4: return "Magic Lantern";
// 		case 5: return "Night Vision";
// 		case 6: return "Daylight";
// 		case 7: return "Sunlight";
// 		case 17: return "Leap";
// 		case 18: return "Slow Fall";
// 		case 19: return "Levitate";
// 		case 20: return "Water Walk";
// 		case 21: return "Fly";
// 		case 34: return "Resist Blows";
// 		case 35: return "Thick Skin";
// 		case 37: return "Iron Flesh";
// 		case 49: return "Curse";
// 		case 50: return "Stealth";
// 		case 51: return "Conceal";
// 		case 52: return "Invisibilty";
// 		case 53: return "Missile Protection";
// 		case 54: return "Flameproof";
// 		case 55: return "Poison Resistance";
// 		case 56: return "Magic Protection";
// 		case 57: return "Greater Magic Protection";
// 		case 64: return "Lesser Heal 1";
// 		case 65: return "Lesser Heal 2";
// 		case 66: return "Lesser Heal 3";
// 		case 67: return "Lesser Heal 4";
// 		case 68: return "Heal 1";
// 		case 69: return "Heal 2";
// 		case 70: return "Heal 3";
// 		case 71: return "Heal 4";
// 		case 72: return "Enhanced Heal 1";
// 		case 73: return "Enhanced Heal 2";
// 		case 74: return "Enhanced Heal 3";
// 		case 75: return "Enhanced Heal 4";
// 		case 76: return "Greater Heal 1";
// 		case 77: return "Greater Heal 2";
// 		case 78: return "Greater Heal 3";
// 		case 79: return "Greater Heal 4";
// 		case 81: return "Magic Arrow";
// 		case 82: return "Electrical Bolt";
// 		case 83: return "Fireball";
// 		case 97: return "Reveal";
// 		case 98: return "Sheet Lightning";
// 		case 99: return "Confusion";
// 		case 100: return "Flame Wind";
// 		case 113: return "Cause Fear";
// 		case 114: return "Smite Undead";
// 		case 115: return "Ally";
// 		case 116: return "Poison";
// 		case 117: return "Paralyze";
// 		case 129: return "Create Food";
// 		case 130: return "Set Guard";
// 		case 131: return "Rune of Warding";
// 		case 132: return "Summon Monster";
// 		case 144: return "Cursed";
// 		case 145: return "Cursed";
// 		case 146: return "Cursed";
// 		case 147: return "Cursed";
// 		case 148: return "Cursed";
// 		case 149: return "Cursed";
// 		case 150: return "Cursed";
// 		case 151: return "Cursed";
// 		case 152: return "Cursed";
// 		case 153: return "Cursed";
// 		case 154: return "Cursed";
// 		case 155: return "Cursed";
// 		case 156: return "Cursed";
// 		case 157: return "Cursed";
// 		case 158: return "Cursed";
// 		case 159: return "Cursed";
// 		case 160: return "Increase Mana";
// 		case 161: return "Increase Mana";
// 		case 162: return "Increase Mana";
// 		case 163: return "Increase Mana";
// 		case 164: return "Mana Boost";
// 		case 165: return "Mana Boost";
// 		case 166: return "Mana Boost";
// 		case 167: return "Mana Boost";
// 		case 168: return "Regain Mana";
// 		case 169: return "Regain Mana";
// 		case 170: return "Regain Mana";
// 		case 171: return "Regain Mana";
// 		case 172: return "Restore Mana";
// 		case 173: return "Restore Mana";
// 		case 174: return "Restore Mana";
// 		case 175: return "Restore Mana";
// 		case 176: return "Speed";
// 		case 177: return "Detect Monster";
// 		case 178: return "Strengthen Door";
// 		case 179: return "Remove Trap";
// 		case 180: return "Name Enchantment";
// 		case 181: return "Open";
// 		case 182: return "Cure Poison";
// 		case 183: return "Roaming Sight";
// 		case 184: return "Telekinesis";
// 		case 185: return "Tremor";
// 		case 186: return "Gate Travel";
// 		case 187: return "Freeze Time";
// 		case 188: return "Armageddon";
// 		case 190: return "Regeneration";
// 		case 191: return "Mana Regeneration";
// 		case 211: return "the Frog";
// 		case 212: return "Maze Navigation";
// 		case 213: return "Hallucination";
// 		case 256: return "Light";
// 		case 257: return "Resist Blows";
// 		case 258: return "Magic Arrow";
// 		case 259: return "Create Food";
// 		case 260: return "Stealth";
// 		case 261: return "Leap";
// 		case 262: return "Curse";
// 		case 263: return "Slow Fall";
// 		case 264: return "Lesser Heal";
// 		case 265: return "Detect Monster";
// 		case 266: return "Cause Fear";
// 		case 267: return "Rune of Warding";
// 		case 268: return "Speed";
// 		case 269: return "Conceal";
// 		case 270: return "Night Vision";
// 		case 271: return "Electrical Bolt";
// 		case 272: return "Strengthen Door";
// 		case 273: return "Thick Skin";
// 		case 274: return "Water Walk";
// 		case 275: return "Heal";
// 		case 276: return "Levitate";
// 		case 277: return "Poison";
// 		case 278: return "Flameproof";
// 		case 279: return "Remove Trap";
// 		case 280: return "Fireball";
// 		case 281: return "Smite Undead";
// 		case 282: return "Name Enchantment";
// 		case 283: return "Missile Protection";
// 		case 284: return "Open";
// 		case 285: return "Cure Poison";
// 		case 286: return "Greater Heal";
// 		case 287: return "Sheet Lightning";
// 		case 288: return "Gate Travel";
// 		case 289: return "Paralyze";
// 		case 290: return "Daylight";
// 		case 291: return "Telekinesis";
// 		case 292: return "Fly";
// 		case 293: return "Ally";
// 		case 294: return "Summon Monster";
// 		case 295: return "Invisibility";
// 		case 296: return "Confusion";
// 		case 297: return "Reveal";
// 		case 298: return "Iron Flesh";
// 		case 299: return "Tremor";
// 		case 300: return "Roaming Sight";
// 		case 301: return "Flame Wind";
// 		case 302: return "Freeze Time";
// 		case 303: return "Armageddon";
// 		case 304: return "Mass Paralyze";
// 		case 305: return "Acid";
// 		case 306: return "Local Teleport";
// 		case 307: return "Mana Boost";
// 		case 308: return "Restore Mana";
// 		case 384: return "Leap";
// 		case 385: return "Slow Fall";
// 		case 386: return "Levitate";
// 		case 387: return "Water Walk";
// 		case 388: return "Fly";
// 		case 389: return "Curse";
// 		case 390: return "Stealth";
// 		case 391: return "Conceal";
// 		case 392: return "Invisibility";
// 		case 393: return "Missile Protection";
// 		case 394: return "Flameproof";
// 		case 395: return "Freeze Time";
// 		case 396: return "Roaming Sight";
// 		case 397: return "Haste";
// 		case 398: return "Telekinesis";
// 		case 399: return "Resist Blows";
// 		case 400: return "Thick Skin";
// 		case 401: return "Light";
// 		case 402: return "Iron Flesh";
// 		case 403: return "Night Vision";
// 		case 404: return "Daylight";
// 		case 448: return "Minor Accuracy";
// 		case 449: return "Accuracy";
// 		case 450: return "Additional Accuracy";
// 		case 451: return "Major Accuracy";
// 		case 452: return "Great Accuracy";
// 		case 453: return "Very Great Accuracy";
// 		case 454: return "Tremendous Accuracy";
// 		case 455: return "Unsurpassed Accuracy";
// 		case 456: return "Minor Damage";
// 		case 457: return "Damage";
// 		case 458: return "Additional Damage";
// 		case 459: return "Major Damage";
// 		case 460: return "Great Damage";
// 		case 461: return "Very Great Damage";
// 		case 462: return "Tremendous Damage";
// 		case 463: return "Unsurpassed Damage";
// 		case 464: return "Minor Protection";
// 		case 465: return "Protection";
// 		case 466: return "Additional Protection";
// 		case 467: return "Major Protection";
// 		case 468: return "Great Protection";
// 		case 469: return "Very Great Protection";
// 		case 470: return "Tremendous Protection";
// 		case 471: return "Unsurpassed Protection";
// 		case 472: return "Minor Toughness";
// 		case 473: return "Toughness";
// 		case 474: return "Additional Toughness";
// 		case 475: return "Major Toughness";
// 		case 476: return "Great Toughness";
// 		case 477: return "Very Great Toughness";
// 		case 478: return "Tremendous Toughness";
// 		case 479: return "Unsurpassed Toughness";
//
// 		default: return "INVALID";
// 	}
// }