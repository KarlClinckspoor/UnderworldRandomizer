using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

namespace RandomizerUnitTests.Editor.LEV;

[TestFixture]
public class TestUWLinkedList_Containers
{
#pragma warning disable CS8618
    private Container _bag1;
    private Tile _tile1;
    private Container _bag2;
    private GameObject[] _gameObjects;
#pragma warning restore CS8618
    
    /// <summary>
    /// Contents
    /// bag2(5) -> end(0)
    ///    |
    ///    -> bag1(4) -> end(0)
    ///        |
    ///        -> Key(1) -> EnchantedWeapon(2) -> EnchantedArmor(3) -> end(0)
    /// Tile either points to bag1 or bag2 (depends on test)
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var tempbuffer = new byte[8];
        _bag1 = new Container(tempbuffer, 4)
        {
            QuantityOrSpecialLinkOrSpecialProperty = 1,
            Contents = new UWLinkedList() {StartingIdx = 1, RepresentingContainer = true}
        };
        _bag2 = new Container(tempbuffer, 5)
        {
            QuantityOrSpecialLinkOrSpecialProperty = 4,
            Contents = new UWLinkedList() {StartingIdx = 4, RepresentingContainer = true}
        }; // for bag in bag

        _gameObjects = new GameObject[]
        {
            new StaticObject(tempbuffer, 0), // Empty obj
            new Key(tempbuffer, 1) {next = 2},
            new EnchantedWeapon(tempbuffer, 2) {next = 3},
            new EnchantedArmor(tempbuffer, 3) {next = 0},
            _bag1,
            _bag2,
            new Door(tempbuffer, 6),
        };
        _tile1 = new Tile(0, 0, 0, 0) {FirstObjIdx = 4};
    }

    [Test]
    public void TestAddingToContainer()
    {
        _bag1.Contents.PopulateObjectList(_gameObjects);
        Assert.True(_bag1.Contents.Count == 3);
        Assert.True(_bag1.Contents[0].Equals(_gameObjects[1]));
        Assert.True(_bag1.Contents[1].Equals(_gameObjects[2]));
        Assert.True(_bag1.Contents[2].Equals(_gameObjects[3]));
        Assert.False( (from obj in _gameObjects where obj.InContainer select 1).Sum() != 3 );
        Assert.True( (from obj in _gameObjects where obj.InContainer select 1).Sum() == 3 );
        Assert.True( _gameObjects.Where(x=>x.InContainer).All(x=>x.ReferenceCount == 1));
    }

    [Test]
    public void TestBagInBag()
    {
        _bag1.Contents.PopulateObjectList(_gameObjects);
        _bag2.Contents.PopulateObjectList(_gameObjects);
        Assert.True(_bag2.Contents.Count == 1);
        Assert.True(_bag2.Contents[0].Equals(_bag1));
        Assert.False( (from obj in _gameObjects where obj.InContainer select 1).Sum() != 4 );
        Assert.True( (from obj in _gameObjects where obj.InContainer select 1).Sum() == 4 );
        Assert.True( _gameObjects.Where(x=>x.InContainer).All(x=>x.ReferenceCount == 1));
    }

    [Test]
    public void TestContainerInTile()
    {
        _bag1.Contents.PopulateObjectList(_gameObjects);
        _tile1.ObjectChain.PopulateObjectList(_gameObjects);
        Assert.False( (from obj in _gameObjects where obj.InContainer select 1).Sum() != 3 );
        Assert.True( (from obj in _gameObjects where obj.InContainer select 1).Sum() == 3 );
        Assert.True( _gameObjects.Where(x=>x.InContainer).All(x=>x.ReferenceCount == 1));
    }
    
    [Test]
    public void TestBagInBagInTile()
    {
        _bag1.Contents.PopulateObjectList(_gameObjects);
        _bag2.Contents.PopulateObjectList(_gameObjects);
        _tile1.FirstObjIdx = _bag2.IdxAtObjectArray;
        _tile1.ObjectChain.PopulateObjectList(_gameObjects);
        Assert.False( (from obj in _gameObjects where obj.InContainer select 1).Sum() != 4 );
        Assert.True( (from obj in _gameObjects where obj.InContainer select 1).Sum() == 4 );
        Assert.True( _gameObjects.Where(x=>x.InContainer).All(x=>x.ReferenceCount == 1));
    }
    
}