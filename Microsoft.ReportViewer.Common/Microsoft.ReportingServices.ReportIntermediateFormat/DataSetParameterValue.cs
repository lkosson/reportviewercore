using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class DataSetParameterValue : ParameterValue, IParameterDef
	{
		private bool m_readOnly;

		private bool m_nullable;

		private bool m_omitFromQuery;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool ReadOnly
		{
			get
			{
				return m_readOnly;
			}
			set
			{
				m_readOnly = value;
			}
		}

		internal bool Nullable
		{
			get
			{
				return m_nullable;
			}
			set
			{
				m_nullable = value;
			}
		}

		internal bool OmitFromQuery
		{
			get
			{
				return m_omitFromQuery;
			}
			set
			{
				m_omitFromQuery = value;
			}
		}

		string IParameterDef.Name => base.Name;

		Microsoft.ReportingServices.ReportProcessing.ObjectType IParameterDef.ParameterObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter;

		DataType IParameterDef.DataType => DataType.Object;

		public bool MultiValue => true;

		public int DefaultValuesExpressionCount
		{
			get
			{
				if (base.Value == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public int ValidValuesValueExpressionCount => 0;

		public int ValidValuesLabelExpressionCount => 0;

		public IParameterDataSource DefaultDataSource
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public IParameterDataSource ValidValuesDataSource
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ReadOnly, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Nullable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.OmitFromQuery, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetParameterValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue, list);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSetParameterValue;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReadOnly:
					writer.Write(m_readOnly);
					break;
				case MemberName.Nullable:
					writer.Write(m_nullable);
					break;
				case MemberName.OmitFromQuery:
					writer.Write(m_omitFromQuery);
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReadOnly:
					m_readOnly = reader.ReadBoolean();
					break;
				case MemberName.Nullable:
					m_nullable = reader.ReadBoolean();
					break;
				case MemberName.OmitFromQuery:
					m_omitFromQuery = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public bool ValidateValueForNull(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return ParameterBase.ValidateValueForNull(newValue, Nullable, errorContext, Microsoft.ReportingServices.ReportProcessing.ObjectType.QueryParameter, base.Name, parameterValueProperty);
		}

		public bool ValidateValueForBlank(object newValue, ErrorContext errorContext, string parameterValueProperty)
		{
			return true;
		}

		public bool HasDefaultValuesExpressions()
		{
			if (base.Value != null)
			{
				return base.Value.IsExpression;
			}
			return false;
		}

		public bool HasDefaultValuesDataSource()
		{
			return false;
		}

		public bool HasValidValuesValueExpressions()
		{
			return false;
		}

		public bool HasValidValuesLabelExpressions()
		{
			return false;
		}

		public bool HasValidValuesDataSource()
		{
			return false;
		}
	}
}
