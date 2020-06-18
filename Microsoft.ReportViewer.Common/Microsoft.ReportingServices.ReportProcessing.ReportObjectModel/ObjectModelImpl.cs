using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class ObjectModelImpl : ObjectModel, IConvertible
	{
		private FieldsImpl m_fields;

		private ParametersImpl m_parameters;

		private GlobalsImpl m_globals;

		private UserImpl m_user;

		private ReportItemsImpl m_reportItems;

		private AggregatesImpl m_aggregates;

		private DataSetsImpl m_dataSets;

		private DataSourcesImpl m_dataSources;

		private ReportProcessing.ProcessingContext m_processingContext;

		internal const string NamespacePrefix = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.";

		public override Fields Fields => FieldsImpl;

		public override Parameters Parameters => ParametersImpl;

		public override Globals Globals => GlobalsImpl;

		public override User User => UserImpl;

		public override ReportItems ReportItems => ReportItemsImpl;

		public override Aggregates Aggregates => AggregatesImpl;

		public override DataSets DataSets => DataSetsImpl;

		public override DataSources DataSources => DataSourcesImpl;

		internal FieldsImpl FieldsImpl
		{
			get
			{
				return m_fields;
			}
			set
			{
				m_fields = value;
			}
		}

		internal ParametersImpl ParametersImpl
		{
			get
			{
				return m_parameters;
			}
			set
			{
				m_parameters = value;
			}
		}

		internal GlobalsImpl GlobalsImpl
		{
			get
			{
				return m_globals;
			}
			set
			{
				m_globals = value;
			}
		}

		internal UserImpl UserImpl
		{
			get
			{
				return m_user;
			}
			set
			{
				m_user = value;
			}
		}

		internal ReportItemsImpl ReportItemsImpl
		{
			get
			{
				return m_reportItems;
			}
			set
			{
				m_reportItems = value;
			}
		}

		internal AggregatesImpl AggregatesImpl
		{
			get
			{
				return m_aggregates;
			}
			set
			{
				m_aggregates = value;
			}
		}

		internal DataSetsImpl DataSetsImpl
		{
			get
			{
				return m_dataSets;
			}
			set
			{
				m_dataSets = value;
			}
		}

		internal DataSourcesImpl DataSourcesImpl
		{
			get
			{
				return m_dataSources;
			}
			set
			{
				m_dataSources = value;
			}
		}

		internal ObjectModelImpl(ReportProcessing.ProcessingContext processingContext)
		{
			m_fields = null;
			m_parameters = null;
			m_globals = null;
			m_user = null;
			m_reportItems = null;
			m_aggregates = null;
			m_dataSets = null;
			m_dataSources = null;
			m_processingContext = processingContext;
		}

		internal ObjectModelImpl(ObjectModelImpl copy, ReportProcessing.ProcessingContext processingContext)
		{
			m_fields = null;
			m_parameters = copy.m_parameters;
			m_globals = copy.m_globals;
			m_user = copy.m_user;
			m_reportItems = copy.m_reportItems;
			m_aggregates = copy.m_aggregates;
			m_dataSets = copy.m_dataSets;
			m_dataSources = copy.m_dataSources;
			m_processingContext = processingContext;
		}

		public override bool InScope(string scope)
		{
			return m_processingContext.ReportRuntime.InScope(scope);
		}

		public override int RecursiveLevel(string scope)
		{
			return m_processingContext.ReportRuntime.RecursiveLevel(scope);
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(ObjectModel))
			{
				return this;
			}
			throw new NotSupportedException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}
	}
}
