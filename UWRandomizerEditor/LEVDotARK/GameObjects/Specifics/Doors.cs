namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class Door : SpecialLinkGameObject
{
    public Door(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public override bool ShouldBeMoved { get; set; } = false;

    // On doors:
    // Bit 1 of "owner" field is set, door is spiked
    // sp_link field points to a_lock object (010f), door is locked.

    /// <summary>
    /// Points the "sp_link" field to 0, removing any reference to a lock.
    /// </summary>
    public void RemoveLock()
    {
        LinkSpecial = 0;
        ReconstructBuffer();
    }

    // TODO: add verification
    /// <summary>
    /// Checks if sp_link (linkSpecial) points to something other than 0. If so, returns the value, which
    /// is the index of the lock.
    /// </summary>
    public bool HasLock(out ushort LockIdx)
    {
        LockIdx = LinkSpecial;
        return HasLock();
    }

    /// <summary>
    /// Checks if sp_link (linkSpecial) points to something other than 0.
    /// </summary>
    public bool HasLock()
    {
        if (LinkSpecial != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Replaces sp_link (linkSpecial) with the value provided. Does not check if it really points to a lock object.
    /// </summary>
    /// <param name="lock_idx"></param>
    public void AddLock(ushort lock_idx)
    {
        // TODO: add some checks
        LinkSpecial = lock_idx;
        ReconstructBuffer();
    }

    // TODO: By "bit 1" does it mean 0b10 or 0b01? I'm assuming it's the former.
    public bool IsSpiked()
    {
        if (Utils.GetBits(OwnerOrSpecial, 0b10, 0) == 1)
        {
            return true;
        }

        return false;
    }

    public void AddSpike()
    {
        Utils.SetBits(OwnerOrSpecial, 0b1, 0b10, 0);
        ReconstructBuffer();
    }

    public void RemoveSpike()
    {
        Utils.SetBits(OwnerOrSpecial, 0b0, 0b10, 0);
        ReconstructBuffer();
    }

    public void Unlock(GameObject[] blockGameObjects)
    {
        if (blockGameObjects[LinkSpecial] is Lock LockObject)
        {
            LockObject.IsLocked = false;
            LockObject.ReconstructBuffer();
        }
        else
        {
            throw new InvalidOperationException("Cannot unlock because door isn't pointing to a lock object");
        }
    }
}