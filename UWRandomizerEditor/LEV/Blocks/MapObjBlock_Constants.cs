using UWRandomizerEditor.LEV.GameObjects;

namespace UWRandomizerEditor.LEV.Blocks;

public partial class MapObjBlock 
{
    // From uw-formats.txt
    // offset  size   description
    // 0000    4000   tilemap (64 x 64 x 4 bytes)
    public const int TileMapOffset = 0;
    public const int TileMapLength = 0x4000;
    public const int TileWidth = 64;
    public const int TileHeight = 64;

    public const int TileMapEntrySize = 4;

    // 4000    1b00   mobile object information (objects 0000-00ff, 256 x 27 bytes)
    public const int MobileObjectInfoOffset = 0x4000;
    public const int MobileObjectInfoLength = 0x1b00;
    public const int MobileObjectInfoEntrySize = MobileObject.FixedMobileBufferLength;
    public const int MobileObjectNum = 256;

    // 5b00    1800   static object information (objects 0100-03ff, 768 x 8 bytes)
    public const int StaticObjectInfoOffset = 0x5b00;
    public const int StaticObjectInfoLength = 0x1800;
    public const int StaticObjectInfoEntrySize = GameObject.FixedBufferLength;
    public const int StaticObjectNum = 768;

    // 7300    01fc   free list for mobile objects (objects 0002-00ff, 254 x 2 bytes)
    public const int FreeListMobileObjectsOffset = 0x7300;
    public const int FreeListMobileObjectsLength = 0x01fc;
    public const int FreeListMobileObjectsEntrySize = 2;
    public const int NumOfFreeMobileObjects = 254;

    // 74fc    0600   free list for static objects (objects 0100-03ff, 768 x 2 bytes)
    public const int FreeListStaticObjectsOffset = 0x74fc;
    public const int FreeListStaticObjectsLength = 0x0600;
    public const int FreeListStaticObjectsEntrySize = 2;
    public const int FreeStaticObjectSlotsNumber = 768;

    // 7afc    0104   unknown(260 bytes)
    public const int UnknownOffset = 0x7afc;
    public const int UnknownLength = 0x104;
    public const int UnknownEntrySize = 1; // irrelevant ATM

    // 7c00    0002
    public const int Unknown2Offset = 0x7c00;
    public const int Unknown2Length = 2;
    public const int Unknown2EntrySize = 2;

    // 7c02    0002   no.entries in mobile free list minus 1
    public const int FirstFreeSlotInMobileSlotsOffset = 0x7c02;
    public const int NumEntriesMobileFreeListAdjLength = 2;
    public const int NumEntriesMobileFreeListAdjEntrySize = 2;

    // 7c04    0002   no. entries in static free list minus 1
    public const int FirstFreeSlotInStaticSlotsOffset = 0x7c04;
    public const int NumEntriesStaticFreeListAdjLength = 2;
    public const int NumEntriesStaticFreeListAdjEntrySize = 2;

    // 7c06    0002   0x7775 ('uw')
    public const int EndOfBlockConfirmationOffset = 0x7c06;

    public const int EndOfBlockConfirmationDataSize = 2;

    // Has to be this order, because bit converter was inverting this. "uw"
    public const short EndOfBlockConfirmationValue = 0x7577;

    public const int FixedBlockLength = 0x7c08;
}