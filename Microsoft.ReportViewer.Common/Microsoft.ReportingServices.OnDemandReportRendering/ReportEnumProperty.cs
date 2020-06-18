namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportEnumProperty<EnumType> : ReportProperty where EnumType : struct
	{
		private EnumType m_value;

		public EnumType Value => m_value;

		internal ReportEnumProperty()
		{
			m_value = default(EnumType);
		}

		internal ReportEnumProperty(EnumType value)
		{
			m_value = value;
		}

		internal ReportEnumProperty(bool isExpression, string expressionString, EnumType value)
			: this(isExpression, expressionString, value, default(EnumType))
		{
		}

		internal ReportEnumProperty(bool isExpression, string expressionString, EnumType value, EnumType defaultValue)
			: base(isExpression, expressionString)
		{
			if (!isExpression)
			{
				m_value = value;
			}
			else
			{
				m_value = defaultValue;
			}
		}
	}
}
