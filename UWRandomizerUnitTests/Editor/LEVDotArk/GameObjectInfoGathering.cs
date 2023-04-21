using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;

namespace RandomizerUnitTests.Editor.LEVDotArk;

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
        var AL = new ArkLoader(Paths.UW_ArkOriginalPath);
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

        for (int lvl = 0; lvl < AL.TileMapObjectsBlocks.Length; lvl++)
        {
            var block = AL.TileMapObjectsBlocks[lvl];
            extraCSVcontent.AppendLine(
                                       $"Block: {block.LevelNumber}," +
                                       $"First Free Mobile Slot {block.FirstFreeMobileSlot}," +
                                       $"First Free Static Slot {block.FirstFreeStaticSlot}," +
                                       $"First Free Mobile Slot entry {block.AllFreeObjectSlots[block.FirstFreeMobileSlot]}," +
                                       $"First Free Static Slot entry {block.AllFreeObjectSlots[block.FirstFreeStaticSlot]}," +
                                       $"First Free Mobile Index {block.FirstFreeMobileObjectIdx}," +
                                       $"First Free Static Index {block.FirstFreeStaticObjectIdx}," +
                                       // $"First Free Static Index2 {block.FirstFreeStaticObjectIdx2}," +
                                       $"First Free Mobile ObjIdx {block.FirstFreeMobileObject.IdxAtObjectArray}" +
                                       $"First Free Static ObjIdx {block.FirstFreeStaticObject.IdxAtObjectArray}"
                                       );
            
            foreach (var gameObject in block.AllGameObjects)
            {
                var isReferencedByFreeSlot = block.AllFreeObjectSlots.Any(x => x.IdxAtArray == gameObject.IdxAtObjectArray);
                var whichTileReferencesGameObject =
                    block.Tiles
                        .Where(x => x.ObjectChain.Any(
                            y => y.IdxAtObjectArray == gameObject.IdxAtObjectArray)
                        ).Select(x => $"{x.XPos},{x.YPos}");
                mainCSVcontent.AppendLine(
                    $"{lvl};" +
                    $"{gameObject.IdxAtObjectArray};" +
                    $"{gameObject.ItemID};" +
                    $"{gameObject.next};" +
                    $"{gameObject.ReferenceCount};" +
                    $"({string.Join(" ", whichTileReferencesGameObject)});"+
                    $"{gameObject.InContainer};"+
                    $"{isReferencedByFreeSlot};"+
                    $"{gameObject.Invalid};" +
                    $"{gameObject}"
                    );
            }
        }
        File.WriteAllText("./GameObjectStudy.csv", mainCSVcontent.ToString());
        File.WriteAllText("./GameObjectStudyExtra.txt", extraCSVcontent.ToString());
    }
}