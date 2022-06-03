using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using static Randomizer.Utils;

namespace RandomizerUnitTests;

internal class TestSetBits
{
    [Test]
    public void TestSetBits1()
    {
        byte currval = 0b0001;
        byte mask = 0b0011;
        byte newval = 0b0011;
        byte shift = 1;
        byte expected = 0b0111;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");

    }
    [Test]
    public void TestSetBits2() 
    { 

        byte currval  = 0b0001;
        byte mask     = 0b0110;
        byte newval   = 0b0011;
        byte shift    = 0;
        byte expected = 0b0011;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }

    [Test]
    public void TestSetBits3()
    {
        byte currval  = 0b1111;
        byte mask     = 0b0011;
        byte newval   = 0b1100;
        byte shift    = 1;
        byte expected = 0b1001;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }

    [Test]
    public void TestSetBits4()
    {
        byte currval  = 0b1111;
        byte mask     = 0b0110;
        byte newval   = 0b0000;
        byte shift    = 0;
        byte expected = 0b1001;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }

    [Test]
    public void TestSetBits5()
    {
        byte currval  = 0b1111_1111;
        byte mask     = 0b0011_1100;
        byte newval   = 0b0000_0000;
        byte shift    = 0;
        byte expected = 0b1100_0011;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }
    [Test]
    public void TestSetBits6()
    {
        byte currval  = 0b1111_1111;
        byte mask     = 0b0000_1111;
        byte newval   = 0b0000_0000;
        byte shift    = 2;
        byte expected = 0b1100_0011;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }

    [Test]
    public void TestSetBits7()
    {
        byte currval  = 0b1100_1110;
        byte mask     = 0b0011_1000;
        byte newval   = 0b1001_0000;
        byte shift    = 0;
        byte expected = 0b1101_0110;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }
    [Test]
    public void TestSetBits8()
    {
        byte currval  = 0b1111_0000;
        byte mask     = 0b0000_1111;
        byte newval   = 0b0000_1010;
        byte shift    = 4;
        byte expected = 0b1010_0000;
        int calc = SetBits(currval, newval, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }


}

internal class TestGetBits
{
    [Test]
    public void TestGetBits1()
    {
        int value    = 0b1010;
        int mask     = 0b0011;
        int shift    = 0;
        int expected = 0b0010;
        int calc     = GetBits(value, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }
    [Test]
    public void TestGetBits2()
    {
        int value    = 0b0000;
        int mask     = 0b0011;
        int shift    = 0;
        int expected = 0b0000;
        int calc     = GetBits(value, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }

    [Test]
    public void TestGetBits3()
    {
        int value    = 0b1100_1001;
        int mask     = 0b1111;
        int shift    = 4;
        int expected = 0b1100;
        int calc     = GetBits(value, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }

    [Test]
    public void TestGetBits4()
    {
        int value    = 0b1010_0101;
        int mask     = 0b1111;
        int shift    = 2;
        int expected = 0b1001;
        int calc     = GetBits(value, mask, shift);
        Assert.AreEqual(expected, calc, $"{expected:x}!={calc:x}");
    }


}
