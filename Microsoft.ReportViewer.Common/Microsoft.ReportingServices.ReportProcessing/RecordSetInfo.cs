using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetInfo
	{
		private bool m_readerExtensionsSupported;

		private RecordSetPropertyNamesList m_fieldPropertyNames;

		private CompareOptions m_compareOptions;

		[NonSerialized]
		private bool m_validCompareOptions;

		internal bool ReaderExtensionsSupported
		{
			get
			{
				return m_readerExtensionsSupported;
			}
			set
			{
				m_readerExtensionsSupported = value;
			}
		}

		internal RecordSetPropertyNamesList FieldPropertyNames
		{
			get
			{
				return m_fieldPropertyNames;
			}
			set
			{
				m_fieldPropertyNames = value;
			}
		}

		internal CompareOptions CompareOptions
		{
			get
			{
				return m_compareOptions;
			}
			set
			{
				m_compareOptions = value;
			}
		}

		internal bool ValidCompareOptions
		{
			get
			{
				return m_validCompareOptions;
			}
			set
			{
				m_validCompareOptions = value;
			}
		}

		internal RecordSetInfo(bool readerExtensionsSupported, CompareOptions compareOptions)
		{
			m_readerExtensionsSupported = readerExtensionsSupported;
			m_compareOptions = compareOptions;
		}

		internal RecordSetInfo()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReaderExtensionsSupported, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FieldPropertyNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RecordSetPropertyNamesList));
			memberInfoList.Add(new MemberInfo(MemberName.CompareOptions, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
