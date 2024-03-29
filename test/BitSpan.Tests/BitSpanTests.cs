using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace NickStrupat.BitSpan_Tests
{
	public class BitSpanTests
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
		[InlineData(0, 8, 1)]
		[InlineData(0, 16, 2)]
		public void Length(Int32 expectedBitLength, Byte bitOffset, Int32 bytesCount)
		{
			Span<Byte> bytes = stackalloc Byte[bytesCount];
			var bitSpan = new BitSpan(bytes, bitOffset);
			Assert.Equal(expectedBitLength, bitSpan.Length);
		}

		[Theory]
		[InlineData(9, 1)]
		[InlineData(16, 1)]
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
			Assert.True(BitSpan.Empty.IsEmpty);

			Assert.True(new BitSpan().IsEmpty);

			Assert.True(default(BitSpan).IsEmpty);

			Assert.True(new BitSpan(stackalloc Byte[0], 0).IsEmpty);
			
			Assert.False(new BitSpan(stackalloc Byte[1], 0).IsEmpty);
			
			Assert.True(new BitSpan(stackalloc Byte[1], 8).IsEmpty);
			
			Assert.False(new BitSpan(stackalloc Byte[1], 7).IsEmpty);
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
		public void ToString_(Byte bitOffset)
		{
			Span<Byte> bytes = stackalloc Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0100_0000 };
			var integer = MemoryMarshal.Read<UInt32>(bytes);
			var expected = Convert.ToString(integer, 2).PadRight(32, '0').Substring(bitOffset);
			var bitSpan = new BitSpan(bytes, bitOffset);
			var actual = bitSpan.ToString();
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(32)]
		[InlineData(33)]
		[InlineData(Byte.MaxValue)]
		public void ToStringThrows(Byte bitOffset)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => {
				Span<Byte> bytes = stackalloc Byte[] { 0b0000_0001, 0b0000_0000, 0b0000_0000, 0b0100_0000 };
				var integer = MemoryMarshal.Read<UInt32>(bytes);
				var expected = Convert.ToString(integer, 2).PadRight(32, '0').Substring(bitOffset);
				var bitSpan = new BitSpan(bytes, bitOffset);
				var actual = bitSpan.ToString();
				Assert.Equal(expected, actual);
			});
		}

		[Fact]
		public void EqualsThrows() => Assert.Throws<NotSupportedException>(() => BitSpan.Empty.Equals(null));

		[Fact]
		public void GetHashCodeThrows() => Assert.Throws<NotSupportedException>(() => BitSpan.Empty.GetHashCode());

		[Theory]
		[InlineData(null, null, new Byte[] { 0b1111_1111 }, new Byte[] { 0 })]
		[InlineData(null, null, new Byte[] { 0b1111_1111, 0b1111_1111 }, new Byte[] { 0, 0 })]
		[InlineData(null, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0, 0, 0 })]
		[InlineData(0, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0, 0, 0 })]
		[InlineData(1, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_0001, 0, 0 })]
		[InlineData(2, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_0011, 0, 0 })]
		[InlineData(3, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_0111, 0, 0 })]
		[InlineData(4, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_1111, 0, 0 })]
		[InlineData(5, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0001_1111, 0, 0 })]
		[InlineData(6, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0011_1111, 0, 0 })]
		[InlineData(7, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0111_1111, 0, 0 })]
		[InlineData(8, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b1111_1111, 0, 0 })]
		[InlineData(9, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b1111_1111, 0b0000_00001, 0 })]
		public void Clear(Int32? start, Int32? length, Byte[] bytes, Byte[] expected)
		{
			var bitSpan = new BitSpan(bytes, start.GetValueOrDefault(), length.GetValueOrDefault(bytes.Length * 8));
			bitSpan.Clear();
			Assert.Equal(expected, bytes);
		}

		[Theory]
		[InlineData(null, null, new Byte[] { 0 }      , new Byte[] { 0b1111_1111 }                          )]
		[InlineData(null, null, new Byte[] { 0, 0 }   , new Byte[] { 0b1111_1111, 0b1111_1111 }             )]
		[InlineData(null, null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 })]
		[InlineData(0   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 })]
		[InlineData(1   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1110, 0b1111_1111, 0b1111_1111 })]
		[InlineData(2   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1100, 0b1111_1111, 0b1111_1111 })]
		[InlineData(3   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(4   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(5   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1110_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(6   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1100_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(7   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1000_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(8   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b0000_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(9   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b0000_0000, 0b1111_1110, 0b1111_1111 })]
		public void Set(Int32? start, Int32? length, Byte[] bytes, Byte[] expected)
		{
			var bitSpan = new BitSpan(bytes, start.GetValueOrDefault(), length.GetValueOrDefault(bytes.Length * 8));
			bitSpan.Set();
			Assert.Equal(expected, bytes);
		}

		[Theory]
		[InlineData(false, null, null, new Byte[] { 0b1111_1111 }                          , new Byte[] { 0 }                           )]
		[InlineData(false, null, null, new Byte[] { 0b1111_1111, 0b1111_1111 }             , new Byte[] { 0, 0 }                        )]
		[InlineData(false, null, null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0, 0, 0 }                     )]
		[InlineData(false, 0   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0, 0, 0 }                     )]
		[InlineData(false, 1   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_0001, 0, 0 }           )]
		[InlineData(false, 2   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_0011, 0, 0 }           )]
		[InlineData(false, 3   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_0111, 0, 0 }           )]
		[InlineData(false, 4   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0000_1111, 0, 0 }           )]
		[InlineData(false, 5   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0001_1111, 0, 0 }           )]
		[InlineData(false, 6   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0011_1111, 0, 0 }           )]
		[InlineData(false, 7   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b0111_1111, 0, 0 }           )]
		[InlineData(false, 8   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b1111_1111, 0, 0 }           )]
		[InlineData(false, 9   , null, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 }, new Byte[] { 0b1111_1111, 0b0000_0001, 0 })]
		[InlineData(true , null, null, new Byte[] { 0 }      , new Byte[] { 0b1111_1111 }                          )]
		[InlineData(true , null, null, new Byte[] { 0, 0 }   , new Byte[] { 0b1111_1111, 0b1111_1111 }             )]
		[InlineData(true , null, null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 0   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 1   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1110, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 2   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1100, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 3   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_1000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 4   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1111_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 5   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1110_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 6   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1100_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 7   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b1000_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 8   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b0000_0000, 0b1111_1111, 0b1111_1111 })]
		[InlineData(true , 9   , null, new Byte[] { 0, 0, 0 }, new Byte[] { 0b0000_0000, 0b1111_1110, 0b1111_1111 })]
		public void Fill(Boolean value, Int32? start, Int32? length, Byte[] bytes, Byte[] expected)
		{
			var bitSpan = new BitSpan(bytes, start.GetValueOrDefault(), length.GetValueOrDefault(bytes.Length * 8));
			bitSpan.Fill(value);
			Assert.Equal(expected, bytes);
		}
	}
}
