using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class Gap
	{
		public float Inside;

		public float Center;

		public float Outside;

		private float baseInside;

		private float baseOutside;

		public float InsideGap => Center - Inside;

		public float OutsideGap => Center + Outside;

		public Gap(float center)
		{
			Center = center;
		}

		public void SetBase()
		{
			baseInside = Inside;
			baseOutside = Outside;
		}

		public void SetOffset(Placement placement, float length)
		{
			switch (placement)
			{
			case Placement.Inside:
				Inside += length;
				break;
			case Placement.Cross:
				Inside += length / 2f;
				Outside += length / 2f;
				break;
			case Placement.Outside:
				Outside += length;
				break;
			default:
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}

		public void SetOffsetBase(Placement placement, float length)
		{
			switch (placement)
			{
			case Placement.Inside:
				Inside = Math.Max(Inside, baseInside + length);
				break;
			case Placement.Cross:
				Inside = Math.Max(Inside, length / 2f);
				Outside = Math.Max(Outside, length / 2f);
				break;
			case Placement.Outside:
				Outside = Math.Max(Outside, baseOutside + length);
				break;
			default:
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}
	}
}
