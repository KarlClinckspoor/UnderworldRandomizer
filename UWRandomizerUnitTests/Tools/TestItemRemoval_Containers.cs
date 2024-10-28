using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerEditor.LEV.GameObjects.Specifics;
using UWRandomizerTools;

namespace RandomizerUnitTests.Tools;

[TestFixture]
public class TestItemRemoval_Containers
{
#pragma warning disable CS8618
    private Container _bag1;
    private Tile _tile1;
    private Container _bag2;
    private GameObject[] _gameObjects;
#pragma warning restore CS8618
    
    
    /// <summary>
    /// Item sequence:
    /// Tile -> bag2(5)  -> Door(6) -> Trap(7) -> EnchantedWeapon(8) -> Trigger(10) -> end(0)
    ///          | (in)
    ///          -> bag1(4)    -> end(0)
    ///              | (in)
    ///              -> Key(1) -> EnchantedWeapon(2) -> EnchantedArmor(3) -> end(0)
    ///
    /// Movables: bag2, EnchantedWeapon
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
            Contents = new UWLinkedList() {StartingIdx = 4, RepresentingContainer = true},
            next = 6,
        }; // for bag in bag

        _gameObjects = new GameObject[]
        {
            new StaticObject(tempbuffer, 0), // Empty obj
            new Key(tempbuffer, 1) {next = 2},
            new EnchantedWeapon(tempbuffer, 2) {next = 3},
            new EnchantedArmor(tempbuffer, 3) {next = 0},
            _bag1,
            _bag2,
            new Door(tempbuffer, 6) {next = 7},
            new Trap(tempbuffer, 7) {next = 8},
            new EnchantedWeapon(tempbuffer, 8) {next = 10},
            new TexturedGameObject(tempbuffer, 9) {next = 0}, // Unused on purpose
            new Trigger(tempbuffer, 10) {next = 0}
        };
    }

    [Test]
    public void TestMovables()
    {
        _tile1 = new Tile(0, 0, 0, 0) {FirstObjIdx = 5};
        _tile1.ObjectChain.PopulateObjectList(_gameObjects);
        _bag1.Contents.PopulateObjectList(_gameObjects);
        _bag2.Contents.PopulateObjectList(_gameObjects);

        List<GameObject> movables = ItemTools.ExtractMovableItems(_tile1, new ItemRandomizationSettings());
        Assert.True(movables[0].Equals(_bag2));
        Assert.True(movables[1].Equals(_gameObjects[8]));
        Assert.True( (from gobj in _gameObjects where gobj.InContainer select 1).Sum() == 4 );
        Assert.False( (from gobj in _gameObjects where gobj.InContainer select 1).Sum() != 4 );
    }
    
}