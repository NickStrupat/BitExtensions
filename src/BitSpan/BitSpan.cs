using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NickStrupat
{
	public readonly ref struct BitSpan
	{
		readonly Span<Byte> bytes;
		readonly Int32 length;
		readonly Byte bitOffset;

		public BitSpan(Span<Byte> bytes)
		{
			this.bytes = bytes;
			this.length = this.bytes.Length << 3;
			this.bitOffset = 0;
		}

		public BitSpan(Span<Byte> bytes, Byte bitOffset)
		{
			this.bytes = bytes;
			this.length = this.bytes.Length << 3;
			this.bitOffset = CheckBitOffset(this.length, bitOffset);
		}

		public BitSpan(Span<Byte> bytes, Int32 length)
		{
			this.bytes = bytes;
			this.length = CheckLength(bytes, length);
			this.bitOffset = 0;
		}

		public BitSpan(Span<Byte> bytes, Int32 length, Byte bitOffset)
		{
			this.bytes = bytes;
			this.length = CheckLength(bytes, length);
			this.bitOffset = CheckBitOffset(this.length, bitOffset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Int32 CheckLength(Span<Byte> bytes, Int32 length) =>
			length > 0 & length <= bytes.Length << 3 ? length : throw new ArgumentException();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Byte CheckBitOffset(Int32 length, Byte bitOffset) =>
			bitOffset < length ? bitOffset : throw new ArgumentOutOfRangeException(nameof(bitOffset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 CheckIndex(Int32 index) =>
			bitOffset + index < length ? bitOffset + index : throw new ArgumentOutOfRangeException(nameof(index));

		public Byte this[Int32 index]
		{
			get => bytes.GetBit(CheckIndex(index));
			set => bytes.WriteBit(CheckIndex(index), value);
		}

		public static Boolean operator ==(BitSpan left, BitSpan right) =>
			left.length == right.length &
			left.bitOffset == right.bitOffset &&
			left.bytes == right.bytes;
		public static Boolean operator !=(BitSpan left, BitSpan right) => !(left == right);

		public static BitSpan Empty => default;
		public Boolean IsEmpty => length - bitOffset == 0 | bytes.IsEmpty;

		public Int32 Length => (bytes.Length << 3) - bitOffset;

		public override String ToString()
		{
			var sb = new StringBuilder(Length);
			foreach (var bit in this)
				sb.Append(bit == 0 ? '0' : '1');
			return sb.ToString();
		}

		public void Clear()
		{
			if (bitOffset > 0b111)
				bytes.Slice(bitOffset >> 3).Clear();
			bytes[0] |= (Byte)(0b1111_1111 << bitOffset);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Equals() on BitSpan will always throw an exception. Use == instead.")]
		public override Boolean Equals(Object obj) => throw new NotSupportedException();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetHashCode() on BitSpan will always throw an exception.")]
		public override Int32 GetHashCode() => throw new NotSupportedException();

		public Enumerator GetEnumerator() => new Enumerator(this);

		public ref struct Enumerator
		{
			readonly BitSpan bitSpan;
			Int32 index;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Enumerator(BitSpan bitSpan)
			{
				this.bitSpan = bitSpan;
				index = -1;
			}

			public Byte Current {
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => bitSpan[index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Boolean MoveNext()
			{
				int index = this.index + 1;
				if (index >= bitSpan.Length)
					return false;
				this.index = index;
				return true;
			}
		}
	}
}
