namespace Microsoft.ReportingServices.ReportRendering
{
	internal class MemberBase
	{
		private bool m_customControl;

		internal bool IsCustomControl => m_customControl;

		internal MemberBase(bool isCustomControl)
		{
			m_customControl = isCustomControl;
		}
	}
}
