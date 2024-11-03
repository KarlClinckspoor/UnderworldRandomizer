using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UWRandomizerEditor.LEV;

namespace RandomizerUnitTests.Editor.LEV;

/// <summary>
/// The purpose of this is to evaluate properties of the GameObjects and see if I'm implementing the free slots and
/// indexes correctly
/// </summary>
[TestFixture]
[Category("RequiresSettings")]
public class GameObjectInfoGathering
{
    [Test]
    public void MakeCSVFile()
    {
        var AL = Utils.LoadAndAssertOriginalLevArk();
        Assert.True(Utils.CheckEqualityOfSha256Hash(AL.Buffer, Utils.OriginalLevArkSha256Hash));
        
        var mainCSVcontent = new StringBuilder();
        mainCSVcontent.AppendLine(
            "level;" +
            "index;" +
            "id;" +
            "next;" +
            "refCount;" +
            "inTile;" +
            "inContainer;" +
            "isReferencedByFreeSlot;"+
            "invalid;" +
            "internalName"
            );
        var extraCSVcontent = new StringBuilder();
        extraCSVcontent.AppendLine("" +
                                   "Block;" +
                                   "FirstFreeMobileSlot;" +
                                   "FirstFreeStaticSlot;" +
                                   "FirstFreeMobileSlotEntry;" +
                                   "FirstFreeStaticSlotEntry;" +
                                   "FirstFreeMobileIndex;" +
                                   "FirstFreeStaticIndex;" +
                                   "FirstFreeMobileObjIdx;" +
                                   "FirstFreeStaticObjIdx" +
                                   "");

        for (int lvl = 0; lvl < AL.TileMapObjectsBlocks.Length; lvl++)
        {
            var block = AL.TileMapObjectsBlocks[lvl];
            extraCSVcontent.AppendLine(
                                       $"{block.LevelNumber};" +
                                       $"{block.IdxLookupOfFreeMobileObject};" +
                                       $"{block.IdxLookupOfFreeStaticObject};" +
                                       $"{block.IndicesOfFreeObjects[block.IdxLookupOfFreeMobileObject]};" +
                                       $"{block.IndicesOfFreeObjects[block.IdxLookupOfFreeStaticObject]};" +
                                       $"{block.IdxOfFreeMobileObject};" +
                                       $"{block.IdxOfFreeStaticObject};" +
                                       $"{block.CurrentFreeMobileObject.IdxAtObjectArray};" +
                                       $"{block.CurrentFreeStaticObject.IdxAtObjectArray}"
                                       );
            
            foreach (var gameObject in block.AllGameObjects)
            {
                var isReferencedByFreeSlot = block.IndicesOfFreeObjects.Any(x => x == gameObject.IdxAtObjectArray);
                var whichTileReferencesGameObject =
                    block.Tiles
                        .Where(x => x.ObjectChain.Any(
                            y => y.IdxAtObjectArray == gameObject.IdxAtObjectArray)
                        ).Select(x => $"{x.Row},{x.Column}");
                mainCSVcontent.AppendLine(
                    $"{lvl};" +
                    $"{gameObject.IdxAtObjectArray};" +
                    $"{gameObject.ItemID};" +
                    $"{gameObject.next};" +
                    $"{gameObject.ReferenceCount};" +
                    $"({string.Join(" ", whichTileReferencesGameObject)});"+
                    $"{gameObject.InContainer};"+
                    $"{isReferencedByFreeSlot};"+
                    $"{gameObject}"
                    );
            }
        }
        File.WriteAllText("./GameObjectStudy.csv", mainCSVcontent.ToString());
        File.WriteAllText("./GameObjectStudyExtra.txt", extraCSVcontent.ToString());
    }
}