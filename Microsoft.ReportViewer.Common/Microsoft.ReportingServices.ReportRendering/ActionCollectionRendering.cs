using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionCollectionRendering : MemberBase
	{
		internal ActionItemList m_actionList;

		internal ActionItemInstanceList m_actionInstanceList;

		internal RenderingContext m_renderingContext;

		internal Action[] m_actions;

		internal string m_ownerUniqueName;

		internal ActionCollectionRendering()
			: base(isCustomControl: false)
		{
		}
	}
}
