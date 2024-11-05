using UWRandomizerEditor.LEV.GameObjects;

namespace UWRandomizerEditor.LEV.Blocks;

public partial class MapObjBlock 
{
    // From uw-formats.txt
    // offset  size   description
    // 0000    4000   tilemap (64 x 64 x 4 bytes)
    public const int OffsetOfTileMap = 0;
    public const int LengthOfTileMap = 0x4000;
    public const int TileWidth = 64;
    public const int TileHeight = 64;

    public const int EntrySizeOfTileMap = 4;

    // 4000    1b00   mobile object information (objects 0000-00ff, 256 x 27 bytes)
    public const int OffsetOfMobileObjectInfo = 0x4000;
    public const int LengthOfMobileObjectInfo = 0x1b00;
    public const int EntrySizeOfMobileObjectInfo = MobileObject.FixedMobileBufferLength;
    public const int NumOfMobileObjects = 256;

    // 5b00    1800   static object information (objects 0100-03ff, 768 x 8 bytes)
    public const int OffsetOfStaticObjectInfo = 0x5b00;
    public const int LengthOfStaticObjectInfo = 0x1800;
    public const int EntrySizeOfStaticObjectInfo = GameObject.FixedBufferLength;
    public const int NumOfStaticObjects = 768;

    // 7300    01fc   free list for mobile objects (objects 0002-00ff, 254 x 2 bytes)
    public const int OffsetOfFreeListMobileObjects = 0x7300;
    public const int LengthOfFreeListMobileObjects = 0x01fc;
    public const int EntrySizeFreeListMobileObjects = 2;
    public const int NumOfFreeMobileObjects = 254;

    // 74fc    0600   free list for static objects (objects 0100-03ff, 768 x 2 bytes)
    public const int OffsetOfFreeListStaticObjects = 0x74fc;
    public const int LengthOfFreeListStaticObjects = 0x0600;
    public const int EntrySizeOfFreeListStaticObjects = 2;
    public const int NumberOfFreeStaticObjectSlots = 768;

    // 7afc    0104   unknown(260 bytes) - known now thanks to krokots' work.
    public const int OffsetOfListOfActiveMobileObjects = 0x7afc;
    public const int LengthOfListOfActiveMobileObjects = 0x104;
    public const int EntrySizeOfListOfActiveMobs = 1;

    // 7c00    0002
    public const int OffsetOfIdxLookupOfActiveMobs = 0x7c00;
    public const int LengthOfIdxLookupOfActiveMobs = 2;
    public const int EntrySizeOfIdxLookupOfActiveMobs = 2;

    // 7c02    0002   no.entries in mobile free list minus 1
    public const int OffsetOfFirstFreeSlotInMobileSlots = 0x7c02;
    public const int NumEntriesMobileFreeListAdjLength = 2;
    public const int NumEntriesMobileFreeListAdjEntrySize = 2;

    // 7c04    0002   no. entries in static free list minus 1
    public const int OffsetOfFirstFreeSlotInStaticSlots = 0x7c04;
    public const int NumEntriesStaticFreeListAdjLength = 2;
    public const int NumEntriesStaticFreeListAdjEntrySize = 2;

    // 7c06    0002   0x7775 ('uw')
    public const int OffsetOfEndOfBlockConfirmation = 0x7c06;

    public const int DataSizeOfEndOfBlockConfirmation = 2;

    // Has to be this order, because bit converter was inverting this. "uw"
    public const short EndOfBlockConfirmationValue = 0x7577;

    public const int FixedBlockLength = 0x7c08;
}