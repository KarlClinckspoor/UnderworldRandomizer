namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Model: StaticObject
{
    public Model(byte[] buffer, ushort IdxAtObjArray) : base(buffer, IdxAtObjArray) { }

}

public class ContainerModel : Model, IContainer
{
    public UWLinkedList Contents { get; set; }

    public ContainerModel(byte[] buffer, ushort IdxAtObjArray) : base(buffer, IdxAtObjArray)
    {
        Contents = new UWLinkedList() {StartingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }
}