using System;
using System.Collections;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class BarPointsDrawingOrderComparer : IComparer
	{
		private ChartArea area;

		private Point3D areaProjectionCenter = new Point3D(float.NaN, float.NaN, float.NaN);

		private bool selection;

		public BarPointsDrawingOrderComparer(ChartArea area, bool selection, COPCoordinates coord)
		{
			this.area = area;
			this.selection = selection;
			if (area.DrawPointsToCenter(ref coord))
			{
				areaProjectionCenter = area.GetCenterOfProjection(coord);
				float x = areaProjectionCenter.X;
				areaProjectionCenter.X = areaProjectionCenter.Y;
				areaProjectionCenter.Y = x;
			}
		}

		public int Compare(object o1, object o2)
		{
			DataPoint3D dataPoint3D = (DataPoint3D)o1;
			DataPoint3D dataPoint3D2 = (DataPoint3D)o2;
			int num = 0;
			if (dataPoint3D.xPosition < dataPoint3D2.xPosition)
			{
				num = -1;
			}
			else if (dataPoint3D.xPosition > dataPoint3D2.xPosition)
			{
				num = 1;
			}
			else
			{
				if (dataPoint3D.yPosition < dataPoint3D2.yPosition)
				{
					num = 1;
				}
				else if (dataPoint3D.yPosition > dataPoint3D2.yPosition)
				{
					num = -1;
				}
				if (!float.IsNaN(areaProjectionCenter.Y))
				{
					double num2 = Math.Min(dataPoint3D.yPosition, dataPoint3D.height);
					double num3 = Math.Max(dataPoint3D.yPosition, dataPoint3D.height);
					double num4 = Math.Min(dataPoint3D2.yPosition, dataPoint3D2.height);
					double num5 = Math.Max(dataPoint3D2.yPosition, dataPoint3D2.height);
					if (!area.IsBottomSceneWallVisible())
					{
						num = ((num3 >= (double)areaProjectionCenter.Y && num5 >= (double)areaProjectionCenter.Y) ? num : ((num3 >= (double)areaProjectionCenter.Y) ? 1 : (num * -1)));
					}
					else if (num2 <= (double)areaProjectionCenter.Y && num4 <= (double)areaProjectionCenter.Y)
					{
						num *= -1;
					}
					else if (num2 <= (double)areaProjectionCenter.Y)
					{
						num = 1;
					}
				}
				else if (!area.DrawPointsInReverseOrder())
				{
					num *= -1;
				}
			}
			if (dataPoint3D.xPosition != dataPoint3D2.xPosition)
			{
				if (!float.IsNaN(areaProjectionCenter.X))
				{
					if (dataPoint3D.xPosition + dataPoint3D.width / 2.0 >= (double)areaProjectionCenter.X && dataPoint3D2.xPosition + dataPoint3D2.width / 2.0 >= (double)areaProjectionCenter.X)
					{
						num *= -1;
					}
				}
				else if (area.IsBottomSceneWallVisible())
				{
					num *= -1;
				}
			}
			if (!selection)
			{
				return num;
			}
			return -num;
		}
	}
}
