using System;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	public sealed class CMapMapping : IComparable<CMapMapping>
	{
		internal readonly ushort Source;

		internal readonly ushort Destination;

		internal CMapMapping(ushort source, ushort destination)
		{
			Source = source;
			Destination = destination;
		}

		public int CompareTo(CMapMapping other)
		{
			if (other == null)
			{
				return 1;
			}
			return Source.CompareTo(other.Source);
		}

		internal int GetSourceLeftByte()
		{
			return Source >> 8;
		}

		internal int GetSourceDelta(CMapMapping other)
		{
			return Source - other.Source;
		}

		internal int GetDestinationDelta(CMapMapping other)
		{
			return Destination - other.Destination;
		}
	}
}
