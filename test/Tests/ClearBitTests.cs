using System;
using Xunit;

namespace NickStrupat.BitExtensions.Tests
{
	public class ClearBitTests
	{
		[Theory]
		[InlineData(0b1111_1110, 0)]
		[InlineData(0b1111_1101, 1)]
		[InlineData(0b1111_1011, 2)]
		[InlineData(0b1111_0111, 3)]
		[InlineData(0b1110_1111, 4)]
		[InlineData(0b1101_1111, 5)]
		[InlineData(0b1011_1111, 6)]
		[InlineData(0b0111_1111, 7)]
		[InlineData(0b1111_1111, 8)]
		[InlineData(0b1111_1111, 9)]
		[InlineData(0b1111_1111, 64)]
		[InlineData(0b1111_1111, 127)]
		[InlineData(0b1111_1111, 128)]
		[InlineData(0b1111_1111, Byte.MaxValue)]
		public void ClearBit(Byte expected, Byte index)
		{
			Byte @byte = 0b1111_1111;
			@byte.ClearBit(index);
			Assert.Equal(expected, @byte);
		}

		[Theory]
		[InlineData(new Byte[] { 0b1111_1110, 0b1111_1111, 0b1111_1111, 0b1111_1111 }, 0)]
		[InlineData(new Byte[] { 0b1111_1111, 0b1111_1101, 0b1111_1111, 0b1111_1111 }, 9)]
		public void ClearBitOnSpan(Byte[] expected, Int32 index)
		{
			var bytes = new Byte[4];
			bytes.AsSpan().Fill(0b1111_1111);
			bytes.AsSpan().ClearBit(index);
			Assert.Equal(expected, bytes);
		}
	}
}
