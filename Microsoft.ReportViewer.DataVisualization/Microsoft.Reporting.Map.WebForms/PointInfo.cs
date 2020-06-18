using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Map.WebForms
{
	internal struct PointInfo
	{
		public PointF Point;

		public int Index;

		public PointF[] Points;

		public GraphicsPath Path;

		public Direction Direction;

		public int GetNextIndex(Direction direction)
		{
			if (direction == Direction.Forward)
			{
				if (Index == Points.Length - 1)
				{
					return 0;
				}
				return Index + 1;
			}
			if (Index == 0)
			{
				return Points.Length - 1;
			}
			return Index - 1;
		}

		public PointInfo GetNextPoint(Direction direction)
		{
			PointInfo result = default(PointInfo);
			result.Index = GetNextIndex(direction);
			result.Points = Points;
			result.Point = result.Points[result.Index];
			result.Path = Path;
			result.Direction = direction;
			return result;
		}
	}
}
