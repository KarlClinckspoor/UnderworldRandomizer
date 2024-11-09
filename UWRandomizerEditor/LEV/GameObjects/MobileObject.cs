namespace UWRandomizerEditor.LEV.GameObjects;

/// <summary>
/// Class that describes Mobile Objects (e.g. NPCs). Inheritance should use the parameterless constructor because of the virtual methods to update buffer/entries.
/// </summary>
public class MobileObject : GameObject, IContainer
{
    public const int ExtraMobileBufferLength = 19;
    public const int FixedMobileBufferLength = FixedBufferLength + ExtraMobileBufferLength;
    public bool IsActive = false;
    public ushort IdxAtActiveMobj = 0; // Hope this doesn't bite me.
    
    // These refer to the full buffer
    private const int baseoffset_HP = 0x8;
    private const int baseoffset_unk2 = 0x9;
    private const int baseoffset_unk3 = 0xa;
    private const int baseoffset_NPCGoalGTarg = 0xb;
    private const int baseoffset_NPCLevelTalkedAttitude = 0xd;
    private const int baseoffset_NPCHeightQM = 0xf;
    private const int baseoffset_unk4 = 0x11;
    private const int baseoffset_unk5 = 0x12;
    private const int baseoffset_unk6 = 0x13;
    private const int baseoffset_unk7 = 0x14;
    private const int baseoffset_unk8 = 0x15;
    private const int baseoffset_NPChome = 0x16;
    private const int baseoffset_NPCheading = 0x18;
    private const int baseoffset_NPCHunger = 0x19;
    private const int baseoffset_NPCwhoami = 0x1a;
    
    // These refer to the extra bytes reserved for the Mobile Object
    private const int offset_HP = 0x8 - FixedBufferLength;
    private const int offset_unk2 = 0x9 - FixedBufferLength;
    private const int offset_unk3 = 0xa - FixedBufferLength;
    private const int offset_NPCGoalGTarg = 0xb - FixedBufferLength;
    private const int offset_NPCLevelTalkedAttitude = 0xd - FixedBufferLength;
    private const int offset_NPCHeightQM = 0xf - FixedBufferLength;
    private const int offset_unk4 = 0x11 - FixedBufferLength;
    private const int offset_unk5 = 0x12 - FixedBufferLength;
    private const int offset_unk6 = 0x13 - FixedBufferLength;
    private const int offset_unk7 = 0x14 - FixedBufferLength;
    private const int offset_unk8 = 0x15 - FixedBufferLength;
    private const int offset_NPChome = 0x16 - FixedBufferLength;
    private const int offset_NPCheading = 0x18 - FixedBufferLength;
    private const int offset_NPCHunger = 0x19 - FixedBufferLength;
    private const int offset_NPCwhoami = 0x1a - FixedBufferLength;
    

    public byte HP
    {
        get => ExtraInfoBuffer[offset_HP];
        set => ExtraInfoBuffer[offset_HP] = value;
    }

    public byte byte2_unk
    {
        get => ExtraInfoBuffer[offset_unk2];
        set => ExtraInfoBuffer[offset_unk2] = value;
    }

    public byte byte3_unk
    {
        get => ExtraInfoBuffer[offset_unk3];
        set => ExtraInfoBuffer[offset_unk3] = value;
    }

    public short NPCGoalGtarg
    {
        get => BitConverter.ToInt16(ExtraInfoBuffer, offset_NPCGoalGTarg);
        set => BitConverter.GetBytes(value).CopyTo(ExtraInfoBuffer, offset_NPCGoalGTarg);
    }

    public short NPCLevelTalkedAttitude
    {
        get => BitConverter.ToInt16(ExtraInfoBuffer, offset_NPCLevelTalkedAttitude);
        set => BitConverter.GetBytes(value).CopyTo(ExtraInfoBuffer, offset_NPCLevelTalkedAttitude);
    }

    public short NPCheightQM
    {
        get => BitConverter.ToInt16(ExtraInfoBuffer, offset_NPCHeightQM);
        set => BitConverter.GetBytes(value).CopyTo(ExtraInfoBuffer, offset_NPCHeightQM);
    }

    public byte byte4_unk
    {
        get => ExtraInfoBuffer[offset_unk4];
        set => ExtraInfoBuffer[offset_unk4] = value;
    }

    public byte byte5_unk
    {
        get => ExtraInfoBuffer[offset_unk5];
        set => ExtraInfoBuffer[offset_unk5] = value;
    }

    public byte byte6_unk
    {
        get => ExtraInfoBuffer[offset_unk6];
        set => ExtraInfoBuffer[offset_unk6] = value;
    }

    public byte byte7_unk
    {
        get => ExtraInfoBuffer[offset_unk7];
        set => ExtraInfoBuffer[offset_unk7] = value;
    }

    public byte byte8_unk
    {
        get => ExtraInfoBuffer[offset_unk8];
        set => ExtraInfoBuffer[offset_unk8] = value;
    }

    public short NPChome
    {
        get => BitConverter.ToInt16(ExtraInfoBuffer, offset_NPChome);
        set => BitConverter.GetBytes(value).CopyTo(ExtraInfoBuffer, offset_NPChome);
    }

    public byte NPCheading
    {
        get => ExtraInfoBuffer[offset_NPCheading];
        set => ExtraInfoBuffer[offset_NPCheading] = value;
    }

    public byte byte_NPCHunger
    {
        get => ExtraInfoBuffer[offset_NPCHunger];
        set => ExtraInfoBuffer[offset_NPCHunger] = value;
    }

    public byte NPCwhoami
    {
        get => ExtraInfoBuffer[offset_NPCwhoami];
        set => ExtraInfoBuffer[offset_NPCwhoami] = value;
    }

    public int ObjectBufferIfx; // TODO: What's this?

    public int Goal
    {
        get => Utils.GetBits(NPCGoalGtarg, 0b1111, 0);
        set => NPCGoalGtarg = (short) Utils.SetBits(NPCGoalGtarg, value, 0b1111, 0);
    }

    public int Gtarg
    {
        get => Utils.GetBits(NPCGoalGtarg, 0b11111111, 4);
        set => NPCGoalGtarg = (short) Utils.SetBits(NPCGoalGtarg, value, 0b11111111, 4);
    }

    public int Level
    {
        get => Utils.GetBits(NPCLevelTalkedAttitude, 0b111, 0);
        set => NPCLevelTalkedAttitude = (short) Utils.SetBits(NPCLevelTalkedAttitude, value, 0b111, 0);
    }

    public bool TalkedTo
    {
        get => Utils.GetBits(NPCLevelTalkedAttitude, 0b1, 13) == 1;
        set => NPCLevelTalkedAttitude =
                (short) Utils.SetBits(NPCLevelTalkedAttitude, value ? 1 : 0, 0b1, 13);
    }

    public int Attitude
    {
        get => Utils.GetBits(NPCLevelTalkedAttitude, 0b11, 14);
        set => NPCLevelTalkedAttitude = (short) Utils.SetBits(NPCLevelTalkedAttitude, value, 0b11, 14);
    }

    public int Height
    {
        get => Utils.GetBits(NPCheightQM, 0b1111111, 6);
        set => NPCheightQM = (short) Utils.SetBits(NPCheightQM, value, 0b1111111, 6);
    }

    public int YHome
    {
        get => Utils.GetBits(NPChome, 0b111111, 4);
        set => NPChome = (short) Utils.SetBits(NPChome, value, 0b111111, 4);
    }

    public int XHome
    {
        get => Utils.GetBits(NPChome, 0b111111, 10);
        set => NPChome = (short) Utils.SetBits(NPChome, value, 0b111111, 10);
    }

    public int NPCHeading
    {
        get => Utils.GetBits(NPCheading, 0b1111, 0);
        set => NPCheading = (byte) Utils.SetBits(NPCheading, value, 0b1111, 0);
    }

    public int Hunger
    {
        get => Utils.GetBits(byte_NPCHunger, 0b111111, 0);
        set => byte_NPCHunger = (byte) Utils.SetBits(byte_NPCHunger, value, 0b111111, 0);
    }

    // TODO: Isn't this basically just returning NPCwhoami?
    public int whoami
    {
        get => Utils.GetBits(NPCwhoami, 0b11111111, 0);
        set => NPCwhoami = (byte) Utils.SetBits(NPCwhoami, value, 0b11111111, 0);
    }

    /// <summary>
    /// NPC inventory
    /// </summary>
    public UWLinkedList Contents { get; set; }

    public override bool ReconstructBuffer()
    {
        LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, Contents.StartingIdx, 0b11_1111_1111, 6);
        base.ReconstructBuffer();
        return true;
    }

    public MobileObject(byte[] buffer, ushort idx): base()
    {
        // This has to be created here. If I call the base class, then the ExtraInfoBuffer will not have been filled.
        ExtraInfoBuffer = new byte[ExtraMobileBufferLength];
        Buffer = buffer;
        IdxAtObjectArray = idx;
        Contents = new UWLinkedList() {StartingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }

    public MobileObject(
        byte[] baseBuffer,
        byte byte1_hp,
        byte unk2,
        byte unk3,
        short NPCGoalGtarg,
        short NPCLevelTalked,
        short NPCheight,
        byte unk4,
        byte unk5,
        byte unk6,
        byte unk7,
        byte unk8,
        short NPChome,
        byte heading,
        byte hunger,
        byte whoami,
        ushort idx
    )
    {
        baseBuffer.CopyTo(BasicInfoBuffer, 0);
        ExtraInfoBuffer = new byte[ExtraMobileBufferLength];
        HP = byte1_hp;
        byte2_unk = unk2;
        byte3_unk = unk3;
        this.NPCGoalGtarg = NPCGoalGtarg;
        NPCLevelTalkedAttitude = NPCLevelTalked;
        NPCheightQM = NPCheight;
        byte4_unk = unk4;
        byte5_unk = unk5;
        byte6_unk = unk6;
        byte7_unk = unk7;
        byte8_unk = unk8;
        this.NPChome = NPChome;
        Heading = heading;
        Hunger = hunger;
        NPCwhoami = whoami;
        IdxAtObjectArray = idx;
        Contents = new UWLinkedList() {StartingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }

    public MobileObject(ushort short1,
        ushort short2,
        ushort short3,
        ushort short4,
        byte byte1_hp,
        byte unk2,
        byte unk3,
        short NPCGoalGTarg,
        short NPCLevelTalked,
        short NPCheight,
        byte unk4,
        byte unk5,
        byte unk6,
        byte unk7,
        byte unk8,
        short NPChome,
        byte heading,
        byte hunger,
        byte whoami,
        ushort idx)
    {
        ObjIdFlags = short1;
        Position = short2;
        QualityChain = short3;
        LinkSpecial = short4;

        ExtraInfoBuffer = new byte[ExtraMobileBufferLength];
        
        HP = byte1_hp;
        byte2_unk = unk2;
        byte3_unk = unk3;
        NPCGoalGtarg = NPCGoalGTarg;
        NPCLevelTalkedAttitude = NPCLevelTalked;
        NPCheightQM = NPCheight;
        byte4_unk = unk4;
        byte5_unk = unk5;
        byte6_unk = unk6;
        byte7_unk = unk7;
        byte8_unk = unk8;
        this.NPChome = NPChome;
        Heading = heading;
        Hunger = hunger;
        NPCwhoami = whoami;
        
        IdxAtObjectArray = idx;
        Contents = new UWLinkedList() {StartingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }

    private MobileObject(): base()
    {
        Contents = new UWLinkedList() {StartingIdx = 0, RepresentingContainer = true};
    }

    public static MobileObject ZeroedOutMobileObject(ushort idx) => new MobileObject() {IdxAtObjectArray = idx};
    
    public MobileObject Clone() => new MobileObject(Buffer, IdxAtObjectArray);
}