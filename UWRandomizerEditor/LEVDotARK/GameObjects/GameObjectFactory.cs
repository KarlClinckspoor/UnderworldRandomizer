using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;
using static UWRandomizerEditor.Utils;

namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public static class GameObjectFactory
{

    private static short getItemIDFromBuffer(byte[] buffer)
    {
        short objid_flagsField = BitConverter.ToInt16(buffer, 0);
        // TODO: Duplicated, but oh well
        return (short) GetBits(objid_flagsField, 0b111111111, 0);  
    }

    public static GameObject CreateFromBuffer(byte[] buffer, short idxAtArray)
    {
        short itemID = getItemIDFromBuffer(buffer);
        if (buffer.Length == MobileObject.TotalLength)
        {
            return new MobileObject(buffer, idxAtArray);
        }

        if (buffer.Length == StaticObject.TotalLength)
        {
            return new StaticObject(buffer, idxAtArray);
        }
        

    //     if (buffer.Length == MobileObject.TotalLength)
    //     {
    //         return new MobileObject(buffer, idxAtArray);
    //     }
    //     else if (buffer.Length == StaticObject.TotalLength)
    //     {
    //         if (itemID <= 0x1f) // Weapons and missiles
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x20 & itemID <= 0x3f) // Armor and clothes
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x80 & itemID <= 0x8f) // Containers
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x90 & itemID <= 0x97) // Light sources
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x98 & itemID <= 0x9f) // Wands
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0xa0 & itemID <= 0xaf) // Treasure
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0xb0 & itemID <= 0xbf) // Comestible
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0xc0 & itemID <= 0xdf) // Scenery and junk
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0xe0 & itemID <= 0xff) // Runes and bits of key
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x100 & itemID <= 0x10e)
    //             return new Key(buffer, idxAtArray);
    //         if (itemID == 0x10f)
    //             return new Lock(buffer, idxAtArray);
    //         if (itemID >= 0x110 & itemID <= 0x11f) // quest items
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x120 & itemID <= 0x12f) // Inventory items, misc stuff
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x130 & itemID <= 0x13f) // Books and scrolls
    //             return new StaticObject(buffer, idxAtArray);
    //         if (itemID >= 0x140 & itemID <= 0x14f) // Doors
    //             return new Door(buffer, idxAtArray);
    //         if (itemID >= 0x150 & itemID <= 0x15f) // Furniture
    //             return new Furniture(buffer, idxAtArray);
    //         if (itemID >= 0x160 & itemID <= 0x16f) // Pillars, etc
    //             return new Furniture(buffer, idxAtArray); // TODO: should be joined with furniture?
    //         if (itemID >= 0x170 & itemID <= 0x17f) // Switches
    //             return new TexturedGameObject(buffer, idxAtArray);
    //         if (itemID >= 0x180 & itemID <= 0x19f) // Traps
    //             return new Trap(buffer, idxAtArray);
    //         if (itemID >= 0x1a0 & itemID <= 0x1bf) // Triggers
    //             return new Trigger(buffer, idxAtArray);
    //         if (itemID >= 0x1c0 & itemID <= 0x1cf) // Explosions, splats, fountain, silver tree
    //             return new StaticObject(buffer, idxAtArray) {ShouldBeMoved = false};
    //         if (itemID > 0x1cf & itemID <= 0x1ff) // Not described in the text
    //             return new StaticObject(buffer, idxAtArray);
    //         
    //         if (itemID > 0x1ff)
    //         {
    //             throw new ArgumentException($"Invalid itemID {itemID} when creating a GameObject");
    //         }
    //     }
    throw new ArgumentException($"Invalid buffer length of {buffer.Length} when creating a GameObject");
    }

}