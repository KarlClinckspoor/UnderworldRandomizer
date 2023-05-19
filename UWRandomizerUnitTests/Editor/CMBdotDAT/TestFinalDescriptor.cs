using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using UWRandomizerEditor.CMBdotDAT;

namespace RandomizerUnitTests.Editor.CMBdotDAT;

[TestFixture]
public class TestFinalEntry
{
    [Test]
    public void TestFinalEntryCtor()
    {
        var entry = new FinalDescriptor();
        Assert.True(entry.ItemID == 0);
        Assert.True(entry.IsDestroyed == false);
        Assert.True(entry.Buffer.All(x => x == 0));
    }

    [Test]
    public void TestFinalEntryJsonSerialize()
    {
        var entry = new FinalDescriptor();
        var json = JsonSerializer.Serialize(entry, options: new JsonSerializerOptions() {WriteIndented = true});
        var expectedJson = @"{
  ""ItemID"": 0,
  ""IsDestroyed"": false
}";
        Assert.True(json == expectedJson);
    }
}
