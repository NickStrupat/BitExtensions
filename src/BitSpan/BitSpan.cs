using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NickStrupat
{
	public readonly ref struct BitSpan
	{
		public readonly Span<Byte> Bytes;
		private readonly Byte bitStart;
		private readonly Int32 bitLength;

		public BitSpan(Span<Byte> bytes)
		{
			Bytes = bytes;
			bitStart = 0;
			bitLength = bytes.Length * 8;
		}

		public BitSpan(Span<Byte> bytes, Int32 bitStart)
		{
			Bytes = bytes[(bitStart / 8)..];
			this.bitStart = (Byte)(bitStart & 0b111);
			bitLength = Bytes.Length * 8 - this.bitStart;
			if (bitLength < 0)
				throw new ArgumentOutOfRangeException(nameof(bitStart));
		}

		public BitSpan(Span<Byte> bytes, Int32 bitStart, Int32 length)
		{
			var startBytes = bitStart / 8;
			var lengthBytes = (length / 8) - startBytes;
			Bytes = bytes.Slice(startBytes, lengthBytes);
			var startBits = (Byte)(bitStart & 0b111);
			this.bitStart = startBits;
			this.bitLength = (lengthBytes << 3) + (length & 0b111) - startBits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 CheckIndex(Int32 index) =>
			bitStart + index < bitLength ? bitStart + index : throw new ArgumentOutOfRangeException(nameof(index));

		public Byte this[Int32 index]
		{
			get => Bytes.GetBit(CheckIndex(index));
			set => Bytes.WriteBit(CheckIndex(index), value);
		}

		public static Boolean operator ==(BitSpan left, BitSpan right)
		{
			if (left.Length != right.Length)
				return false;
			if ((left.Length == left.Bytes.Length * 8) & (right.Length == right.Bytes.Length * 8))
				return left.Bytes == right.Bytes;
			var l = left.GetEnumerator();
			var r = right.GetEnumerator();
			while (l.MoveNext() && r.MoveNext())
				if (l.Current != r.Current)
					return false;
			return true;
		}
		public static Boolean operator !=(BitSpan left, BitSpan right) => !(left == right);

		public static BitSpan Empty => default;
		public Boolean IsEmpty => bitLength - bitStart == 0 | Bytes.IsEmpty;

		public Int32 Length => bitLength;

		public override String ToString()
		{
			var sb = new StringBuilder(Length);
			foreach (var bit in this)
				sb.Append((Char)(bit + '0'));
			return sb.ToString();
		}

		public void Clear()
		{
			var hasBitOffset = bitStart != 0;
			var hasBitLength = bitLength != (Bytes.Length << 3) - bitStart;
			if (hasBitOffset)
				Bytes[0] &= (Byte)((1 << bitStart) - 1);
			var sliceStart = Unsafe.As<Boolean, Byte>(ref hasBitOffset);
			var sliceLength = Unsafe.As<Boolean, Byte>(ref hasBitLength);
			var lastLength = bitLength >> 3;
			Bytes.Slice(sliceStart, lastLength - sliceLength).Clear();
			if (hasBitLength)
				Bytes[^1] &= (Byte)~((1 << lastLength) - 1);
		}

		public void Set()
		{
			var hasBitOffset = bitStart != 0;
			var hasBitLength = bitLength != (Bytes.Length << 3) - bitStart;
			if (hasBitOffset)
				Bytes[0] |= (Byte)~((1 << bitStart) - 1);
			var sliceStart = Unsafe.As<Boolean, Byte>(ref hasBitOffset);
			var sliceLength = Unsafe.As<Boolean, Byte>(ref hasBitLength);
			var lastLength = bitLength >> 3;
			Bytes.Slice(sliceStart, lastLength - sliceLength).Fill(Byte.MaxValue);
			if (hasBitLength)
				Bytes[^1] |= (Byte)((1 << lastLength) - 1);
		}

		public void Fill(Boolean value)
		{
			if (value)
				Set();
			else
				Clear();
		}

#pragma warning disable CS0809
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Equals() on BitSpan will always throw an exception. Use == instead.")]
		public override Boolean Equals(Object? obj) => throw new NotSupportedException();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetHashCode() on BitSpan will always throw an exception.")]
		public override Int32 GetHashCode() => throw new NotSupportedException();
#pragma warning restore CS0809

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
				if (index + 1 >= bitSpan.Length)
					return false;
				index++;
				return true;
			}
		}
	}
}
