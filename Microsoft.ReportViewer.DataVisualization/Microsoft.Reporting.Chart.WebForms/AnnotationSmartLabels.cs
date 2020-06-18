using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationSmartLabels_AnnotationSmartLabels")]
	internal class AnnotationSmartLabels : SmartLabels
	{
		internal override bool IsSmartLabelCollide(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF position, SizeF size, PointF markerPosition, StringFormat format, LabelAlignmentTypes labelAlignment, bool checkCalloutLineOverlapping)
		{
			bool result = false;
			if (common.Chart != null)
			{
				foreach (ChartArea chartArea in common.Chart.ChartAreas)
				{
					if (area.Visible)
					{
						chartArea.smartLabels.checkAllCollisions = true;
						if (chartArea.smartLabels.IsSmartLabelCollide(common, graph, area, smartLabelsStyle, position, size, markerPosition, format, labelAlignment, checkCalloutLineOverlapping))
						{
							chartArea.smartLabels.checkAllCollisions = false;
							return true;
						}
						chartArea.smartLabels.checkAllCollisions = false;
					}
				}
			}
			RectangleF labelPosition = GetLabelPosition(graph, position, size, format, adjustForDrawing: false);
			bool flag = (labelAlignment == LabelAlignmentTypes.Center && !smartLabelsStyle.MarkerOverlapping) ? true : false;
			if (checkAllCollisions)
			{
				flag = false;
			}
			foreach (RectangleF smartLabelsPosition in smartLabelsPositions)
			{
				if (smartLabelsPosition.IntersectsWith(labelPosition))
				{
					if (!flag)
					{
						return true;
					}
					flag = false;
				}
			}
			return result;
		}

		internal override void AddMarkersPosition(CommonElements common, ChartArea area)
		{
			if (smartLabelsPositions.Count != 0 || common == null || common.Chart == null)
			{
				return;
			}
			foreach (Annotation annotation in common.Chart.Annotations)
			{
				annotation.AddSmartLabelMarkerPositions(common, smartLabelsPositions);
			}
		}

		internal override void DrawCallout(CommonElements common, ChartGraphics graph, ChartArea area, SmartLabelsStyle smartLabelsStyle, PointF labelPosition, SizeF labelSize, StringFormat format, PointF markerPosition, SizeF markerSize, LabelAlignmentTypes labelAlignment)
		{
		}
	}
}
