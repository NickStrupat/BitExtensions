using System;
using Xunit;

namespace NickStrupat.BitExtensions.Tests
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
		[InlineData(0b0, 8)]
		[InlineData(0b0, 9)]
		[InlineData(0b0, 10)]
		[InlineData(0b0, 11)]
		[InlineData(0b0, 12)]
		[InlineData(0b0, 13)]
		[InlineData(0b0, 14)]
		[InlineData(0b0, 64)]
		[InlineData(0b0, 127)]
		[InlineData(0b0, 128)]
		[InlineData(0b0, Byte.MaxValue)]
		public void GetBit(Byte expected, Byte index)
		{
			Byte @byte = 0b1011_0100;
			var bitState = @byte.GetBit(index);
			Assert.Equal(expected, bitState);
		}

		[Theory]
		[InlineData(1, 0)]
		[InlineData(0, 1)]
		[InlineData(0, 2)]
		[InlineData(0, 3)]
		[InlineData(0, 4)]
		[InlineData(1, 30)]
		[InlineData(0, 31)]
		[InlineData(0, 32)]
		[InlineData(0, 123)]
		[InlineData(0, Int32.MaxValue)]
		[InlineData(0, Int32.MinValue)]
		public void GetBitOnSpan(Byte expectedBitState, Int32 index)
		{
			var bytes = new Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0100_0000 };
			Byte getBitState() => bytes.AsSpan().GetBit(index);
			if (index < 0 | index >=  bytes.Length * 8)
				Assert.Throws<IndexOutOfRangeException>(() => getBitState());
			else
				Assert.Equal(expectedBitState, getBitState());
		}
	}
}
