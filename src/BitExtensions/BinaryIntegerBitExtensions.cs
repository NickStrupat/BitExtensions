using System;
using System.Numerics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BitExtensions.Tests")]

namespace NickStrupat;

public static class BinaryIntegerBitExtensions
{
	internal static Boolean ReadBitUnchecked<T>(this T value, Int32 indexUnchecked) where T : unmanaged, IBinaryInteger<T>
	{
		return (value & (T.One << indexUnchecked)) != T.Zero;
	}
	
	internal static void WriteBitUnchecked<T>(ref this T value, Int32 indexUnchecked, Boolean bitState) where T : unmanaged, IBinaryInteger<T>
	{
		var safeBoolean = Unsafe.As<Boolean, Byte>(ref bitState) != 0; // ensure the boolean value wasn't unsafely cast from some value other than 0 or 1
		var setMask = Unsafe.As<Boolean, T>(ref safeBoolean) << indexUnchecked;
		var clearMask = ~(T.One << indexUnchecked);
		value = value & clearMask | setMask;
	}
	
	internal static void SetBitUnchecked<T>(ref this T value, Int32 indexUnchecked) where T : unmanaged, IBinaryInteger<T>
	{
		value |= T.One << indexUnchecked;
	}
	
	public static void ClearBitUnchecked<T>(ref this T value, Int32 indexUnchecked) where T : unmanaged, IBinaryInteger<T>
	{
		value &= ~(T.One << indexUnchecked);
	}
	
	internal static void ToggleBitUnchecked<T>(ref this T value, Int32 indexUnchecked) where T : unmanaged, IBinaryInteger<T>
	{
		value ^= T.One << indexUnchecked;
	}
	
	internal static T ReadBitsUnchecked<T>(this T value, Int32 indexUnchecked, Int32 countUnchecked) where T : unmanaged, IBinaryInteger<T>
	{
		var mask = (T.One << countUnchecked) - T.One;
		return value >> indexUnchecked & mask;
	}
	
	internal static void WriteBitsUnchecked<T>(ref this T value, Int32 indexUnchecked, Int32 countUnchecked, T bits) where T : unmanaged, IBinaryInteger<T>
	{
		var clearMask = ~(((T.One << countUnchecked) - T.One) << indexUnchecked);
		value = value & clearMask | (bits << indexUnchecked);
	}

	
	public static Boolean ReadBit<T>(this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		return value.ReadBitUnchecked(CheckIndex<T>(index));
	}
	
	public static void WriteBit<T>(ref this T value, Int32 index, Boolean bitState) where T : unmanaged, IBinaryInteger<T>
	{
		value.WriteBitUnchecked(CheckIndex<T>(index), bitState);
	}
	
	public static void SetBit<T>(ref this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		value.SetBitUnchecked(CheckIndex<T>(index));
	}
	
	public static void ClearBit<T>(ref this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		value.ClearBitUnchecked(CheckIndex<T>(index));
	}
	
	public static void ToggleBit<T>(ref this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		value.ToggleBitUnchecked(CheckIndex<T>(index));
	}
	
	public static T ReadBits<T>(this T value, Int32 index, Int32 count) where T : unmanaged, IBinaryInteger<T>
	{
		return value.ReadBitsUnchecked(CheckIndex<T>(index), CheckCount<T>(index, count));
	}
	
	public static void WriteBits<T>(ref this T value, Int32 index, Int32 count, T bits) where T : unmanaged, IBinaryInteger<T>
	{
		value.WriteBitsUnchecked(CheckIndex<T>(index), CheckCount<T>(index, count), bits);
	}

	
	public static Boolean ReadBitFast<T>(this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		return value.ReadBitUnchecked(MaskIndex<T>(index));
	}
	
	public static void WriteBitFast<T>(ref this T value, Int32 index, Boolean bitState) where T : unmanaged, IBinaryInteger<T>
	{
		value.WriteBitUnchecked(MaskIndex<T>(index), bitState);
	}
	
	public static void SetBitFast<T>(ref this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		value.SetBitUnchecked(MaskIndex<T>(index));
	}
	
	public static void ClearBitFast<T>(ref this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		value.ClearBitUnchecked(MaskIndex<T>(index));
	}
	
	public static void ToggleBitFast<T>(ref this T value, Int32 index) where T : unmanaged, IBinaryInteger<T>
	{
		value.ToggleBitUnchecked(MaskIndex<T>(index));
	}
	
	public static T ReadBitsFast<T>(this T value, Int32 index, Int32 count) where T : unmanaged, IBinaryInteger<T>
	{
		return value.ReadBitsUnchecked(MaskIndex<T>(index), count);
	}
	
	public static void WriteBitsFast<T>(ref this T value, Int32 index, Int32 count, T bits) where T : unmanaged, IBinaryInteger<T>
	{
		value.WriteBitsUnchecked(MaskIndex<T>(index), count, bits);
	}
	
	private static class BitSizeOf<T> where T : unmanaged
	{
		public static readonly Int32 Value = Unsafe.SizeOf<T>() * 8;
	}
	
	private static Int32 MaskIndex<T>(Int32 index) where T : unmanaged
	{
		return index & (BitSizeOf<T>.Value - 1);
	}

	private static Int32 CheckIndex<T>(Int32 index) where T : unmanaged
	{
		if (index < 0 | index >= BitSizeOf<T>.Value)
			throw new ArgumentOutOfRangeException(nameof(index));
		return index;
	}
	
	private static Int32 CheckCount<T>(Int32 index, Int32 count) where T : unmanaged
	{
		if (count < 0 | count > BitSizeOf<T>.Value - index)
			throw new ArgumentOutOfRangeException(nameof(count));
		return count;
	}
}