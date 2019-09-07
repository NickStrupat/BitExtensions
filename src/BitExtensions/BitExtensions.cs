using System;
using System.Runtime.CompilerServices;

namespace NickStrupat
{
	// TODO: figure out branchless masking of indexes to 3 bits so any index over 7 returns 0
	public static class BitExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte GetBit(this ReadOnlySpan<Byte> bytes, Int32 index) =>
			bytes[(index & ~0b111) >> 3].GetBit((Byte) (index & 0b111));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte GetBit(this Span<Byte> bytes, Int32 index) =>
			GetBit((ReadOnlySpan<Byte>)bytes, index);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBit(this Span<Byte> bytes, Int32 index) =>
			bytes[(index & ~0b111) >> 3].SetBit((Byte)(index & 0b111));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClearBit(this Span<Byte> bytes, Int32 index) =>
			bytes[(index & ~0b111) >> 3].ClearBit((Byte)(index & 0b111));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBit(this Span<Byte> bytes, Int32 index, Byte state) =>
			bytes[(index & ~0b111) >> 3].WriteBit((Byte)(index & 0b111), state);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte GetBit(this Byte @byte, Byte index) =>
			(Byte)((@byte >> (index)) & 1);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBit(this ref Byte @byte, Byte index) =>
			@byte |= (Byte)(1 << (index));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClearBit(this ref Byte @byte, Byte index) =>
			@byte &= (Byte)~(1 << (index));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBit(this ref Byte @byte, Byte index, Byte bitState)
		{
			Int32 shift = index & 0b111;
			UInt32 mask = 1U << shift;
			bitState &= 1;
			bitState <<= shift;
			@byte = (Byte)((@byte & ~mask) | bitState);
		}
	}
}
