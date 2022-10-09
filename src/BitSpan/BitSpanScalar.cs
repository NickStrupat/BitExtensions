using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NickStrupat;

[StructLayout(LayoutKind.Sequential, Size = Size, Pack = 1)] // use 256-bit size so we can index this scalar with System.Byte without having to do bounds checks
public unsafe struct BitSpanScalar
{
	public const Int32 Size = 256 / 8; // 256 bits; divide by 8 to get size, in bytes
	
	public BitSpanScalar(UInt64 _0, UInt64 _1, UInt64 _2, UInt64 _3)
	{
		data[0] = _0;
		data[1] = _1;
		data[2] = _2;
		data[3] = _3;
	}
	
	public Span<T> Span<T>() where T : unmanaged => new(Unsafe.AsPointer(ref this), Size / Unsafe.SizeOf<T>());
	private Span<UInt64> data => Span<UInt64>();

	public Boolean this[Byte index]
	{
		get => data[3 - index / 64] << (index % 64) != 0;
		set
		{
			ref var element = ref data[3 - index / 64];
			var safeValue = Unsafe.As<Boolean, Byte>(ref value) != 0; // ensure the boolean value wasn't unsafely cast from some value other than 0 or 1
			var bitValue = Unsafe.As<Boolean, Byte>(ref safeValue);
			
			var mask = (UInt64)1 << (index % 64);
			element = (element & ~mask) | (bitValue);
		}
	}

	public void Clear() => data.Clear();
	public void Set() => data.Fill(UInt64.MaxValue);

	public override String ToString()
	{
		return String.Create(256, this, static (span, @this) =>
		{
			for (var i = 0; i != 4; i++)
				UInt64Extensions.ToBitString(span.Slice(i * 64, 64), @this.data[i]);
		});
	}
}

public static class UInt64Extensions
{
	public static String ToBitString(this UInt64 value) => String.Create(64, value, ToBitString);

	internal static void ToBitString(Span<Char> destination, UInt64 value)
	{
		for (var i = 0; i != 64; i++)
		{
			destination[64 - 1 - i] = (value & 1) == 1 ? '1' : '0';
			value >>= 1;
		}
	}
	
	public static String ToBitString(this BitArray bitArray)
	{
		return String.Create(bitArray.Count, bitArray, static (span, bitArray) =>
		{
			for (var i = 0; i < bitArray.Count; i++)
				span[i] = bitArray[i] ? '1' : '0';
		});
	}
}