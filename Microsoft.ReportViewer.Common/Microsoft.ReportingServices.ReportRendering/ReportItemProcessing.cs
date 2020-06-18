using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportItemProcessing : MemberBase
	{
		internal string DefinitionName;

		internal string Label;

		internal string Bookmark;

		internal string Tooltip;

		internal ReportSize Height;

		internal ReportSize Width;

		internal ReportSize Top;

		internal ReportSize Left;

		internal int ZIndex;

		internal bool Hidden;

		internal SharedHiddenState SharedHidden = SharedHiddenState.Never;

		internal DataValueInstanceList SharedStyles;

		internal DataValueInstanceList NonSharedStyles;

		internal ReportItemProcessing()
			: base(isCustomControl: true)
		{
		}

		internal ReportItemProcessing DeepClone()
		{
			ReportItemProcessing reportItemProcessing = new ReportItemProcessing();
			if (DefinitionName != null)
			{
				reportItemProcessing.DefinitionName = string.Copy(DefinitionName);
			}
			if (Label != null)
			{
				reportItemProcessing.Label = string.Copy(Label);
			}
			if (Bookmark != null)
			{
				reportItemProcessing.Bookmark = string.Copy(Bookmark);
			}
			if (Tooltip != null)
			{
				reportItemProcessing.Tooltip = string.Copy(Tooltip);
			}
			if (Height != null)
			{
				reportItemProcessing.Height = Height.DeepClone();
			}
			if (Width != null)
			{
				reportItemProcessing.Width = Width.DeepClone();
			}
			if (Top != null)
			{
				reportItemProcessing.Top = Top.DeepClone();
			}
			if (Left != null)
			{
				reportItemProcessing.Left = Left.DeepClone();
			}
			reportItemProcessing.ZIndex = ZIndex;
			reportItemProcessing.Hidden = Hidden;
			reportItemProcessing.SharedHidden = SharedHidden;
			Global.Tracer.Assert(SharedStyles == null && NonSharedStyles == null);
			return reportItemProcessing;
		}
	}
}
