using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetPropertyNames
	{
		private StringList m_propertyNames;

		internal StringList PropertyNames
		{
			get
			{
				return m_propertyNames;
			}
			set
			{
				m_propertyNames = value;
			}
		}

		internal RecordSetPropertyNames()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PropertyNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
