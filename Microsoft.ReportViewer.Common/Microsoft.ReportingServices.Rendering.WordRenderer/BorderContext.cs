namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class BorderContext
	{
		internal static readonly BorderContext EmptyBorder = new BorderContext(0);

		internal static readonly BorderContext TopBorder = new BorderContext(1);

		internal static readonly BorderContext LeftBorder = new BorderContext(2);

		internal static readonly BorderContext RightBorder = new BorderContext(8);

		internal static readonly BorderContext BottomBorder = new BorderContext(4);

		internal static readonly BorderContext TopLeftBorder = new BorderContext(3);

		internal static readonly BorderContext TopRightBorder = new BorderContext(9);

		internal static readonly BorderContext BottomLeftBorder = new BorderContext(6);

		internal static readonly BorderContext BottomRightBorder = new BorderContext(12);

		private const int TopBit = 1;

		private const int LeftBit = 2;

		private const int BottomBit = 4;

		private const int RightBit = 8;

		private int m_borderContext;

		internal bool Top
		{
			get
			{
				return (m_borderContext & 1) > 0;
			}
			set
			{
				if (value)
				{
					m_borderContext |= 1;
				}
				else
				{
					m_borderContext &= -2;
				}
			}
		}

		internal bool Left
		{
			get
			{
				return (m_borderContext & 2) > 0;
			}
			set
			{
				if (value)
				{
					m_borderContext |= 2;
				}
				else
				{
					m_borderContext &= -3;
				}
			}
		}

		internal bool Bottom
		{
			get
			{
				return (m_borderContext & 4) > 0;
			}
			set
			{
				if (value)
				{
					m_borderContext |= 4;
				}
				else
				{
					m_borderContext &= -5;
				}
			}
		}

		internal bool Right
		{
			get
			{
				return (m_borderContext & 8) > 0;
			}
			set
			{
				if (value)
				{
					m_borderContext |= 8;
				}
				else
				{
					m_borderContext &= -9;
				}
			}
		}

		internal BorderContext()
		{
			m_borderContext = 0;
		}

		internal BorderContext(BorderContext borderContext)
		{
			m_borderContext = borderContext.m_borderContext;
		}

		internal BorderContext(int borderContext)
		{
			m_borderContext = borderContext;
		}

		internal void Reset()
		{
			m_borderContext = 0;
		}
	}
}
