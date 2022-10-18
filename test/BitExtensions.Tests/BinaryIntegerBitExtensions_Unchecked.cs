using System;
using Xunit;

namespace NickStrupat.BitExtensions.Tests;

public class BinaryIntegerBitExtensions_Unchecked
{
	[Theory]
	[InlineData(0b_0000_0000, 0, false)]
	[InlineData(0b_0000_0001, 0, true)]
	[InlineData(0b_0000_0000, 1, false)]
	[InlineData(0b_0000_0010, 1, true)]
	[InlineData(0b_0000_0000, 7, false)]
	[InlineData(0b_1000_0000, 7, true)]
	[InlineData(0b_0000_0000, 8, false)]
	[InlineData(0b_1111_1111, 8, false)]
	[InlineData(0b_0000_0000, 11, false)]
	[InlineData(0b_1111_1111, 11, false)]
	[InlineData(0b_1111_1111, -1, false)]
	[InlineData(0b_1111_1111, -125, false)]
	public void ReadBitUnchecked(Byte value, Int32 index, Boolean expected)
	{
		Assert.Equal(expected, value.ReadBitUnchecked(index));
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
	[InlineData(0b_0000_0000, 8, true, 0b_0000_0000)]
	[InlineData(0b_1111_1111, 8, true, 0b_1111_1111)]
	[InlineData(0b_0000_0000, 8, false, 0b_0000_0000)]
	[InlineData(0b_1111_1111, 8, false, 0b_1111_1111)]
	public void WriteBitUnchecked(Byte value, Int32 index, Boolean bit, Byte expected)
	{
		value.WriteBitUnchecked(index, bit);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0b_1111_1111_1111_1110, 0, 5, 0b_0000_0000_0001_1110)]
	[InlineData(0b_1111_1111_1111_1110, 1, 7, 0b_0000_0000_0111_1111)]
	public void ReadBitsUnchecked(UInt16 value, Int32 index, Int32 count, UInt16 expected)
	{
		Assert.Equal(expected, value.ReadBitsUnchecked(index, count));
	}
	
	[Theory]
	[InlineData(0b_1111_1111_1111_1111, 0, 0, 0b_0000_0000_0000_0001, 0b_1111_1111_1111_1111)]
	[InlineData(0b_1111_1111_1111_1111, 0, 0, 0b_0000_0000_0000_0000, 0b_1111_1111_1111_1111)]
	[InlineData(0b_1111_1111_0000_1111, 5, 2, 0b_0000_0000_0000_0011, 0b_1111_1111_0110_1111)]
	public void WriteBitsUnchecked(UInt16 value, Int32 index, Int32 count, UInt16 bits, UInt16 expected)
	{
		value.WriteBitsUnchecked(index, count, bits);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0, 0, 1)]
	[InlineData(0b_01, 1, 0b_11)]
	public void SetBitUnchecked(UInt16 value, Int32 index, UInt16 expected)
	{
		value.SetBitUnchecked(index);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0b_11, 0, 0b_10)]
	[InlineData(0b_11, 1, 0b_01)]
	public void ClearBitUnchecked(UInt16 value, Int32 index, UInt16 expected)
	{
		value.ClearBitUnchecked(index);
		Assert.Equal(expected, value);
	}
	
	[Theory]
	[InlineData(0b_11, 0, 0b_10)]
	[InlineData(0b_10, 0, 0b_11)]
	[InlineData(0b_11, 1, 0b_01)]
	[InlineData(0b_01, 1, 0b_11)]
	public void ToggleBitUnchecked(UInt16 value, Int32 index, UInt16 expected)
	{
		value.ToggleBitUnchecked(index);
		Assert.Equal(expected, value);
	}
}