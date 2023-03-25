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
        Assert.True(entry.itemID == 0);
        Assert.True(entry.IsDestroyed == false);
        // Assert.True((from v in entry.buffer where v == 0 select v).All(x => x == 0));
        Assert.True(entry.buffer.All(x => x == 0));
    }

    [Test]
    public void TestFinalEntryCtorWithArgs()
    {
        var entry = new FinalEntry(1234, true);
        Assert.True(entry.itemID == 0);
        Assert.True(entry.IsDestroyed == false);
        Assert.True(entry.buffer.All(x => x == 0));
    }

    [Test]
    public void TestFinalEntryJsonSerialize() // TODO: This is not the right place to have this test
    {
        var entry = new FinalEntry();
        var json = JsonSerializer.Serialize(entry, options: new JsonSerializerOptions() {WriteIndented = true});
        var expectedJson = @"{
  ""itemID"": 0,
  ""IsDestroyed"": false
}";
        Assert.True(json == expectedJson);
    }
}

[TestFixture]
public class TestItemDescriptor
{
    [Test]
    public void TestDescriptorCtorDefault()
    {
        var itemDescriptor = new ItemDescriptor();
        Assert.True(itemDescriptor.itemID == 0);
        Assert.True(itemDescriptor.IsDestroyed == false);
        Assert.True(itemDescriptor.buffer.All(x => x == 0));
    }

    [Test]
    public void TestDescriptorCtor2Props()
    {
        var itemDescriptor = new ItemDescriptor(1, false);
        var itemDescriptor2 = new ItemDescriptor(1, true);

        Assert.True(itemDescriptor.itemID == 1);
        Assert.True(itemDescriptor.IsDestroyed == false);
        Assert.True(itemDescriptor.buffer[0] == 1);
        Assert.True(itemDescriptor.buffer[1] == 0);

        Assert.True(itemDescriptor2.itemID == 1);
        Assert.True(itemDescriptor2.IsDestroyed == true);
        Assert.True(itemDescriptor2.buffer[0] == 1);
        Assert.True(itemDescriptor2.buffer[1] == 0x80);

        var itemDescriptor3 = new ItemDescriptor(300, false);
        var itemDescriptor4 = new ItemDescriptor(300, true);

        Assert.True(itemDescriptor3.itemID == 300); // Testing outside the limit of 1 byte
        Assert.True(itemDescriptor3.IsDestroyed == false);
        Assert.True(itemDescriptor3.buffer[0] == 0b0010_1100);
        Assert.True(itemDescriptor3.buffer[1] == 0b0000_0001);

        Assert.True(itemDescriptor4.itemID == 300);
        Assert.True(itemDescriptor4.IsDestroyed == true);
        Assert.True(itemDescriptor4.buffer[0] == 0b0010_1100);
        Assert.True(itemDescriptor4.buffer[1] == 0b1000_0001);
    }

    [Test]
    public void TestDescriptorCtorBuffer()
    {
        var itemDescriptor = new ItemDescriptor(new byte[] {1, 0});
        var itemDescriptor2 = new ItemDescriptor(new byte[] {1, 0x80});

        Assert.True(itemDescriptor.itemID == 1);
        Assert.True(itemDescriptor.IsDestroyed == false);

        Assert.True(itemDescriptor2.itemID == 1);
        Assert.True(itemDescriptor2.IsDestroyed == true);
    }
}