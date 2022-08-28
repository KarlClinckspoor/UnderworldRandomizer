﻿using System.Diagnostics;
using static Randomizer.Utils;

namespace Randomizer
{
    // TODO: CONTINUE HERE
    public class MobileObject : GameObject
    {
        public new const int ExtraLength = 19;
        public new const int TotalLength = BaseLength + ExtraLength;

        protected const int offset1 = 0x8;
        protected const int offset2 = 0x9;
        protected const int offset3 = 0xa;
        protected const int offset4 = 0xb;
        protected const int offset5 = 0xd;
        protected const int offset6 = 0xf;
        protected const int offset7 = 0x11;
        protected const int offset8 = 0x12;
        protected const int offset9 = 0x13;
        protected const int offset10 = 0x14;
        protected const int offset11 = 0x15;
        protected const int offset12 = 0x16;
        protected const int offset13 = 0x18;
        protected const int offset14 = 0x19;
        protected const int offset15 = 0x1a;

        protected byte byte1_hp; // offset 0008
        protected byte byte2_unk; // offset 0009
        protected byte byte3_unk; // offset 000a
        protected short short_NPCGoalGtarg; // offset 000b
        protected short short_NPCLevelTalkedAttitude; // offset 000d
        protected short short_NPCheightQM; // offset 000f
        protected byte byte4_unk; // offset 0011
        protected byte byte5_unk; // offset 0012
        protected byte byte6_unk; // offset 0013
        protected byte byte7_unk; // offset 0014
        protected byte byte8_unk; // offset 0015
        protected short short_NPChome; // offset 0016
        protected byte byte_NPCheading; // offset 0018
        protected byte byte_NPCHunger; // offset 0019
        protected byte byte_NPCwhoami; // offset 001a

        public new byte[] Buffer = new byte[TotalLength];
        public int ObjectBufferIfx;

        public int HP
        {
            get { return (int) byte1_hp; }
            set { byte1_hp = (byte) value; UpdateBuffer();}
        }

        public int Goal
        {
            get { return GetBits(short_NPCGoalGtarg, 0b1111, 0); }
            set { short_NPCGoalGtarg = (short) SetBits(short_NPCGoalGtarg, value, 0b1111, 0); UpdateBuffer();}
        }

        public int Gtarg
        {
            get { return GetBits(short_NPCGoalGtarg, 0b11111111, 4); }
            set { short_NPCGoalGtarg = (short) SetBits(short_NPCGoalGtarg, value, 0b11111111, 4); UpdateBuffer();}
        }

        public int Level
        {
            get { return GetBits(short_NPCLevelTalkedAttitude, 0b111, 0); }
            set { short_NPCLevelTalkedAttitude = (short) SetBits(short_NPCLevelTalkedAttitude, value, 0b111, 0); UpdateBuffer();}
        }

        public bool TalkedTo
        {
            get { return GetBits(short_NPCLevelTalkedAttitude, 0b1, 13) == 1; }
            set { short_NPCLevelTalkedAttitude = (short) SetBits(short_NPCLevelTalkedAttitude, value ? 1:0, 0b1, 13); UpdateBuffer();}
        }

        public int Attitude
        {
            get { return GetBits(short_NPCLevelTalkedAttitude, 0b11, 14); }
            set { short_NPCLevelTalkedAttitude = (short) SetBits(short_NPCLevelTalkedAttitude, value, 0b11, 14); UpdateBuffer();}
        }

        public int Height
        {
            get { return GetBits(short_NPCheightQM, 0b1111111, 6); }
            set { short_NPCheightQM = (short) SetBits(short_NPCheightQM, value, 0b1111111, 6); UpdateBuffer();}
        }

        public int YHome
        {
            get { return GetBits(short_NPChome, 0b111111, 4); }
            set { short_NPChome = (short) SetBits(short_NPChome, value, 0b111111, 4); UpdateBuffer();}
        }

        public int XHome
        {
            get { return GetBits(short_NPChome, 0b111111, 10); }
            set { short_NPChome = (short) SetBits(short_NPChome, value, 0b111111, 10); UpdateBuffer();}
        }

        public int NPCHeading
        {
            get { return GetBits(byte_NPCheading, 0b1111, 0); }
            set { byte_NPCheading = (byte) SetBits(byte_NPCheading, value, 0b1111, 0); UpdateBuffer();}
        }

        public int Hunger
        {
            get { return GetBits(byte_NPCHunger, 0b111111, 0); }
            set { byte_NPCHunger = (byte) SetBits(byte_NPCHunger, value, 0b111111, 0); UpdateBuffer();}
        }

        public int whoami
        {
            get { return GetBits(byte_NPCwhoami, 0b11111111, 0); }
            set { byte_NPCwhoami = (byte) SetBits(byte_NPCwhoami, value, 0b11111111, 0); UpdateBuffer();}
        }

        public new void UpdateBuffer()
        {
            // From base
            byte[] tempbuffer = new byte[TotalLength];
            byte[] field1 = BitConverter.GetBytes(this.objid_flagsField);
            byte[] field2 = BitConverter.GetBytes(this.positionField);
            byte[] field3 = BitConverter.GetBytes(this.quality_chainField);
            byte[] field4 = BitConverter.GetBytes(this.link_specialField);
            field1.CopyTo(tempbuffer, 0);
            field2.CopyTo(tempbuffer, 2);
            field3.CopyTo(tempbuffer, 4);
            field4.CopyTo(tempbuffer, 6);
            tempbuffer.CopyTo(Buffer, 0);
            // new ones
            Buffer[offset1] = byte1_hp;
            Buffer[offset2] = byte2_unk;
            Buffer[offset3] = byte3_unk;
            BitConverter.GetBytes(short_NPCGoalGtarg).CopyTo(Buffer, offset4);
            BitConverter.GetBytes(short_NPCLevelTalkedAttitude).CopyTo(Buffer, offset5);
            BitConverter.GetBytes(short_NPCheightQM).CopyTo(Buffer, offset6);
            Buffer[offset7] = byte4_unk;
            Buffer[offset8] = byte5_unk;
            Buffer[offset9] = byte6_unk;
            Buffer[offset10] = byte7_unk;
            Buffer[offset11] = byte8_unk;
            BitConverter.GetBytes(short_NPChome).CopyTo(Buffer, offset12);
            Buffer[offset13] = byte_NPCheading;
            Buffer[offset14] = byte_NPCHunger;
            Buffer[offset15] = byte_NPCwhoami;
        }

        public new void UpdateEntries()
        {
            // From base
            objid_flagsField = BitConverter.ToInt16(Buffer, 0);
            positionField = BitConverter.ToInt16(Buffer, 2);
            quality_chainField = BitConverter.ToInt16(Buffer, 4);
            link_specialField = BitConverter.ToInt16(Buffer, 6);
            // New ones
            byte1_hp = Buffer[offset1];
            byte2_unk = Buffer[offset2];
            byte3_unk = Buffer[offset3];
            short_NPCGoalGtarg = BitConverter.ToInt16(Buffer, offset4);
            short_NPCLevelTalkedAttitude = BitConverter.ToInt16(Buffer, offset5);
            short_NPCheightQM = BitConverter.ToInt16(Buffer, offset6);
            byte4_unk = Buffer[offset7];
            byte5_unk = Buffer[offset8];
            byte6_unk = Buffer[offset9];
            byte7_unk = Buffer[offset10];
            byte8_unk = Buffer[offset11];
            short_NPChome = BitConverter.ToInt16(Buffer, offset12);
            byte_NPCheading = Buffer[offset13];
            byte_NPCHunger = Buffer[offset14];
            byte_NPCwhoami = Buffer[offset15];
        }
        
        // todo: don't forget to check if these calls to virtual functions will mess stuff up.
        public MobileObject(byte[] buffer, short idx)
        {
            // Debug.Assert(buffer.Length == TotalLength);
            this.Buffer = buffer;
            this.IdxAtObjectArray = idx;
            UpdateEntries();
        }

        public MobileObject(byte[] baseBuffer, byte byte1_hp, byte unk2, byte unk3, short NPCGoalGTarg, short NPCLevelTalked,
            short NPCheight, byte unk4, byte unk5, byte unk6,
            byte unk7, byte unk8, short NPChome, byte heading, byte hunger, byte whoami, short idx)
        {
            baseBuffer.CopyTo(Buffer, 0);
            byte[] extra = new byte[ExtraLength]
            {
                byte1_hp,
                byte2_unk,
                byte3_unk,
                BitConverter.GetBytes(NPCGoalGTarg)[0],
                BitConverter.GetBytes(NPCGoalGTarg)[1],
                BitConverter.GetBytes(NPCLevelTalked)[0],
                BitConverter.GetBytes(NPCLevelTalked)[1],
                BitConverter.GetBytes(NPCheight)[0],
                BitConverter.GetBytes(NPCheight)[1],
                byte4_unk,
                byte5_unk,
                byte6_unk,
                byte7_unk,
                byte8_unk,
                BitConverter.GetBytes(NPChome)[0],
                BitConverter.GetBytes(NPChome)[1],
                heading,
                hunger,
                whoami
            };
            extra.CopyTo(Buffer, BaseLength);
            IdxAtObjectArray = idx;
            UpdateEntries();
        }
        public MobileObject(short short1, short short2, short short3, short short4, byte byte1_hp, byte unk2, byte unk3, short NPCGoalGTarg, short NPCLevelTalked,
            short NPCheight, byte unk4, byte unk5, byte unk6,
            byte unk7, byte unk8, short NPChome, byte heading, byte hunger, byte whoami, short idx)
        {
            byte[] baseBuffer = new byte[BaseLength];
            BitConverter.GetBytes(short1).CopyTo(baseBuffer, 2*0);
            BitConverter.GetBytes(short2).CopyTo(baseBuffer, 2*1);
            BitConverter.GetBytes(short3).CopyTo(baseBuffer, 2*2);
            BitConverter.GetBytes(short4).CopyTo(baseBuffer, 2*3);
            byte[] extra = new byte[ExtraLength]
            {
                byte1_hp,
                byte2_unk,
                byte3_unk,
                BitConverter.GetBytes(NPCGoalGTarg)[0],
                BitConverter.GetBytes(NPCGoalGTarg)[1],
                BitConverter.GetBytes(NPCLevelTalked)[0],
                BitConverter.GetBytes(NPCLevelTalked)[1],
                BitConverter.GetBytes(NPCheight)[0],
                BitConverter.GetBytes(NPCheight)[1],
                byte4_unk,
                byte5_unk,
                byte6_unk,
                byte7_unk,
                byte8_unk,
                BitConverter.GetBytes(NPChome)[0],
                BitConverter.GetBytes(NPChome)[1],
                heading,
                hunger,
                whoami
            };
            extra.CopyTo(Buffer, BaseLength);
            IdxAtObjectArray = idx;
            UpdateEntries();
        }
    }
}