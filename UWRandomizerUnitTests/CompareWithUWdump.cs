﻿using System;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEV;

namespace RandomizerUnitTests;

public class CompareWithUWdump
{
    [Test]
    public void TestYetAnotherThing()
    {
        var path = @"C:\Users\karl9\OneDrive\UnderworldStudy\UW\SAVE1\LEV.ARK";
        var AL = new LevLoader(path);

        var problematicObject = AL.TileMapObjectsBlocks[4].AllGameObjects[0x01a9];
        var shouldBeFreeObject = AL.TileMapObjectsBlocks[4].AllGameObjects[0x01a7];
        var coinID = 160;
        var temp1 = AL.TileMapObjectsBlocks[4].FreeMobileObjIndexes.ToList();
        var temp2 = AL.TileMapObjectsBlocks[4].FreeStaticObjIndexes.ToList();
        var temp3 = temp1.Concat(temp2).ToList();

        temp3.Add(0);

        Console.WriteLine(AL.TileMapObjectsBlocks[4].FirstFreeMobileSlot);
        Console.WriteLine(AL.TileMapObjectsBlocks[4].FirstFreeMobileObjectIdx);
        Console.WriteLine(AL.TileMapObjectsBlocks[4].FirstFreeStaticSlot);
        Console.WriteLine(AL.TileMapObjectsBlocks[4].FirstFreeStaticObjectIdx);
        var nums = new ushort[]
        {
            0x0000, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009, 0x000a, 0x000b, 0x000c, 0x000d, 0x000e, 0x000f, 0x0010,
            0x0011,0x0012,0x0013,0x0014,0x0015,0x0016,0x0017,0x0018,0x0019,0x001a,0x001b,0x001c,0x001d,0x001e,0x001f,0x0020,
            0x0021,0x0022,0x0023,0x0024,0x0025,0x0026,0x0027,0x0028,0x0029,0x002a,0x002b,0x002c,0x002d,0x002e,0x002f,0x0030,
            0x0031,0x0032,0x0033,0x0034,0x0035,0x0036,0x0037,0x0038,0x0039,0x003a,0x003b,0x003c,0x003d,0x003e,0x003f,0x0040,
            0x0041,0x0042,0x0043,0x0044,0x0045,0x0046,0x0047,0x0048,0x0049,0x004a,0x004b,0x004c,0x004d,0x004e,0x004f,0x0050,
            0x0051,0x0052,0x0053,0x0054,0x0055,0x0056,0x0057,0x0058,0x0059,0x005a,0x005b,0x005c,0x005d,0x005e,0x005f,0x0060,
            0x0061,0x0062,0x0063,0x0064,0x0065,0x0066,0x0067,0x0068,0x0069,0x006a,0x006b,0x006c,0x006d,0x006e,0x006f,0x0070,
            0x0071,0x0072,0x0073,0x0074,0x0075,0x0076,0x0077,0x0078,0x0079,0x007a,0x007b,0x007c,0x007d,0x007e,0x007f,0x0080,
            0x0081,0x0082,0x0083,0x0084,0x0085,0x0086,0x0087,0x0088,0x0089,0x008a,0x008b,0x008c,0x008d,0x008e,0x008f,0x0090,
            0x0091,0x0092,0x0093,0x0094,0x0095,0x0096,0x0097,0x0098,0x0099,0x009a,0x009b,0x009c,0x009d,0x009e,0x009f,0x00a0,
            0x00a1,0x00a2,0x00a3,0x00a4,0x00a5,0x00a6,0x00a7,0x00a8,0x00a9,0x00aa,0x00ab,0x00ac,0x00ad,0x00ae,0x00af,0x00b0,
            0x00c1,0x00c2,0x00c3,0x00cb,0x00ce,0x00cf,0x00d0,0x00d1,0x00d2,0x0100,0x0101,0x0102,0x0103,0x0104,0x0105,0x0106,
            0x00b1,0x00b2,0x00b3,0x00b4,0x00b5,0x00b6,0x00b7,0x00b8,0x00b9,0x00ba,0x00bb,0x00bc,0x00bd,0x00be,0x00bf,0x00c0,
            0x0107,0x0108,0x0109,0x010a,0x010b,0x010c,0x010d,0x010e,0x010f,0x0110,0x0111,0x0112,0x0113,0x0114,0x0115,0x0116,
            0x0117,0x0118,0x0119,0x011a,0x011b,0x011c,0x011d,0x011e,0x011f,0x0120,0x0121,0x0122,0x0123,0x0124,0x0125,0x0126,
            0x0127,0x0128,0x0129,0x012a,0x012b,0x012c,0x012d,0x012e,0x012f,0x0130,0x0131,0x0132,0x0133,0x0134,0x0135,0x0136,
            0x0137,0x0138,0x0139,0x013a,0x013b,0x013c,0x013d,0x013e,0x013f,0x0140,0x0141,0x0142,0x0143,0x0144,0x0145,0x0146,
            0x0147,0x0148,0x0149,0x014a,0x014b,0x014c,0x014d,0x014e,0x014f,0x0150,0x0151,0x0152,0x0153,0x0154,0x0155,0x0156,
            0x0157,0x0158,0x0159,0x015a,0x015b,0x015c,0x015d,0x015e,0x015f,0x0160,0x0161,0x0162,0x0163,0x0164,0x0165,0x0166,
            0x0167,0x0168,0x0169,0x016a,0x016b,0x016c,0x016d,0x016e,0x016f,0x0170,0x0171,0x0172,0x0173,0x0174,0x0175,0x0176,
            0x0177,0x0178,0x0179,0x017a,0x017b,0x017c,0x017d,0x017e,0x017f,0x0180,0x0181,0x0182,0x0183,0x0184,0x0185,0x0186,
            0x0187,0x0188,0x0189,0x018a,0x018b,0x018c,0x018d,0x018e,0x018f,0x0190,0x0191,0x0192,0x0193,0x0194,0x0195,0x0196,
            0x0197,0x0198,0x0199,0x019a,0x019b,0x019c,0x019d,0x019e,0x019f,0x01a0,0x01a1,0x01a2,0x01a3,0x01a4,0x01a5,0x01a6,
            0x01a7,0x01b6,0x01b7,0x01b8,0x01b9,0x01ba,0x01bb,0x01bc,0x01bd,0x01be,0x01c0,0x01c1,0x01c2,0x01c3,0x01c5,0x01c6,
            0x01c7,0x01c8,0x01c9,0x01ca,0x01cb,0x01cd,0x01ce,0x01cf,0x01d0,0x01d1,0x01d2,0x01d3,0x01d4,0x01d5,0x01d6,0x01d7,
            0x01d8,0x01d9,0x01da,0x01db,0x01dc,0x01e3,0x01e5,0x01e7,0x01e8,0x01e9,0x01ea,0x01ec,0x01ed,0x01ee,0x01f3,0x01f5,
            0x01f6,0x01f7,0x0202,0x021d,0x0220,0x0241,0x0244,0x0245,0x0246,0x027d,0x027e,0x0353,0x0376,0x039d,0x03a4,0x03aa,
            0x03b2,0x03c2
        };

        foreach (var t in temp3)
        {
            // Assert.True(nums.Contains(t), $"uwdump doesn't contain {t} ({t:x4})");
            if (!nums.Contains(t))
                Console.WriteLine($"uwdump doesn't contain {t} ({t:x4})");
        }
        foreach (var t in nums)
        {
            // Assert.True(temp3.Contains(t), $"myOwn doesn't contain {t} ({t:x4})");
            if (!temp3.Contains(t))
                Console.WriteLine($"myOwn doesn't contain {t} ({t:x4})");
        }


    }
}