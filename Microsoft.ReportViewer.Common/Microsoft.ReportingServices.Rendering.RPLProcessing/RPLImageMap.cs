namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageMap
	{
		private RPLFormat.ShapeType m_shape;

		private float[] m_coordinates;

		private string m_tooltip;

		public RPLFormat.ShapeType Shape
		{
			get
			{
				return m_shape;
			}
			set
			{
				m_shape = value;
			}
		}

		public float[] Coordinates
		{
			get
			{
				return m_coordinates;
			}
			set
			{
				m_coordinates = value;
			}
		}

		public string ToolTip
		{
			get
			{
				return m_tooltip;
			}
			set
			{
				m_tooltip = value;
			}
		}
	}
}
