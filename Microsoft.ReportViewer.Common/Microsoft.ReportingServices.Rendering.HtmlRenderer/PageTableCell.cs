using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class PageTableCell
	{
		[NonSerialized]
		private RoundedFloat m_x = new RoundedFloat(0f);

		[NonSerialized]
		private RoundedFloat m_y = new RoundedFloat(0f);

		private RoundedFloat m_dx = new RoundedFloat(0f);

		private RoundedFloat m_dy = new RoundedFloat(0f);

		private bool m_consumedByEmptyWhiteSpace;

		private int m_rowSpan = 1;

		private int m_colSpan = 1;

		private bool m_keepRightBorder;

		private bool m_keepBottomBorder;

		private bool m_fInUse;

		private bool m_fEaten;

		private bool m_vertMerge;

		private bool m_horzMerge;

		private bool m_firstHorzMerge;

		private bool m_firstVertMerge;

		[NonSerialized]
		private int m_usedCell = -1;

		private RPLItemMeasurement m_measurement;

		private RPLLine m_borderLeft;

		private RPLLine m_borderTop;

		private RPLLine m_borderRight;

		private RPLLine m_borderBottom;

		internal bool VertMerge
		{
			get
			{
				return m_vertMerge;
			}
			set
			{
				m_vertMerge = value;
			}
		}

		internal bool HorzMerge
		{
			get
			{
				return m_horzMerge;
			}
			set
			{
				m_horzMerge = value;
			}
		}

		internal bool FirstHorzMerge
		{
			get
			{
				return m_firstHorzMerge;
			}
			set
			{
				m_firstHorzMerge = value;
			}
		}

		internal bool FirstVertMerge
		{
			get
			{
				return m_firstVertMerge;
			}
			set
			{
				m_firstVertMerge = value;
			}
		}

		internal bool KeepRightBorder
		{
			get
			{
				return m_keepRightBorder;
			}
			set
			{
				m_keepRightBorder = value;
			}
		}

		internal bool KeepBottomBorder
		{
			get
			{
				return m_keepBottomBorder;
			}
			set
			{
				m_keepBottomBorder = value;
			}
		}

		internal bool ConsumedByEmptyWhiteSpace
		{
			get
			{
				return m_consumedByEmptyWhiteSpace;
			}
			set
			{
				m_consumedByEmptyWhiteSpace = value;
			}
		}

		internal bool HasBorder
		{
			get
			{
				if (m_borderLeft == null && m_borderTop == null && m_borderRight == null)
				{
					return m_borderBottom != null;
				}
				return true;
			}
		}

		internal RPLLine BorderLeft
		{
			get
			{
				return m_borderLeft;
			}
			set
			{
				m_borderLeft = value;
			}
		}

		internal RPLLine BorderRight
		{
			get
			{
				return m_borderRight;
			}
			set
			{
				m_borderRight = value;
			}
		}

		internal RPLLine BorderBottom
		{
			get
			{
				return m_borderBottom;
			}
			set
			{
				m_borderBottom = value;
			}
		}

		internal RPLLine BorderTop
		{
			get
			{
				return m_borderTop;
			}
			set
			{
				m_borderTop = value;
			}
		}

		internal int UsedCell
		{
			get
			{
				return m_usedCell;
			}
			set
			{
				m_usedCell = value;
			}
		}

		internal RoundedFloat XValue => m_x;

		internal RoundedFloat DXValue => m_dx;

		internal RoundedFloat YValue => m_y;

		internal RoundedFloat DYValue => m_dy;

		internal int RowSpan
		{
			get
			{
				return m_rowSpan;
			}
			set
			{
				m_rowSpan = value;
			}
		}

		internal int ColSpan
		{
			get
			{
				return m_colSpan;
			}
			set
			{
				m_colSpan = value;
			}
		}

		internal bool InUse
		{
			get
			{
				return m_fInUse;
			}
			set
			{
				m_fInUse = value;
			}
		}

		internal bool Eaten
		{
			get
			{
				return m_fEaten;
			}
			set
			{
				m_fEaten = value;
			}
		}

		internal RPLItemMeasurement Measurement => m_measurement;

		internal bool NeedsRowHeight
		{
			get
			{
				if (!Eaten && InUse && RowSpan == 1)
				{
					RPLItemMeasurement measurement = Measurement;
					if (measurement != null)
					{
						RPLElement element = measurement.Element;
						if (element is RPLTablix || element is RPLSubReport || element is RPLRectangle)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		internal PageTableCell(float x, float y, float dx, float dy)
		{
			m_x.Value = x;
			m_y.Value = y;
			m_dx.Value = dx;
			m_dy.Value = dy;
		}

		internal void MarkCellEaten(int index)
		{
			m_fEaten = true;
			m_usedCell = index;
		}

		internal void MarkCellUsed(RPLItemMeasurement measurement, int colSpan, int rowSpan, int index)
		{
			m_colSpan = colSpan;
			m_rowSpan = rowSpan;
			m_usedCell = index;
			m_measurement = measurement;
			m_fInUse = true;
		}
	}
}
