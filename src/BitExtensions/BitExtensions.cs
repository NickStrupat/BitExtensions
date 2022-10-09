using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NickStrupat
{
	// ref struct BitRef
	// {
	// 	private readonly ref BitSpanScalar bitSpanScalar;
	// 	private readonly Byte index;
	//
	// 	public BitRef(BitSpanScalar bitSpanScalar, Byte index)
	// 	{
	// 		bitSpanScalar = bitSpanScalar;
	// 		this.index = index;
	// 	}
	//
	// 	public Boolean Value
	// 	{
	// 		get => bitSpanScalar[index];
	// 		set => bitSpanScalar[index] = value;
	// 	}
	// }
	
	public static class BitExtensions
	{
		public static String ToBitString<T>(ref this T value) where T : unmanaged
		{
			return MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)).ToBitString();
		}
		
		public static String ToBitString(this ReadOnlySpan<Byte> bytes)
		{
			Span<Char> span = stackalloc Char[bytes.Length * 8];
			for (var i = 0; i < bytes.Length; i++)
			for (var j = 0; j < 8; j++)
				span[^(i * 8 + j + 1)] = (bytes[i] & (1 << j)) != 0 ? '1' : '0';
			return new(span);
		}
		public static String ToBitStringWithDigitGroupSeparator<T>(this T value, Int32 groupSize = 8, Char separator = '_') where T : unmanaged
		{
			return MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)).ToBitStringWithDigitGroupSeparator(groupSize, separator);
		}
		
		public static String ToBitStringWithDigitGroupSeparator(this ReadOnlySpan<Byte> bytes, Int32 groupSize = 8, Char separator = '_')
		{
			var bitCount = bytes.Length * 8;
			var separatorCount = bitCount / groupSize - 1;
			Span<Char> span = stackalloc Char[bitCount + separatorCount];
			var si = 0;
			for (var i = 0; i < bytes.Length; i++)
			for (var j = 0; j < 8; j++)
			{
				var bi = i * 8 + j;
				if (bi != 0 && bi % groupSize == 0)
					span[^++si] = separator;
				span[^++si] = (bytes[i] & (1 << j)) != 0 ? '1' : '0';
			}

			return new(span);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static Byte GetBit(this ReadOnlySpan<Byte> bytes, Int32 index) =>
			(Byte) (bytes[index / 8] >>> (index & 0b111) & 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte GetBit(this Span<Byte> bytes, Int32 index) =>
			GetBit((ReadOnlySpan<Byte>) bytes, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBit(this Span<Byte> bytes, Int32 index) => bytes[index >>> 3].SetBit((Byte) (index & 0b111));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClearBit(this Span<Byte> bytes, Int32 index) =>
			bytes[index >>> 3].ClearBit((Byte) (index & 0b111));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBit(this Span<Byte> bytes, Int32 index, Byte state) =>
			bytes[index >>> 3].WriteBit((Byte) (index & 0b111), state);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte GetBit(this Byte @byte, Byte index) =>
			(Byte) ((@byte >>> CheckIndex(index)) & 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBit(this ref Byte @byte, Byte index) =>
			@byte |= (Byte) (1 << CheckIndex(index));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClearBit(this ref Byte @byte, Byte index) =>
			@byte &= (Byte) ~(1 << CheckIndex(index));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBit(this ref Byte @byte, Byte index, Byte bitState)
		{
			CheckIndex(index);
			Int32 shift = index & 0b111;
			UInt32 mask = 1U << shift;
			bitState &= 1;
			bitState <<= shift;
			@byte = (Byte) ((@byte & ~mask) | bitState);
		}
		
		private static Byte CheckIndex(Byte index) => index <= 0b111 ? index : throw new IndexOutOfRangeException(nameof(index));

		public static void WriteBits(this Span<Byte> bytes, Int32 offset, Byte bits, Byte bitCount)
		{
			var byteOffset = offset >>> 3;
			var bitOffset = offset & 0b111;
			var mask = (Byte) (bits << bitOffset);
			var clearMask = (Byte) ~(((1 << bitCount) - 1) << bitOffset);
			bytes[byteOffset] = (Byte) (bytes[byteOffset] & clearMask | mask);
		}

		internal static Boolean ReadBit(ref this UInt64 bits, Byte index) => (bits & (1UL << index)) != 0;
		internal static void WriteBit(ref this UInt64 bits, Byte index, Boolean bitState) => bits = bitState ? bits | (1UL << index) : bits & ~(1UL << index);

		internal static void SetBit(ref this UInt64 bits, Byte index) => bits |= (1UL << index);
		internal static void ClearBit(ref this UInt64 bits, Byte index) => bits &= ~(1UL << index);

		internal static void SetBits(ref this UInt64 bits) => bits = UInt64.MaxValue;
		internal static void ClearBits(ref this UInt64 bits) => bits = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static (Int32 spanIndex, Byte bitIndex) GetIndexes(this Int32 index) => ((index & ~0b111) >> 3, (Byte) (index & 0b111));

		// internal static Boolean ReadBit(this ReadOnlySpan<BitSpanScalar> bits, Int32 index)
		// {
		// 	var (spanIndex, bitIndex) = GetIndexes(index);
		// 	return bits[spanIndex][bitIndex];
		// }
		//
		// internal static void WriteBit(this Span<BitSpanScalar> bits, Int32 index, Boolean bitState)
		// {
		// 	var (spanIndex, bitIndex) = GetIndexes(index);
		// 	bits[spanIndex][bitIndex] = bitState;
		// }
		//
		// internal static void SetBit(this Span<BitSpanScalar> bits, Int32 index)
		// {
		// 	var (spanIndex, bitIndex) = GetIndexes(index);
		// 	bits[spanIndex][bitIndex] = true;
		// }
		//
		// internal static void ClearBit(this Span<BitSpanScalar> bits, Int32 index)
		// {
		// 	var (spanIndex, bitIndex) = GetIndexes(index);
		// 	bits[spanIndex][bitIndex] = false;
		// }
	}
}
