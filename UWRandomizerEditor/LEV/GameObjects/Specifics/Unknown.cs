namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class UnknownS: StaticObject
{
    public UnknownS(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public UnknownS(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
    
}

public class UnknownM : MobileObject
{
    public UnknownM(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public UnknownM(byte[] baseBuffer, byte byte1_hp, byte unk2, byte unk3, short NPCGoalGtarg, short NPCLevelTalked, short NPCheight, byte unk4, byte unk5, byte unk6, byte unk7, byte unk8, short NPChome, byte heading, byte hunger, byte whoami, ushort idx) : base(baseBuffer, byte1_hp, unk2, unk3, NPCGoalGtarg, NPCLevelTalked, NPCheight, unk4, unk5, unk6, unk7, unk8, NPChome, heading, hunger, whoami, idx)
    {
    }

    public UnknownM(ushort short1, ushort short2, ushort short3, ushort short4, byte byte1_hp, byte unk2, byte unk3, short NPCGoalGTarg, short NPCLevelTalked, short NPCheight, byte unk4, byte unk5, byte unk6, byte unk7, byte unk8, short NPChome, byte heading, byte hunger, byte whoami, ushort idx) : base(short1, short2, short3, short4, byte1_hp, unk2, unk3, NPCGoalGTarg, NPCLevelTalked, NPCheight, unk4, unk5, unk6, unk7, unk8, NPChome, heading, hunger, whoami, idx)
    {
    }
}