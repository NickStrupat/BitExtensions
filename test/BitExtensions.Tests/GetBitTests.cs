using System;
using Xunit;

namespace NickStrupat.BitExtensions_Tests
{
	public class GetBitTests
	{
		[Theory]
		[InlineData(0b0, 0)]
		[InlineData(0b0, 1)]
		[InlineData(0b1, 2)]
		[InlineData(0b0, 3)]
		[InlineData(0b1, 4)]
		[InlineData(0b1, 5)]
		[InlineData(0b0, 6)]
		[InlineData(0b1, 7)]
		public void GetBit_ValidIndex(Byte expected, Byte index)
		{
			const Byte @byte = 0b1011_0100;
			var bitState = @byte.GetBit(index);
			Assert.Equal(expected, bitState);
		}
		
		[Theory]

		[InlineData(8)]
		[InlineData(9)]
		[InlineData(10)]
		[InlineData(11)]
		[InlineData(12)]
		[InlineData(13)]
		[InlineData(14)]
		[InlineData(64)]
		[InlineData(127)]
		[InlineData(128)]
		[InlineData(Byte.MaxValue)]
		public void GetBit_InvalidIndex(Byte index)
		{
			const Byte @byte = 0b1011_0100;
			Assert.Throws<IndexOutOfRangeException>(() => @byte.GetBit(index));
		}

		[Theory]
		[InlineData(1, 0)]
		[InlineData(0, 1)]
		[InlineData(0, 2)]
		[InlineData(0, 3)]
		[InlineData(0, 4)]
		[InlineData(1, 30)]
		[InlineData(0, 31)]
		public void GetBitOnSpan_ValidIndex(Byte expectedBitState, Int32 index)
		{
			var bytes = new Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0100_0000 };
			var bitState = bytes.AsSpan().GetBit(index);
			Assert.Equal(expectedBitState, bitState);
		}

		[Theory]
		[InlineData(32)]
		[InlineData(123)]
		[InlineData(Int32.MaxValue)]
		[InlineData(Int32.MinValue)]
		public void GetBitOnSpan_InvalidIndex(Int32 index)
		{
			var bytes = new Byte[4];
			Assert.Throws<IndexOutOfRangeException>(() => bytes.AsSpan().GetBit(index));
		}
	}
}
