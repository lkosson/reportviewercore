using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class Visibility
	{
		protected ReportBoolProperty m_startHidden;

		public abstract ReportBoolProperty Hidden
		{
			get;
		}

		public abstract string ToggleItem
		{
			get;
		}

		public abstract SharedHiddenState HiddenState
		{
			get;
		}

		public abstract bool RecursiveToggleReceiver
		{
			get;
		}

		internal static ReportBoolProperty GetStartHidden(Microsoft.ReportingServices.ReportProcessing.Visibility visibility)
		{
			ReportBoolProperty reportBoolProperty = null;
			if (visibility == null)
			{
				return new ReportBoolProperty();
			}
			return new ReportBoolProperty(visibility.Hidden);
		}

		internal static ReportBoolProperty GetStartHidden(Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility)
		{
			ReportBoolProperty reportBoolProperty = null;
			if (visibility == null)
			{
				return new ReportBoolProperty();
			}
			return new ReportBoolProperty(visibility.Hidden);
		}

		internal static SharedHiddenState GetHiddenState(Microsoft.ReportingServices.ReportProcessing.Visibility visibility)
		{
			return (SharedHiddenState)Microsoft.ReportingServices.ReportProcessing.Visibility.GetSharedHidden(visibility);
		}

		internal static SharedHiddenState GetHiddenState(Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility)
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Visibility.GetSharedHidden(visibility);
		}
	}
}
