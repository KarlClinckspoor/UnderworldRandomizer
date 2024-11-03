using UWRandomizerEditor.LEV.Blocks;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

namespace UWRandomizerEditor.LEV.GameObjects;

/// <summary>
/// This class controls the creation of game objects based upon their id and a large reference table.
/// </summary>
public static class GameObjectFactory
{
    // ReSharper disable once CognitiveComplexity
    /// <summary>
    /// This creates a <see cref="GameObject"/> based on the provided buffer and its index in the array.
    /// Static or Mobile objects are created based on the itemID, index and buffer length.
    /// </summary>
    /// <param name="buffer">byte buffer containing the data. ItemID is extracted from this</param>
    /// <param name="idxAtArray">index at array to pass along to the object and double-check if the buffer length is adequate</param>
    /// <returns>
    ///   A GameObject that can be cast to the appropriate type later. Some will have the flag 'Invalid',
    ///   which means they have some combination of ItemID and idxAtArray and Buffer length that shouldn't happen, but
    ///   does happen.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if the buffer length is inadequate (different from
    /// <see cref="GameObject.FixedBufferLength"/> or <see cref="MobileObject.FixedMobileBufferLength"/>) </exception>
    /// <exception cref="Exception">Thrown when the itemID can't be matched to an existing object (greater than 0x1ff)</exception>
    public static GameObject CreateFromBuffer(byte[] buffer, ushort idxAtArray)
    {
        if ((idxAtArray == 0) | (idxAtArray == 1)) // These never hold actual object data.
        {
            return new MobileObject(buffer, idxAtArray);
        }

        // Create a StaticObject just to get the ItemID for later.
        var tempObject = new StaticObject(buffer[0..8], 2);
        var itemID = tempObject.ItemID;

        // Start is always MobileObjects
        if (idxAtArray < MapObjBlock.MobileObjectNum)
        {
            if (buffer.Length != MobileObject.FixedMobileBufferLength)
            {
                throw new InvalidOperationException(
                    $"Cannot create a Mobile Object with buffer of length {buffer.Length}");
            }

            // Mobile objects can only have itemIDs in this range
            if ((itemID >= 0x40) & (itemID <= 0x7f))
            {
                return new MobileObject(buffer, idxAtArray);
            }

            // Outside this range, it's not a valid MobileObject
            return new MobileObject(buffer, idxAtArray);
        }

        // Rest is always StaticObjects
        if (idxAtArray < (MapObjBlock.MobileObjectNum + MapObjBlock.StaticObjectNum))
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            if (buffer.Length != StaticObject.FixedBufferLength)
            {
                throw new InvalidOperationException(
                    $"Cannot create a Static Object with buffer of length {buffer.Length}");
            }

            if (itemID <= 0x1f) // Weapons and missiles
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x20 & itemID <= 0x3f) // Armor and clothes
                return new StaticObject(buffer, idxAtArray);
            // Monsters -- dealt above. Always MobileObject. If it's here, it's invalid
            if (itemID >= 0x40 & itemID <= 0x7f)
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x80 & itemID <= 0x8f) // Containers
                return new Container(buffer, idxAtArray);
            if (itemID >= 0x90 & itemID <= 0x97) // Light sources
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x98 & itemID <= 0x9f) // Wands
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0xa0 & itemID <= 0xaf) // Treasure
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0xb0 & itemID <= 0xbf) // Comestible
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0xc0 & itemID <= 0xdf) // Scenery and junk
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0xe0 & itemID <= 0xff) // Runes and bits of key
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x100 & itemID <= 0x10e)
                return new Key(buffer, idxAtArray);
            if (itemID == 0x10f)
                return new Lock(buffer, idxAtArray);
            if (itemID >= 0x110 & itemID <= 0x11f) // quest items
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x120 & itemID <= 0x12f) // Inventory items, misc stuff
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x130 & itemID <= 0x13f) // Books and scrolls
                return new StaticObject(buffer, idxAtArray);
            if (itemID >= 0x140 & itemID <= 0x14f) // Doors
                return new Door(buffer, idxAtArray);
            if (itemID >= 0x150 & itemID <= 0x15f) // Furniture
                return new Furniture(buffer, idxAtArray);
            if (itemID >= 0x160 & itemID <= 0x16f) // Pillars, etc
                return new Furniture(buffer, idxAtArray); // TODO: should be joined with furniture?
            if (itemID >= 0x170 & itemID <= 0x17f) // Switches
                return new TexturedGameObject(buffer, idxAtArray);
            if (itemID >= 0x180 & itemID <= 0x19f) // Traps
                return new Trap(buffer, idxAtArray);
            if (itemID >= 0x1a0 & itemID <= 0x1bf) // Triggers
                return new Trigger(buffer, idxAtArray);
            if (itemID >= 0x1c0 & itemID <= 0x1cf) // Explosions, splats, fountain, silver tree
                return new StaticObject(buffer, idxAtArray);
            if (itemID > 0x1cf & itemID <= 0x1ff) // Not described in the text
                return new StaticObject(buffer, idxAtArray);

            throw new Exception($"Can't create object with itemID {itemID}");
        }


        throw new InvalidOperationException(
            $"Object could not be created. Either invalid buffer length ({buffer.Length}) or invalid index {idxAtArray} in GameObjectFactory");
    }
}