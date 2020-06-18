using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Text;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportParameter
	{
		private ParameterInfo m_underlyingParam;

		public string Name => m_underlyingParam.Name;

		public TypeCode DataType => (TypeCode)m_underlyingParam.DataType;

		public bool Nullable => m_underlyingParam.Nullable;

		public bool MultiValue => m_underlyingParam.MultiValue;

		public bool AllowBlank => m_underlyingParam.AllowBlank;

		public string Prompt => m_underlyingParam.Prompt;

		public bool UsedInQuery => m_underlyingParam.UsedInQuery;

		public object Value
		{
			get
			{
				if (m_underlyingParam.Values == null || m_underlyingParam.Values.Length == 0)
				{
					return null;
				}
				return m_underlyingParam.Values[0];
			}
		}

		public object[] Values
		{
			get
			{
				if (m_underlyingParam.Values == null || m_underlyingParam.Values.Length == 0)
				{
					return null;
				}
				return m_underlyingParam.Values;
			}
		}

		internal string StringValues
		{
			get
			{
				if (m_underlyingParam.Values == null || m_underlyingParam.Values.Length == 0)
				{
					return null;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < m_underlyingParam.Values.Length; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(m_underlyingParam.CastToString(m_underlyingParam.Values[i], Localization.ClientPrimaryCulture));
				}
				return stringBuilder.ToString();
			}
		}

		internal ParameterInfo UnderlyingParam => m_underlyingParam;

		internal ReportParameter(ParameterInfo param)
		{
			m_underlyingParam = param;
		}
	}
}
