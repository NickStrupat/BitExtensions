using System;
using Xunit;

namespace NickStrupat.BitExtensions_Tests
{
	public class SetBitTests
	{
		[Theory]
		[InlineData(0b0000_0001, 0)]
		[InlineData(0b0000_0010, 1)]
		[InlineData(0b0000_0100, 2)]
		[InlineData(0b0000_1000, 3)]
		[InlineData(0b0001_0000, 4)]
		[InlineData(0b0010_0000, 5)]
		[InlineData(0b0100_0000, 6)]
		[InlineData(0b1000_0000, 7)]
		public void SetBit_ValidIndex(Byte expected, Byte index)
		{
			Byte @byte = 0b0000_0000;
			@byte.SetBit(index);
			Assert.Equal(expected, @byte);
		}
		
		[Theory]
		[InlineData(8)]
		[InlineData(9)]
		[InlineData(64)]
		[InlineData(127)]
		[InlineData(128)]
		[InlineData(Byte.MaxValue)]
		public void SetBit_InvalidIndex(Byte index)
		{
			Byte @byte = 0b0000_0000;
			Assert.Throws<IndexOutOfRangeException>(() => @byte.SetBit(index));
		}

		[Theory]
		[InlineData(new Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0000_0000 }, 0)]
		[InlineData(new Byte[] { 0b0000_0000, 0b0000_0010, 0b0000_0000, 0b0000_0000 }, 9)]
		public void SetBitOnSpan_ValidIndex(Byte[] expected, Int32 index)
		{
			var bytes = new Byte[4];
			bytes.AsSpan().SetBit(index);
			Assert.Equal(expected, bytes);
		}

		[Theory]
		[InlineData(32)]
		[InlineData(Int16.MaxValue)]
		public void SetBitOnSpan_InvalidIndex(Int32 index)
		{
			var bytes = new Byte[4];
			Assert.Throws<IndexOutOfRangeException>(() => bytes.AsSpan().SetBit(index));
		}
	}
}
