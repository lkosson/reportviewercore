using System;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextBoxPropsDef : RPLItemPropsDef
	{
		private bool m_isSimple = true;

		private string m_formula;

		private bool m_isToggleParent;

		private bool m_canGrow;

		private bool m_canShrink;

		private bool m_canSort;

		private TypeCode m_typeCode = TypeCode.String;

		private bool m_formattedValueExpressionBased;

		private string m_value;

		public bool CanSort
		{
			get
			{
				return m_canSort;
			}
			set
			{
				m_canSort = value;
			}
		}

		public bool CanShrink
		{
			get
			{
				return m_canShrink;
			}
			set
			{
				m_canShrink = value;
			}
		}

		public bool CanGrow
		{
			get
			{
				return m_canGrow;
			}
			set
			{
				m_canGrow = value;
			}
		}

		public string Formula
		{
			get
			{
				return m_formula;
			}
			set
			{
				m_formula = value;
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

		public TypeCode SharedTypeCode
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

		public bool FormattedValueExpressionBased
		{
			get
			{
				return m_formattedValueExpressionBased;
			}
			set
			{
				m_formattedValueExpressionBased = value;
			}
		}

		public bool IsSimple
		{
			get
			{
				return m_isSimple;
			}
			set
			{
				m_isSimple = value;
			}
		}

		internal RPLTextBoxPropsDef()
		{
		}
	}
}
