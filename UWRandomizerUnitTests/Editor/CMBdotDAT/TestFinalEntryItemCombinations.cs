using NUnit.Framework;
using System.Linq;
using System.Text.Json;
using UWRandomizerEditor;
using UWRandomizerEditor.CMBdotDAT;

namespace RandomizerUnitTests;

[TestFixture]
public class TestFinalEntry
{
    [Test]
    public void TestFinalEntryCtor()
    {
        var entry = new FinalEntry();
        Assert.True(entry.ItemID == 0);
        Assert.True(entry.IsDestroyed == false);
        // Assert.True((from v in entry.buffer where v == 0 select v).All(x => x == 0));
        Assert.True(entry.Buffer.All(x => x == 0));
    }

    [Test]
    public void TestFinalEntryCtorWithArgs()
    {
        var entry = new FinalEntry();
        Assert.True(entry.ItemID == 0);
        Assert.True(entry.IsDestroyed == false);
        Assert.True(entry.Buffer.All(x => x == 0));
    }

    [Test]
    public void TestFinalEntryJsonSerialize() // TODO: This is not the right place to have this test
    {
        var entry = new FinalEntry();
        var json = JsonSerializer.Serialize(entry, options: new JsonSerializerOptions() {WriteIndented = true});
        var expectedJson = @"{
  ""ItemID"": 0,
  ""IsDestroyed"": false
}";
        Assert.True(json == expectedJson);
    }
}
