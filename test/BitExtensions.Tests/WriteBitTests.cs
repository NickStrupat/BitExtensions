using System;
using System.Runtime.InteropServices;
using Xunit;

namespace NickStrupat.BitExtensions_Tests
{
	public class WriteBitTests
	{
		[Theory]
		[InlineData(0, 0b0000_0001, 0b1, 0)]
		[InlineData(0, 0b0000_0010, 0b1, 1)]
		[InlineData(0, 0b0000_0100, 0b1, 2)]
		[InlineData(0, 0b0000_1000, 0b1, 3)]
		[InlineData(0, 0b0001_0000, 0b1, 4)]
		[InlineData(0, 0b0010_0000, 0b1, 5)]
		[InlineData(0, 0b0100_0000, 0b1, 6)]
		[InlineData(0, 0b1000_0000, 0b1, 7)]

		[InlineData(0, 0, 0b1, 8)]
		[InlineData(0, 0, 0b1, 9)]
		[InlineData(0, 0, 0b1, 10)]
		[InlineData(0, 0, 0b1, 11)]
		[InlineData(0, 0, 0b1, 12)]
		[InlineData(0, 0, 0b1, 13)]
		[InlineData(0, 0, 0b1, 14)]
		[InlineData(0, 0, 0b1, 15)]

		[InlineData(0b1111_1111, 0b1111_1110, 0b0, 0)]
		[InlineData(0b1111_1111, 0b1111_1101, 0b0, 1)]
		[InlineData(0b1111_1111, 0b1111_1011, 0b0, 2)]
		[InlineData(0b1111_1111, 0b1111_0111, 0b0, 3)]
		[InlineData(0b1111_1111, 0b1110_1111, 0b0, 4)]
		[InlineData(0b1111_1111, 0b1101_1111, 0b0, 5)]
		[InlineData(0b1111_1111, 0b1011_1111, 0b0, 6)]
		[InlineData(0b1111_1111, 0b0111_1111, 0b0, 7)]
		public void WriteBit(Byte initial, Byte expected, Byte bitState, Byte index)
		{
			initial.WriteBit(index, bitState);
			Assert.Equal(expected, initial);
		}

		[Theory]
		[InlineData(1, 0b1, 0)]
		[InlineData(1, 0b1, 1)]
		[InlineData(1, 0b1, 2)]
		[InlineData(1, 0b1, 3)]
		[InlineData(1, 0b1, 4)]
		[InlineData(1, 0b1, 30)]
		[InlineData(1, 0b1, 31)]
		[InlineData(1, 0b1, 32)]
		[InlineData(1, 0b1, 23)]
		[InlineData(1, 0b1, Int32.MaxValue)]
		[InlineData(1, 0b1, Int32.MinValue)]
		public void WriteBitOnSpan(Byte expectedBitState, Byte bitState, Int32 index)
		{
			var bytes = new Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0100_0000 };
			void writeBitState() => bytes.AsSpan().WriteBit(index, bitState);
			if (index < 0 | index >= bytes.Length * 8)
				Assert.Throws<IndexOutOfRangeException>(() => writeBitState());
			else
			{
				writeBitState();
				Assert.Equal(expectedBitState, bytes.AsSpan().GetBit(index));
			}
		}
		[Theory]
		[InlineData(0b0001_1100, 0b0000_0000, 0b111, 3, 2)]
		[InlineData(0b1111_1111, 0b1111_1111, 0b000, 3, 2)]
		public static void WriteBits(Byte expected, Byte initial, Byte bitsToWrite, Byte bitsToWriteCount, Byte offset)
		{
			Span<Byte> bytes = stackalloc Byte[1];
			bytes[0] = initial;
			bytes.WriteBits(bitsToWrite, bitsToWriteCount, 2);
			Assert.Equal(expected, bytes[0]);
		}
	}
}