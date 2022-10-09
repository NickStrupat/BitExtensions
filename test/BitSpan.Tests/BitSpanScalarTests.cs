using System;
using System.Linq;
using Xunit;

namespace NickStrupat.BitSpan_Tests;

public class BitSpanScalarTests
{
	[Theory]
	[InlineData((UInt64)1 << 63, 0, 0, 0)]
	public void BitSpanScalar_ToString_Test(UInt64 _0, UInt64 _1, UInt64 _2, UInt64 _3)
	{
		var bitSpanScalar = new BitSpanScalar(_0, _1, _2, _3);
		var expected = String.Join(String.Empty, new []{_0, _1, _2, _3}.Select(x => x.ToBitString()));
		Assert.Equal(expected, bitSpanScalar.ToString());
	}
	
	[Theory]
	[InlineData(0b_10000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000ul, 0, 0, 0)]
	public void BitArray_ToBitString(UInt64 _0, UInt64 _1, UInt64 _2, UInt64 _3)
	{
		var bitSpanScalar = new BitSpanScalar(_0, _1, _2, _3);
		var expected = String.Join(String.Empty, new []{_0, _1, _2, _3}.Select(x => x.ToBitString()));
		Assert.Equal(expected, bitSpanScalar.ToString());
	}
}