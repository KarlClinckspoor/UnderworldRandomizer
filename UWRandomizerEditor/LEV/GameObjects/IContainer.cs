namespace UWRandomizerEditor.LEV.GameObjects;

public interface IContainer
{
    /// <summary>
    /// Represents the contents of a container (bag, box, etc) or the inventory of an NPC.
    /// </summary>
    public UWLinkedList Contents { get; set; }
}