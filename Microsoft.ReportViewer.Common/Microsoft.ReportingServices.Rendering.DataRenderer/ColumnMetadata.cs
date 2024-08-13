namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class ColumnMetadata
{
	private bool m_isDynamic;

	public bool IsDynamic
	{
		get
		{
			return m_isDynamic;
		}
		set
		{
			m_isDynamic = value;
		}
	}
}
