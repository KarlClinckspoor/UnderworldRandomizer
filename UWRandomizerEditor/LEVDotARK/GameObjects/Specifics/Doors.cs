namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;
using static UWRandomizerEditor.Utils;

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
    public ushort HasLock()
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
    public void AddLock(ushort lock_idx)
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

    public void Unlock(GameObject[] blockGameObjects)
    {
        if (blockGameObjects[link_specialField] is Lock LockObject)
        {
            LockObject.IsLocked = false;
            LockObject.UpdateBuffer();
        }
        else
        {
            throw new InvalidOperationException("Cannot unlock because door isn't pointing to a lock object");
        }
    }
    
}