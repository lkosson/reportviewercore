using System;
using System.Collections;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class MarkerPosition : IComparable
	{
		internal float position;

		internal double value;

		internal Placement placement = Placement.Cross;

		internal MarkerPosition(float position, double value, Placement placement)
		{
			this.position = position;
			this.value = value;
			this.placement = placement;
		}

		internal static bool IsExistsInArray(ArrayList array, MarkerPosition markerPos)
		{
			foreach (MarkerPosition item in array)
			{
				if (markerPos.position == item.position)
				{
					return markerPos.placement == item.placement;
				}
			}
			return false;
		}

		internal static double Snap(ArrayList array, double value)
		{
			for (int i = 0; i < array.Count - 1; i++)
			{
				MarkerPosition markerPosition = (MarkerPosition)array[i];
				MarkerPosition markerPosition2 = (MarkerPosition)array[i + 1];
				if (markerPosition.value <= value && value <= markerPosition2.value)
				{
					if (markerPosition2.value - value < value - markerPosition.value)
					{
						return markerPosition2.value;
					}
					return markerPosition.value;
				}
			}
			return value;
		}

		public int CompareTo(object obj)
		{
			if (obj is MarkerPosition)
			{
				if (value < ((MarkerPosition)obj).value)
				{
					return -1;
				}
				if (value > ((MarkerPosition)obj).value)
				{
					return 1;
				}
			}
			return 0;
		}
	}
}
