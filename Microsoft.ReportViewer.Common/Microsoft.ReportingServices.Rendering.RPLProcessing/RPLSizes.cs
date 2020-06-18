namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLSizes
	{
		protected float m_left;

		protected float m_top;

		protected float m_width;

		protected float m_height;

		public float Left
		{
			get
			{
				return m_left;
			}
			set
			{
				m_left = value;
			}
		}

		public float Top
		{
			get
			{
				return m_top;
			}
			set
			{
				m_top = value;
			}
		}

		public float Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		public float Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		public RPLSizes()
		{
		}

		public RPLSizes(float top, float left, float height, float width)
		{
			m_top = top;
			m_left = left;
			m_height = height;
			m_width = width;
		}

		public RPLSizes(RPLSizes sizes)
		{
			m_top = sizes.Top;
			m_left = sizes.Left;
			m_height = sizes.Height;
			m_width = sizes.Width;
		}
	}
}
