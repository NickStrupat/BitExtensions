using System;
using Xunit;

namespace NickStrupat.BitExtensions.Tests;

public class BinaryIntegerBitExtensions
{
	[Theory]
	[InlineData(0b_0000_0000, 0, false)]
	[InlineData(0b_0000_0001, 0, true)]
	[InlineData(0b_0000_0000, 1, false)]
	[InlineData(0b_0000_0010, 1, true)]
	[InlineData(0b_0000_0000, 7, false)]
	[InlineData(0b_1000_0000, 7, true)]
	public void ReadBit(Byte value, Int32 index, Boolean expected)
	{
		Assert.Equal(expected, value.ReadBit(index));
	}
	
	[Theory]
	[InlineData(0b_0000_0000, 8)]
	[InlineData(0b_1111_1111, 8)]
	[InlineData(0b_0000_0000, 11)]
	[InlineData(0b_1111_1111, 11)]
	public void ReadBit_ThrowsIndexOfOutRange(Byte value, Int32 index)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => value.ReadBit(index));
	}
	
	[Theory]
	[InlineData(0b_0000_0000, 0, true, 0b_0000_0001)]
	[InlineData(0b_1111_1111, 0, true, 0b_1111_1111)]
	[InlineData(0b_0000_0000, 0, false, 0b_0000_0000)]
	[InlineData(0b_1111_1111, 0, false, 0b_1111_1110)]
	[InlineData(0b_0000_0000, 1, true, 0b_0000_0010)]
	[InlineData(0b_1111_1111, 1, true, 0b_1111_1111)]
	[InlineData(0b_0000_0000, 1, false, 0b_0000_0000)]
	[InlineData(0b_1111_1111, 1, false, 0b_1111_1101)]
	[InlineData(0b_0000_0000, 7, true, 0b_1000_0000)]
	[InlineData(0b_1111_1111, 7, true, 0b_1111_1111)]
	[InlineData(0b_0000_0000, 7, false, 0b_0000_0000)]
	[InlineData(0b_1111_1111, 7, false, 0b_0111_1111)]
	public void WriteBit(Byte value, Int32 index, Boolean bit, Byte expected)
	{
		value.WriteBit(index, bit);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0b_0000_0000, 8, true)]
	[InlineData(0b_1111_1111, 8, true)]
	[InlineData(0b_0000_0000, 8, false)]
	[InlineData(0b_1111_1111, 8, false)]
	public void WriteBit_ThrowsIndexOfOutRange(Byte value, Int32 index, Boolean bit)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => value.WriteBit(index, bit));
	}
	
	[Theory]
	[InlineData(0b_1111_1111_1111_1110, 0, 5, 0b_0000_0000_0001_1110)]
	[InlineData(0b_1111_1111_1111_1110, 1, 7, 0b_0000_0000_0111_1111)]
	[InlineData(0b_1111_1111_1111_1110, 15, 1, 0b_0000_0000_0000_0001)]
	[InlineData(0b_1111_1111_1111_1110, 13, 3, 0b_0000_0000_0000_0111)]
	[InlineData(0b_1111_1001_1111_1110, 0, 16, 0b_1111_1001_1111_1110)]
	public void ReadBits(UInt16 value, Int32 index, Int32 count, UInt16 expected)
	{
		Assert.Equal(expected, value.ReadBits(index, count));
	}
	
	[Theory]
	[InlineData(0b_1111_1111_1111_1110, 13, 4)]
	[InlineData(0b_1111_1111_1111_1110, 1, 16)]
	[InlineData(0b_1111_1111_1111_1110, 17, 0)]
	[InlineData(0b_1111_1111_1111_1110, -1, 1)]
	[InlineData(0b_1111_1111_1111_1110, 0, -1)]
	public void ReadBits_ThrowsIndexOfOutRange(UInt16 value, Int32 index, Int32 count)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => value.ReadBits(index, count));
	}
	
	[Theory]
	[InlineData(0b_1111_1111_1111_1111, 0, 0, 0b_0000_0000_0000_0001, 0b_1111_1111_1111_1111)]
	[InlineData(0b_1111_1111_1111_1111, 0, 0, 0b_0000_0000_0000_0000, 0b_1111_1111_1111_1111)]
	[InlineData(0b_1111_1111_0000_1111, 5, 2, 0b_0000_0000_0000_0011, 0b_1111_1111_0110_1111)]
	[InlineData(0b_0000_0000_0000_0000, 15, 1, 0b_0000_0000_0000_0001, 0b_1000_0000_0000_0000)]
	public void WriteBits(UInt16 value, Int32 index, Int32 count, UInt16 bits, UInt16 expected)
	{
		value.WriteBits(index, count, bits);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0b_1111_1111_1111_1111, 15, 0, 0b_0000_0000_0000_0001, 0b_1111_1111_1111_1111)]
	[InlineData(0b_1111_1111_1111_1111, 0, 0, 0b_0000_0000_0000_0000, 0b_1111_1111_1111_1111)]
	[InlineData(0b_1111_1111_0000_1111, 5, 2, 0b_0000_0000_0000_0011, 0b_1111_1111_0110_1111)]
	public void WriteBits_ThrowsIndexOfOutRange(UInt16 value, Int32 index, Int32 count, UInt16 bits, UInt16 expected)
	{
		value.WriteBits(index, count, bits);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0, 0, 1)]
	[InlineData(0b_01, 1, 0b_11)]
	public void SetBit(UInt16 value, Int32 index, UInt16 expected)
	{
		value.SetBit(index);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(16)]
	[InlineData(54321)]
	[InlineData(-1)]
	[InlineData(-54321)]
	public void SetBit_ThrowsIndexOfOutRange(Int32 index)
	{
		UInt16 i = 0;
		Assert.Throws<ArgumentOutOfRangeException>(() => i.SetBit(index));
	}
	
	[Theory]
	[InlineData(0b_11, 0, 0b_10)]
	[InlineData(0b_11, 1, 0b_01)]
	public void ClearBit(UInt16 value, Int32 index, UInt16 expected)
	{
		value.ClearBit(index);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0b_11, 0, 0b_10)]
	[InlineData(0b_10, 0, 0b_11)]
	[InlineData(0b_11, 1, 0b_01)]
	[InlineData(0b_01, 1, 0b_11)]
	public void ToggleBit(UInt16 value, Int32 index, UInt16 expected)
	{
		value.ToggleBit(index);
		Assert.Equal(expected, value);
	}
}