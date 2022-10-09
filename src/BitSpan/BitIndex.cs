using System;

namespace NickStrupat;

public readonly ref struct BitIndex
{
	private readonly Byte index;
	internal BitIndex(Byte index) => this.index = index;
	public static explicit operator Byte(BitIndex bitIndex) => bitIndex.index;
	public static explicit operator BitIndex(Byte index) => new(index <= 7 ? index : throw new ArgumentOutOfRangeException(nameof(index)));
}

public readonly ref struct UInt1
{
	private readonly Byte value;
	private UInt1(Byte value) => this.value = value;
	public static explicit operator UInt1(Byte value) => new(value < 2 ? value : throw new ArgumentOutOfRangeException(nameof(value)));
	public static explicit operator Byte(UInt1 value) => value.value;
}

public readonly ref struct UInt2
{
	private readonly Byte value;
	private UInt2(Byte value) => this.value = value;
	public static explicit operator UInt2(Byte value) => new(value < 4 ? value : throw new ArgumentOutOfRangeException(nameof(value)));
	public static explicit operator Byte(UInt2 value) => value.value;
}