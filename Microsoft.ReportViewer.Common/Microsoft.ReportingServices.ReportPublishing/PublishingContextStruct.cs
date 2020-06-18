using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal struct PublishingContextStruct
	{
		private LocationFlags m_location;

		private ObjectType m_objectType;

		private string m_objectName;

		private string m_prefixPropertyName;

		private int m_maxExpressionLength;

		private PublishingErrorContext m_errorContext;

		internal LocationFlags Location
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		internal ObjectType ObjectType
		{
			get
			{
				return m_objectType;
			}
			set
			{
				m_objectType = value;
			}
		}

		internal string ObjectName
		{
			get
			{
				return m_objectName;
			}
			set
			{
				m_objectName = value;
			}
		}

		internal string PrefixPropertyName
		{
			get
			{
				return m_prefixPropertyName;
			}
			set
			{
				m_prefixPropertyName = value;
			}
		}

		internal PublishingErrorContext ErrorContext
		{
			get
			{
				return m_errorContext;
			}
			set
			{
				m_errorContext = value;
			}
		}

		internal PublishingContextStruct(LocationFlags location, ObjectType objectType, int maxExpressionLength, PublishingErrorContext errorContext)
		{
			m_location = location;
			m_objectType = objectType;
			m_objectName = null;
			m_prefixPropertyName = null;
			m_maxExpressionLength = maxExpressionLength;
			m_errorContext = errorContext;
		}

		internal Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext CreateExpressionContext(Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionType expressionType, DataType constantType, string propertyName, string dataSetName, PublishingContextBase publishingContext)
		{
			return new Microsoft.ReportingServices.RdlExpressions.ExpressionParser.ExpressionContext(propertyName: string.IsNullOrEmpty(propertyName) ? m_prefixPropertyName : ((!string.IsNullOrEmpty(m_prefixPropertyName)) ? (m_prefixPropertyName + propertyName) : propertyName), expressionType: expressionType, constantType: constantType, location: m_location, objectType: m_objectType, objectName: m_objectName, dataSetName: dataSetName, maxExpressionLength: m_maxExpressionLength, publishingContext: publishingContext);
		}

		internal double ValidateSize(string size, string propertyName, ErrorContext errorContext)
		{
			PublishingValidator.ValidateSize(size, m_objectType, m_objectName, propertyName, restrictMaxValue: true, errorContext, out double sizeInMM, out string _);
			return sizeInMM;
		}
	}
}
