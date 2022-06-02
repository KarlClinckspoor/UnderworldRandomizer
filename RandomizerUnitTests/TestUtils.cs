using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using static Randomizer.Utils;

namespace RandomizerUnitTests;

internal class TestUtils
{
    [Test]
    public void TestSetBits()
    {
        byte currval1 = 0b0001;
        byte mask1 = 0b0011;
        byte newval1 = 0b0011;
        byte shift1 = 1;
        byte expected1 = 0b0111;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");

    }
    [Test]
    public void TestSetBits2() 
    { 

        byte currval1  = 0b0001;
        byte mask1     = 0b0110;
        byte newval1   = 0b0011;
        byte shift1    = 0;
        byte expected1 = 0b0011;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }

    [Test]
    public void TestSetBits3()
    {
        byte currval1  = 0b1111;
        byte mask1     = 0b0011;
        byte newval1   = 0b1100;
        byte shift1    = 1;
        byte expected1 = 0b1001;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }

    [Test]
    public void TestSetBits4()
    {
        byte currval1  = 0b1111;
        byte mask1     = 0b0110;
        byte newval1   = 0b0000;
        byte shift1    = 0;
        byte expected1 = 0b1001;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }

    [Test]
    public void TestSetBits5()
    {
        byte currval1  = 0b1111_1111;
        byte mask1     = 0b0011_1100;
        byte newval1   = 0b0000_0000;
        byte shift1    = 0;
        byte expected1 = 0b1100_0011;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }
    [Test]
    public void TestSetBits6()
    {
        byte currval1  = 0b1111_1111;
        byte mask1     = 0b0000_1111;
        byte newval1   = 0b0000_0000;
        byte shift1    = 2;
        byte expected1 = 0b1100_0011;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }

    [Test]
    public void TestSetBits7()
    {
        byte currval1  = 0b1100_1110;
        byte mask1     = 0b0011_1000;
        byte newval1   = 0b1001_0000;
        byte shift1    = 0;
        byte expected1 = 0b1101_0110;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }
    [Test]
    public void TestSetBits8()
    {
        byte currval1  = 0b1111_0000;
        byte mask1     = 0b0000_1111;
        byte newval1   = 0b0000_1010;
        byte shift1    = 4;
        byte expected1 = 0b1010_0000;
        int calc1 = SetBits(currval1, newval1, mask1, shift1);
        Assert.AreEqual(expected1, calc1, $"{expected1:x}!={calc1:x}");
    }


}
