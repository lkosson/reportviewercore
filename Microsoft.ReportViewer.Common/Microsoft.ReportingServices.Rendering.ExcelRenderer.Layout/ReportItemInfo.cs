using Microsoft.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class ReportItemInfo
	{
		private object m_rplSource;

		private int m_top;

		private int m_left;

		private int m_right;

		private int m_alignmentPoint;

		private RPLTextBoxProps m_textBox;

		private Dictionary<string, ToggleParent> m_toggleParents;

		private bool m_isHidden;

		internal object RPLSource => m_rplSource;

		internal int Top => m_top;

		internal int Left => m_left;

		internal int Right => m_right;

		internal RPLTextBoxProps Values
		{
			get
			{
				return m_textBox;
			}
			set
			{
				m_textBox = value;
			}
		}

		internal int AlignmentPoint
		{
			get
			{
				return m_alignmentPoint;
			}
			set
			{
				m_alignmentPoint = value;
			}
		}

		internal bool IsHidden => m_isHidden;

		internal Dictionary<string, ToggleParent> ToggleParents => m_toggleParents;

		internal ReportItemInfo(object aRplSource, int aTop, int aLeft, int aRight, bool aIsHidden, Dictionary<string, ToggleParent> aToggleParents)
		{
			m_rplSource = aRplSource;
			m_top = aTop;
			m_left = aLeft;
			m_right = aRight;
			m_isHidden = aIsHidden;
			m_toggleParents = aToggleParents;
		}

		internal static int CompareTopsThenLefts(ReportItemInfo aLeft, ReportItemInfo aRight)
		{
			int num = aLeft.Top.CompareTo(aRight.Top);
			if (num == 0)
			{
				return aLeft.Left.CompareTo(aRight.Left);
			}
			return num;
		}
	}
}
