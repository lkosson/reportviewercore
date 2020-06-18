using System;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextBoxProps : RPLItemProps
	{
		private bool m_toggleState;

		private RPLFormat.SortOptions m_sortState;

		private bool m_isToggleParent;

		private TypeCode m_typeCode;

		private object m_originalValue;

		private string m_value;

		private RPLActionInfo m_actionInfo;

		private float m_contentHeight;

		private float m_contentOffset;

		private bool m_processedWithError;

		public string Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		public object OriginalValue
		{
			get
			{
				return m_originalValue;
			}
			set
			{
				m_originalValue = value;
			}
		}

		public TypeCode TypeCode
		{
			get
			{
				return m_typeCode;
			}
			set
			{
				m_typeCode = value;
			}
		}

		public bool IsToggleParent
		{
			get
			{
				return m_isToggleParent;
			}
			set
			{
				m_isToggleParent = value;
			}
		}

		public bool ToggleState
		{
			get
			{
				return m_toggleState;
			}
			set
			{
				m_toggleState = value;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return m_actionInfo;
			}
			set
			{
				m_actionInfo = value;
			}
		}

		public RPLFormat.SortOptions SortState
		{
			get
			{
				return m_sortState;
			}
			set
			{
				m_sortState = value;
			}
		}

		public float ContentHeight
		{
			get
			{
				return m_contentHeight;
			}
			set
			{
				m_contentHeight = value;
			}
		}

		public float ContentOffset
		{
			get
			{
				return m_contentOffset;
			}
			set
			{
				m_contentOffset = value;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				return m_processedWithError;
			}
			set
			{
				m_processedWithError = value;
			}
		}

		internal RPLTextBoxProps()
		{
		}

		public object Clone()
		{
			return (RPLTextBoxProps)MemberwiseClone();
		}
	}
}
