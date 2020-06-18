using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ProcessingIntermediateFormatVersion
	{
		private IntermediateFormatVersion m_version;

		internal int Major
		{
			get
			{
				return m_version.Major;
			}
			set
			{
				m_version.Major = value;
			}
		}

		internal int Minor
		{
			get
			{
				return m_version.Minor;
			}
			set
			{
				m_version.Minor = value;
			}
		}

		internal int Build
		{
			get
			{
				return m_version.Build;
			}
			set
			{
				m_version.Build = value;
			}
		}

		internal bool IsOldVersion => m_version.IsOldVersion;

		internal bool IsRIF11_orOlder => m_version.CompareTo(11, 0, 0) <= 0;

		internal bool IsRIF11_orNewer => m_version.CompareTo(11, 0, 0) >= 0;

		internal bool IsRS2000_Beta2_orOlder => m_version.CompareTo(8, 0, 673) <= 0;

		internal bool IsRS2000_WithSpecialRecursiveAggregates => m_version.CompareTo(8, 0, 700) >= 0;

		internal bool IsRS2000_WithNewChartYAxis => m_version.CompareTo(8, 0, 713) >= 0;

		internal bool IsRS2000_WithOtherPageChunkSplit => m_version.CompareTo(8, 0, 716) >= 0;

		internal bool IsRS2000_RTM_orOlder => m_version.CompareTo(8, 0, 743) <= 0;

		internal bool IsRS2000_RTM_orNewer => m_version.CompareTo(8, 0, 743) >= 0;

		internal bool IsRS2000_WithUnusedFieldsOptimization => m_version.CompareTo(8, 0, 801) >= 0;

		internal bool IsRS2000_WithImageInfo => m_version.CompareTo(8, 0, 843) >= 0;

		internal bool IsRS2005_Beta2_orOlder => m_version.CompareTo(9, 0, 852) <= 0;

		internal bool IsRS2005_WithMultipleActions => m_version.CompareTo(9, 0, 937) >= 0;

		internal bool IsRS2005_WithSpecialChunkSplit => m_version.CompareTo(9, 0, 937) >= 0;

		internal bool IsRS2005_IDW9_orOlder => m_version.CompareTo(9, 0, 951) <= 0;

		internal bool IsRS2005_WithTableDetailFix => m_version.CompareTo(10, 2, 0) >= 0;

		internal bool IsRS2005_WithPHFChunks => m_version.CompareTo(10, 3, 0) >= 0;

		internal bool IsRS2005_WithTableOptimizations => m_version.CompareTo(10, 4, 0) >= 0;

		internal bool IsRS2005_WithSharedDrillthroughParams => m_version.CompareTo(10, 8, 0) >= 0;

		internal bool IsRS2005_WithSimpleTextBoxOptimizations => m_version.CompareTo(10, 5, 0) >= 0;

		internal bool IsRS2005_WithChartHeadingInstanceFix => m_version.CompareTo(10, 6, 0) >= 0;

		internal bool IsRS2005_WithXmlDataElementOutputChange => m_version.CompareTo(10, 7, 0) >= 0;

		internal bool Is_WithUserSort => m_version.CompareTo(9, 0, 970) >= 0;

		internal ProcessingIntermediateFormatVersion(IntermediateFormatVersion version)
		{
			m_version = version;
		}

		public override string ToString()
		{
			return m_version.ToString();
		}
	}
}
