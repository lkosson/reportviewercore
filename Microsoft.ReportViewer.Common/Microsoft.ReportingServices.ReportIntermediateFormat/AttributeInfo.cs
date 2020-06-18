using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class AttributeInfo : IPersistable
	{
		private bool m_isExpression;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

		private double m_floatValue;

		private ValueType m_valueType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool IsExpression
		{
			get
			{
				return m_isExpression;
			}
			set
			{
				m_isExpression = value;
			}
		}

		internal string Value
		{
			get
			{
				return m_stringValue;
			}
			set
			{
				m_stringValue = value;
			}
		}

		internal bool BoolValue
		{
			get
			{
				return m_boolValue;
			}
			set
			{
				m_boolValue = value;
			}
		}

		internal int IntValue
		{
			get
			{
				return m_intValue;
			}
			set
			{
				m_intValue = value;
			}
		}

		internal double FloatValue
		{
			get
			{
				return m_floatValue;
			}
			set
			{
				m_floatValue = value;
			}
		}

		internal ValueType ValueType
		{
			get
			{
				return m_valueType;
			}
			set
			{
				m_valueType = value;
			}
		}

		internal AttributeInfo PublishClone(AutomaticSubtotalContext context)
		{
			AttributeInfo attributeInfo = (AttributeInfo)MemberwiseClone();
			if (m_stringValue != null)
			{
				attributeInfo.m_stringValue = (string)m_stringValue.Clone();
			}
			return attributeInfo;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IsExpression, Token.Boolean));
			list.Add(new MemberInfo(MemberName.StringValue, Token.String));
			list.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			list.Add(new MemberInfo(MemberName.FloatValue, Token.Double, Lifetime.AddedIn(200)));
			list.Add(new MemberInfo(MemberName.ValueType, Token.Enum, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsExpression:
					writer.Write(m_isExpression);
					break;
				case MemberName.StringValue:
					writer.Write(m_stringValue);
					break;
				case MemberName.BoolValue:
					writer.Write(m_boolValue);
					break;
				case MemberName.IntValue:
					writer.Write(m_intValue);
					break;
				case MemberName.FloatValue:
					writer.Write(m_floatValue);
					break;
				case MemberName.ValueType:
					writer.WriteEnum((int)m_valueType);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsExpression:
					m_isExpression = reader.ReadBoolean();
					break;
				case MemberName.StringValue:
					m_stringValue = reader.ReadString();
					break;
				case MemberName.BoolValue:
					m_boolValue = reader.ReadBoolean();
					break;
				case MemberName.IntValue:
					m_intValue = reader.ReadInt32();
					break;
				case MemberName.FloatValue:
					m_floatValue = reader.ReadDouble();
					break;
				case MemberName.ValueType:
					m_valueType = (ValueType)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo;
		}
	}
}
