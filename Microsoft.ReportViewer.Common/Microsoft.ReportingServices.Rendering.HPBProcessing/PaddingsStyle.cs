using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PaddingsStyle
	{
		[Flags]
		internal enum PaddingState : byte
		{
			Clear = 0x0,
			Left = 0x1,
			Right = 0x2,
			Top = 0x4,
			Bottom = 0x8
		}

		private double m_padHorizontal;

		private double m_padVertical;

		private double m_padTop;

		private PaddingState m_state;

		internal double PadHorizontal
		{
			get
			{
				return m_padHorizontal;
			}
			set
			{
				m_padHorizontal = value;
			}
		}

		internal double PadTop
		{
			set
			{
				m_padTop = value;
			}
		}

		internal double PadVertical
		{
			get
			{
				return m_padVertical;
			}
			set
			{
				m_padVertical = value;
			}
		}

		internal PaddingState State
		{
			get
			{
				return m_state;
			}
			set
			{
				m_state = value;
			}
		}

		private static ReportSize GetStyleValue(StyleAttributeNames styleName, ReportItem source)
		{
			bool shared = false;
			return GetStyleValue(styleName, ref shared, source);
		}

		private static ReportSize GetStyleValue(StyleAttributeNames styleName, ref bool shared, ReportItem source)
		{
			object obj = null;
			ReportProperty reportProperty = source.Style[styleName];
			if (reportProperty != null)
			{
				if (reportProperty.IsExpression)
				{
					shared = false;
					obj = source.Instance.Style[styleName];
				}
				if (obj == null)
				{
					obj = ((ReportSizeProperty)reportProperty).Value;
				}
			}
			return obj as ReportSize;
		}

		internal static void CreatePaddingsStyle(PageContext pageContext, ReportItem source, out double padVertical, out double padHorizontal, out double padTop)
		{
			padVertical = 0.0;
			padHorizontal = 0.0;
			padTop = 0.0;
			PaddingsStyle paddingsStyle = null;
			bool shared = true;
			double num = 0.0;
			ReportSize styleValue = GetStyleValue(StyleAttributeNames.PaddingTop, ref shared, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (shared)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadVertical += num;
					paddingsStyle.PadTop = num;
					paddingsStyle.State |= PaddingState.Top;
				}
				padTop = num;
				padVertical += num;
			}
			shared = true;
			styleValue = GetStyleValue(StyleAttributeNames.PaddingBottom, ref shared, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (shared)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadVertical += num;
					paddingsStyle.State |= PaddingState.Bottom;
				}
				padVertical += num;
			}
			shared = true;
			styleValue = GetStyleValue(StyleAttributeNames.PaddingLeft, ref shared, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (shared)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadHorizontal += num;
					paddingsStyle.State |= PaddingState.Left;
				}
				padHorizontal += num;
			}
			shared = true;
			styleValue = GetStyleValue(StyleAttributeNames.PaddingRight, ref shared, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (shared)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadHorizontal += num;
					paddingsStyle.State |= PaddingState.Right;
				}
				padHorizontal += num;
			}
			if (paddingsStyle != null)
			{
				if (pageContext.ItemPaddingsStyle == null)
				{
					pageContext.ItemPaddingsStyle = new Hashtable();
				}
				pageContext.ItemPaddingsStyle.Add(source.ID, paddingsStyle);
			}
		}

		internal void GetPaddingValues(ReportItem source, out double padVertical, out double padHorizontal, out double padTop)
		{
			padVertical = m_padVertical;
			padHorizontal = m_padHorizontal;
			padTop = m_padTop;
			ReportSize reportSize = null;
			if ((m_state & PaddingState.Top) == 0)
			{
				reportSize = GetStyleValue(StyleAttributeNames.PaddingTop, source);
				if (reportSize != null)
				{
					padTop = reportSize.ToMillimeters();
					padVertical += padTop;
				}
			}
			if ((m_state & PaddingState.Bottom) == 0)
			{
				reportSize = GetStyleValue(StyleAttributeNames.PaddingBottom, source);
				if (reportSize != null)
				{
					padVertical += reportSize.ToMillimeters();
				}
			}
			if ((m_state & PaddingState.Left) == 0)
			{
				reportSize = GetStyleValue(StyleAttributeNames.PaddingLeft, source);
				if (reportSize != null)
				{
					padHorizontal += reportSize.ToMillimeters();
				}
			}
			if ((m_state & PaddingState.Right) == 0)
			{
				reportSize = GetStyleValue(StyleAttributeNames.PaddingRight, source);
				if (reportSize != null)
				{
					padHorizontal += reportSize.ToMillimeters();
				}
			}
		}
	}
}
