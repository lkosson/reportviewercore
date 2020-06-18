using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataValueInstance
	{
		private string m_name;

		private object m_value;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal object Value
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

		internal DataValueInstance DeepClone()
		{
			DataValueInstance dataValueInstance = new DataValueInstance();
			if (m_name != null)
			{
				dataValueInstance.Name = string.Copy(m_name);
			}
			if (m_value != null)
			{
				CustomReportItem.CloneObject(m_value, out object clone);
				dataValueInstance.Value = clone;
			}
			return dataValueInstance;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Value, Token.Object));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
