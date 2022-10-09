using System;
using Xunit;

namespace NickStrupat.BitExtensions_Tests
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
		public void ClearBit_ValidIndex(Byte expected, Byte index)
		{
			Byte actual = 0b1111_1111;
			actual.ClearBit(index);
			Assert.Equal(expected, actual);
		}
		
		[Theory]
		[InlineData(8)]
		[InlineData(9)]
		[InlineData(64)]
		[InlineData(127)]
		[InlineData(128)]
		[InlineData(Byte.MaxValue)]
		public void ClearBit_InvalidIndex(Byte index)
		{
			Byte @byte = 0b1111_1111;
			Assert.Throws<IndexOutOfRangeException>(() => @byte.ClearBit(index));
		}

		[Theory]
		[InlineData(new Byte[] { 0b1111_1110, 0b1111_1111, 0b1111_1111, 0b1111_1111 }, 0)]
		[InlineData(new Byte[] { 0b1111_1111, 0b1111_1101, 0b1111_1111, 0b1111_1111 }, 9)]
		[InlineData(new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b0111_1111 }, 31)]
		public void ClearBitOnSpan_ValidIndex(Byte[] expected, Int32 index)
		{
			var bytes = new Byte[4];
			bytes.AsSpan().Fill(0b1111_1111);
			bytes.AsSpan().ClearBit(index);
			Assert.Equal(expected, bytes);
		}

		[Theory]
		[InlineData(32)]
		[InlineData(33)]
		[InlineData(Byte.MaxValue)]
		[InlineData(Int16.MaxValue)]
		[InlineData(Int32.MaxValue)]
		public void ClearBitOnSpan_InvalidIndex(Int32 index)
		{
			var bytes = new Byte[4];
			bytes.AsSpan().Fill(0b1111_1111);
			Assert.Throws<IndexOutOfRangeException>(() => bytes.AsSpan().ClearBit(index));
		}
	}
}
