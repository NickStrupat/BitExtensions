using System;
using Xunit;

namespace NickStrupat.BitExtensions_Tests;

public class ToBitStringTests
{
	[Fact]
	public void ToBitString_WithByte_ReturnsCorrectString()
	{
		Byte b = 0b_0000_0001;
		Assert.Equal("00000001", b.ToBitString());
	}
	
	[Fact]
	public void ToBitString_WithShort_ReturnsCorrectString()
	{
		Int16 i = 0b_0010_0101_0011_1101;
		Assert.Equal("0010010100111101", i.ToBitString());
	}
	
	[Fact]
	public void ToBitString_WithInt_ReturnsCorrectString()
	{
		Int32 i = 0b_0000_0001_0000_0010_0000_0100_0000_1000;
		Assert.Equal("00000001000000100000010000001000", i.ToBitString());
	}
	
	[Fact]
	public void ToBitString_WithLong_ReturnsCorrectString()
	{
		Int64 l = 0b_0000_0001_0000_0010_0000_0100_0000_1000_0000_0001_0000_0010_0000_0100_0000_1000;
		Assert.Equal("0000000100000010000001000000100000000001000000100000010000001000", l.ToBitString());
	}
}