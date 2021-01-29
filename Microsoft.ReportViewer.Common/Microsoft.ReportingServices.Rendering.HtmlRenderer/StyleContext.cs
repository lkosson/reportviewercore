namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class StyleContext
	{
		private bool m_inTablix;

		private bool m_styleOnCell;

		private bool m_renderMeasurements = true;

		private bool m_noBorders;

		private bool m_emptyTextBox;

		private bool m_onlyRenderMeasurementsBackgroundBorders;

		private byte m_omitBordersState;

		private bool m_ignoreVerticalAlign;

		private bool m_renderMinMeasurements;

		private bool m_ignorePadding;

		private bool m_rotationApplied;

		private bool m_zeroWidth;

		public bool EmptyTextBox
		{
			get
			{
				return m_emptyTextBox;
			}
			set
			{
				m_emptyTextBox = value;
			}
		}

		public bool NoBorders
		{
			get
			{
				return m_noBorders;
			}
			set
			{
				m_noBorders = value;
			}
		}

		public bool InTablix
		{
			get
			{
				return m_inTablix;
			}
			set
			{
				m_inTablix = value;
			}
		}

		public bool StyleOnCell
		{
			get
			{
				return m_styleOnCell;
			}
			set
			{
				m_styleOnCell = value;
			}
		}

		public bool RenderMeasurements
		{
			get
			{
				return m_renderMeasurements;
			}
			set
			{
				m_renderMeasurements = value;
			}
		}

		public bool RenderMinMeasurements
		{
			get
			{
				return m_renderMinMeasurements;
			}
			set
			{
				m_renderMinMeasurements = value;
			}
		}

		public bool OnlyRenderMeasurementsBackgroundBorders
		{
			get
			{
				return m_onlyRenderMeasurementsBackgroundBorders;
			}
			set
			{
				m_onlyRenderMeasurementsBackgroundBorders = value;
			}
		}

		public byte OmitBordersState
		{
			get
			{
				return m_omitBordersState;
			}
			set
			{
				m_omitBordersState = value;
			}
		}

		public bool IgnoreVerticalAlign
		{
			get
			{
				return m_ignoreVerticalAlign;
			}
			set
			{
				m_ignoreVerticalAlign = value;
			}
		}

		public bool IgnorePadding
		{
			get
			{
				return m_ignorePadding;
			}
			set
			{
				m_ignorePadding = value;
			}
		}

		public bool RotationApplied
		{
			get
			{
				return m_rotationApplied;
			}
			set
			{
				m_rotationApplied = value;
			}
		}

		public bool ZeroWidth
		{
			get
			{
				return m_zeroWidth;
			}
			set
			{
				m_zeroWidth = value;
			}
		}

		public void Reset()
		{
			m_inTablix = false;
			m_styleOnCell = false;
			m_renderMeasurements = true;
			m_noBorders = false;
			m_emptyTextBox = false;
			m_omitBordersState = 0;
			m_ignoreVerticalAlign = false;
			m_ignorePadding = false;
			m_rotationApplied = false;
			m_ignoreVerticalAlign = false;
			m_zeroWidth = false;
		}
	}
}
