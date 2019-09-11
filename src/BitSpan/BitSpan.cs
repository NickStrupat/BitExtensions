using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NickStrupat
{
	public readonly ref struct BitSpan
	{
		readonly Span<Byte> bytes;
		readonly Byte start;
		readonly Int32 length;

		public BitSpan(Span<Byte> bytes)
		{
			this.bytes = bytes;
			start = 0;
			length = bytes.Length << 3;
		}

		public BitSpan(Span<Byte> bytes, Int32 start)
		{
			this.bytes = bytes.Slice(start >> 3);
			this.start = (Byte)(start & 0b111);
			length = this.bytes.Length << 3;
		}

		public BitSpan(Span<Byte> bytes, Int32 start, Int32 length)
		{
			var startBytes = start >> 3;
			var lengthBytes = (length >> 3) - startBytes;
			this.bytes = bytes.Slice(startBytes, lengthBytes);
			var startBits = (Byte)(start & 0b111);
			this.start = startBits;
			this.length = (lengthBytes << 3) + (length & 0b111) - startBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 CheckIndex(Int32 index) =>
			start + index < length ? start + index : throw new ArgumentOutOfRangeException(nameof(index));

		public Byte this[Int32 index]
		{
			get => bytes.GetBit(CheckIndex(index));
			set => bytes.WriteBit(CheckIndex(index), value);
		}

		public static Boolean operator ==(BitSpan left, BitSpan right) =>
			left.length == right.length &
			left.start == right.start &&
			left.bytes == right.bytes;
		public static Boolean operator !=(BitSpan left, BitSpan right) => !(left == right);

		public static BitSpan Empty => default;
		public Boolean IsEmpty => length - start == 0 | bytes.IsEmpty;

		public Int32 Length => length - start;

		public override String ToString()
		{
			var sb = new StringBuilder(Length);
			foreach (var bit in this)
				sb.Append(bit == 0 ? '0' : '1');
			return sb.ToString();
		}

		public void Clear()
		{
			var hasBitOffset = start != 0;
			var hasBitLength = length != (bytes.Length << 3) - start;
			if (hasBitOffset)
				bytes[0] &= (Byte)((1 << start) - 1);
			var sliceStart = Unsafe.As<Boolean, Byte>(ref hasBitOffset);
			var sliceLength = Unsafe.As<Boolean, Byte>(ref hasBitLength);
			var lastLength = length >> 3;
			bytes.Slice(sliceStart, lastLength - sliceLength).Clear();
			if (hasBitLength)
				bytes[bytes.Length - 1] &= (Byte)~((1 << lastLength) - 1);
		}

		public void Fill(Boolean value) => throw null;

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
