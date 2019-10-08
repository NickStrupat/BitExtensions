using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NickStrupat
{
	public readonly ref struct BitSpan
	{
		public readonly Span<Byte> Bytes;
		private readonly Byte start;
		private readonly Int32 length;

		public BitSpan(Span<Byte> bytes)
		{
			Bytes = bytes;
			start = 0;
			length = bytes.Length << 3;
		}

		public BitSpan(Span<Byte> bytes, Int32 start)
		{
			Bytes = bytes.Slice(start >> 3);
			this.start = (Byte)(start & 0b111);
			length = Bytes.Length << 3;
		}

		public BitSpan(Span<Byte> bytes, Int32 start, Int32 length)
		{
			var startBytes = start >> 3;
			var lengthBytes = (length >> 3) - startBytes;
			Bytes = bytes.Slice(startBytes, lengthBytes);
			var startBits = (Byte)(start & 0b111);
			this.start = startBits;
			this.length = (lengthBytes << 3) + (length & 0b111) - startBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 CheckIndex(Int32 index) =>
			start + index < length ? start + index : throw new ArgumentOutOfRangeException(nameof(index));

		public Byte this[Int32 index]
		{
			get => Bytes.GetBit(CheckIndex(index));
			set => Bytes.WriteBit(CheckIndex(index), value);
		}

		public static Boolean operator ==(BitSpan left, BitSpan right)
		{
			if ((left.Length == left.Bytes.Length << 3) & (right.Length == right.Bytes.Length << 3))
				return left.Bytes == right.Bytes;
			
			return false;
		}
		public static Boolean operator !=(BitSpan left, BitSpan right) => !(left == right);

		public static BitSpan Empty => default;
		public Boolean IsEmpty => length - start == 0 | Bytes.IsEmpty;

		public Int32 Length => length - start;

		public override String ToString()
		{
			var sb = new StringBuilder(Length);
			foreach (var bit in this)
				sb.Append((Char)(bit + '0'));
			return sb.ToString();
		}

		public void Clear()
		{
			var hasBitOffset = start != 0;
			var hasBitLength = length != (Bytes.Length << 3) - start;
			if (hasBitOffset)
				Bytes[0] &= (Byte)((1 << start) - 1);
			var sliceStart = Unsafe.As<Boolean, Byte>(ref hasBitOffset);
			var sliceLength = Unsafe.As<Boolean, Byte>(ref hasBitLength);
			var lastLength = length >> 3;
			Bytes.Slice(sliceStart, lastLength - sliceLength).Clear();
			if (hasBitLength)
				Bytes[Bytes.Length - 1] &= (Byte)~((1 << lastLength) - 1);
		}

		public void Set()
		{
			var hasBitOffset = start != 0;
			var hasBitLength = length != (Bytes.Length << 3) - start;
			if (hasBitOffset)
				Bytes[0] |= (Byte)~((1 << start) - 1);
			var sliceStart = Unsafe.As<Boolean, Byte>(ref hasBitOffset);
			var sliceLength = Unsafe.As<Boolean, Byte>(ref hasBitLength);
			var lastLength = length >> 3;
			Bytes.Slice(sliceStart, lastLength - sliceLength).Fill(Byte.MaxValue);
			if (hasBitLength)
				Bytes[Bytes.Length - 1] |= (Byte)((1 << lastLength) - 1);
		}

		public void Fill(Boolean value)
		{
			if (value)
				Set();
			else
				Clear();
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
