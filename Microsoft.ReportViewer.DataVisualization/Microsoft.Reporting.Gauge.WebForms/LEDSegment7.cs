using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[Flags]
	internal enum LEDSegment7 : ulong
	{
		Empty = 0x0uL,
		SA = 0x1uL,
		SB = 0x2uL,
		SC = 0x4uL,
		SD = 0x8uL,
		SE = 0x10uL,
		SF = 0x20uL,
		SG = 0x40uL,
		SDP = 0x4000uL,
		SComma = 0x8000uL,
		N1 = 0x6uL,
		N2 = 0x5BuL,
		N3 = 0x4FuL,
		N4 = 0x66uL,
		N5 = 0x6DuL,
		N6 = 0x7DuL,
		N7 = 0x7uL,
		N8 = 0x7FuL,
		N9 = 0x6FuL,
		N0 = 0x3FuL,
		Neg = 0x40uL,
		Unknown = 0x79uL,
		All = 0x407FuL
	}
}
