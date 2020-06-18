using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataValueCRIList : DataValueList
	{
		private int m_rdlRowIndex = -1;

		private int m_rdlColumnIndex = -1;

		internal int RDLRowIndex
		{
			get
			{
				return m_rdlRowIndex;
			}
			set
			{
				m_rdlRowIndex = value;
			}
		}

		internal int RDLColumnIndex
		{
			get
			{
				return m_rdlColumnIndex;
			}
			set
			{
				m_rdlColumnIndex = value;
			}
		}

		internal DataValueCRIList()
		{
		}

		internal DataValueCRIList(int capacity)
			: base(capacity)
		{
		}

		internal new DataValueCRIList DeepClone(InitializationContext context)
		{
			int count = Count;
			DataValueCRIList dataValueCRIList = new DataValueCRIList(count);
			dataValueCRIList.RDLColumnIndex = m_rdlColumnIndex;
			dataValueCRIList.RDLRowIndex = m_rdlRowIndex;
			for (int i = 0; i < count; i++)
			{
				dataValueCRIList.Add(base[i].DeepClone(context));
			}
			return dataValueCRIList;
		}

		internal void Initialize(string prefix, InitializationContext context)
		{
			Initialize(prefix, m_rdlRowIndex, m_rdlColumnIndex, isCustomProperty: false, context);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.RDLRowIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.RDLColumnIndex, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValue, memberInfoList);
		}
	}
}
