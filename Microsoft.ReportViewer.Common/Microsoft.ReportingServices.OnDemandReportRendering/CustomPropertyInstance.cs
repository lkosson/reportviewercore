using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomPropertyInstance
	{
		private CustomProperty m_customPropertyDef;

		private string m_name;

		private object m_value;

		private TypeCode m_typeCode;

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				ReportElement reportElementOwner = m_customPropertyDef.ReportElementOwner;
				if (reportElementOwner == null || reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_customPropertyDef.Name.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_name = value;
			}
		}

		internal TypeCode TypeCode => m_typeCode;

		public object Value
		{
			get
			{
				return m_value;
			}
			set
			{
				ReportElement reportElementOwner = m_customPropertyDef.ReportElementOwner;
				if (reportElementOwner == null || reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !m_customPropertyDef.Value.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				if (value != null)
				{
					if (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)
					{
						if (!(value is string))
						{
							throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackStringExpected);
						}
					}
					else if (!ReportRuntime.IsVariant(value))
					{
						throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
					}
				}
				m_value = value;
			}
		}

		internal CustomPropertyInstance(CustomProperty customPropertyDef, string name, object value, TypeCode typeCode)
		{
			m_customPropertyDef = customPropertyDef;
			m_name = name;
			m_value = value;
			m_typeCode = typeCode;
		}

		internal void Update(string name, object value, TypeCode typeCode)
		{
			m_name = name;
			m_value = value;
			m_typeCode = typeCode;
		}
	}
}
