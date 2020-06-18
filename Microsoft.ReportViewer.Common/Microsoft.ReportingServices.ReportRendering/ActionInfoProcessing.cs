using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionInfoProcessing : MemberBase
	{
		internal ActionStyle m_style;

		internal ActionCollection m_actionCollection;

		internal DataValueInstanceList m_sharedStyles;

		internal DataValueInstanceList m_nonSharedStyles;

		internal ActionInfoProcessing()
			: base(isCustomControl: true)
		{
		}

		internal ActionInfoProcessing DeepClone()
		{
			Global.Tracer.Assert(m_sharedStyles == null && m_nonSharedStyles == null);
			ActionInfoProcessing actionInfoProcessing = new ActionInfoProcessing();
			if (m_style != null)
			{
				m_style.ExtractRenderStyles(out actionInfoProcessing.m_sharedStyles, out actionInfoProcessing.m_nonSharedStyles);
			}
			if (m_actionCollection != null)
			{
				actionInfoProcessing.m_actionCollection = m_actionCollection.DeepClone();
			}
			return actionInfoProcessing;
		}
	}
}
