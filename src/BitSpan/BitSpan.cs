using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NickStrupat
{
	public readonly ref struct BitSpan
	{
		readonly Span<Byte> bytes;
		readonly Byte bitOffset;

		public BitSpan(Span<Byte> bytes, Byte bitOffset)
		{
			this.bytes = bytes;
			this.bitOffset = bitOffset < bytes.Length * 8 ? bitOffset : throw new ArgumentOutOfRangeException(nameof(bitOffset));
		}

		public Byte this[Int32 index]
		{
			get => bytes.GetBit(index);
			set => bytes.WriteBit(index, value);
		}

		public static BitSpan Empty => default;
		public Boolean IsEmpty => bytes.IsEmpty | bitOffset >> 3 == 0;

		public Int32 Length => (bytes.Length << 3) - bitOffset;

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
