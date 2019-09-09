using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace NickStrupat.BitSpan_Tests
{
	public class BitSpan_Tests
	{
		[Theory]
		[InlineData(8, 0, 1)]
		[InlineData(7, 1, 1)]
		[InlineData(4, 4, 1)]
		[InlineData(1, 7, 1)]
		[InlineData(8, 8, 2)]
		[InlineData(7, 9, 2)]
		[InlineData(13, 3, 2)]
		[InlineData(16, 0, 2)]
		[InlineData(30, 2, 4)]
		public void Length(Int32 expectedBitLength, Byte bitOffset, Int32 bytesCount)
		{
			Span<Byte> bytes = stackalloc Byte[bytesCount];
			var bitSpan = new BitSpan(bytes, bitOffset);
			Assert.Equal(expectedBitLength, bitSpan.Length);
		}

		[Theory]
		[InlineData(8, 1)]
		[InlineData(9, 1)]
		[InlineData(16, 1)]
		[InlineData(16, 2)]
		public void LengthThrows(Byte bitOffset, Int32 bytesCount)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => {
				Span<Byte> bytes = stackalloc Byte[bytesCount];
				var bitSpan = new BitSpan(bytes, bitOffset);
				var length = bitSpan.Length;
			});
		}

		[Fact]
		public void IsEmpty()
		{
			var bitSpan = BitSpan.Empty;
			Assert.True(bitSpan.IsEmpty);

			bitSpan = new BitSpan();
			Assert.True(bitSpan.IsEmpty);

			bitSpan = default;
			Assert.True(bitSpan.IsEmpty);

			Span<Byte> bytes = stackalloc Byte[1];
			var bitSpan2 = new BitSpan(bytes, 0);
			Assert.True(bitSpan2.IsEmpty);
		}

		[Fact]
		public void Empty()
		{
			var bitSpan = BitSpan.Empty;
			Assert.Equal(0, bitSpan.Length);
			Assert.True(bitSpan.IsEmpty);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(12)]
		[InlineData(31)]
		[InlineData(32)]
		public void ToString_(Byte bitOffset)
		{
			Span<Byte> bytes = stackalloc Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0100_0000 };
			var integer = MemoryMarshal.Read<UInt32>(bytes);
			var expected = Convert.ToString(integer, 2).PadRight(32, '0').Substring(bitOffset);
			var bitSpan = new BitSpan(bytes, bitOffset);
			var actual = bitSpan.ToString();
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EqualsThrows() => Assert.Throws<NotSupportedException>(() => BitSpan.Empty.Equals(null));

		[Fact]
		public void GetHashCodeThrows() => Assert.Throws<NotSupportedException>(() => BitSpan.Empty.GetHashCode());
	}
}
