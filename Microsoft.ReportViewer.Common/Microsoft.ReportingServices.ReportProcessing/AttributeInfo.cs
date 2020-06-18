using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class AttributeInfo
	{
		private bool m_isExpression;

		private string m_stringValue;

		private bool m_boolValue;

		private int m_intValue;

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

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.IsExpression, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StringValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.BoolValue, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IntValue, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
