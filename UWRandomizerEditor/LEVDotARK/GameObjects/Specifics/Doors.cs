namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class Door: SpecialLinkGameObject
{
    public Door(byte[] buffer, short idxAtObjArray): base(buffer, idxAtObjArray) {}
    
    public override bool ShouldBeMoved { get; set; } = false;
    
    // On doors:
    // Bit 1 of "owner" field is set, door is spiked
    // sp_link field points to a_lock object (010f), door is locked.
    
    /// <summary>
    /// Points the "sp_link" field to 0, removing any reference to a lock.
    /// </summary>
    public void RemoveLock()
    {
        link_specialField = 0;
        UpdateBuffer();
    }

    // TODO: add verification
    /// <summary>
    /// Checks if sp_link (link_specialField) points to something other than 0. If so, returns the value, which
    /// is the index of the lock.
    /// </summary>
    /// <returns>Index of the lock object</returns>
    public short HasLock()
    {
        if (link_specialField != 0)
        {
            return link_specialField;
        }

        return link_specialField;
    }

    /// <summary>
    /// Replaces sp_link (link_specialField) with the value provided. Does not check if it really points to a lock object.
    /// </summary>
    /// <param name="lock_idx"></param>
    public void AddLock(short lock_idx)
    {
        // TODO: add some checks
        link_specialField = lock_idx;
        UpdateBuffer();
    }
    
    // TODO: By "bit 1" does it mean 0b10 or 0b01? I'm assuming it's the former.
    public bool IsSpiked()
    {
        if (GetBits(Owner_or_special, 0b10, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public void AddSpike()
    {
        SetBits(Owner_or_special, 0b1, 0b10, 0);
        UpdateBuffer();
    }

    public void RemoveSpike()
    {
        SetBits(Owner_or_special, 0b0, 0b10, 0);
        UpdateBuffer();
    }

    // On locks:
    // Bit 9 of the flags indicates if the lock is locked (1) or unlocked (0).
    // Lower 6 bits of link/special field determines the lock id. When unlocking,
    // the ID must match the key ID on the key
    public void Unlock(GameObject[] levelGameObjects)
    {
        // TODO: Make a lock class and implement functions to change ID and lock/unlock
        throw new NotImplementedException();
        // GameObject Lock = levelGameObjects[link_specialField];
        // SetBits(Lock.objid_flagsField, 0, 0b1, 9);
        // Lock.UpdateBuffer();
    }
    
}