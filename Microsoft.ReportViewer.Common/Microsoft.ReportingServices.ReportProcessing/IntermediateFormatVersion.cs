using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class IntermediateFormatVersion
	{
		private int m_major;

		private int m_minor;

		private int m_build;

		private static readonly int m_current_major;

		private static readonly int m_current_minor;

		private static readonly int m_current_build;

		internal int Major
		{
			get
			{
				return m_major;
			}
			set
			{
				m_major = value;
			}
		}

		internal int Minor
		{
			get
			{
				return m_minor;
			}
			set
			{
				m_minor = value;
			}
		}

		internal int Build
		{
			get
			{
				return m_build;
			}
			set
			{
				m_build = value;
			}
		}

		internal bool IsOldVersion
		{
			get
			{
				if (CompareTo(m_current_major, m_current_minor, m_current_build) < 0)
				{
					return true;
				}
				return false;
			}
		}

		internal bool IsRS2000_Beta2_orOlder => CompareTo(8, 0, 673) <= 0;

		internal bool IsRS2000_WithSpecialRecursiveAggregates => CompareTo(8, 0, 700) >= 0;

		internal bool IsRS2000_WithNewChartYAxis => CompareTo(8, 0, 713) >= 0;

		internal bool IsRS2000_WithOtherPageChunkSplit => CompareTo(8, 0, 716) >= 0;

		internal bool IsRS2000_RTM_orOlder => CompareTo(8, 0, 743) <= 0;

		internal bool IsRS2000_RTM_orNewer => CompareTo(8, 0, 743) >= 0;

		internal bool IsRS2000_WithUnusedFieldsOptimization => CompareTo(8, 0, 801) >= 0;

		internal bool IsRS2000_WithImageInfo => CompareTo(8, 0, 843) >= 0;

		internal bool IsRS2005_Beta2_orOlder => CompareTo(9, 0, 852) <= 0;

		internal bool IsRS2005_WithMultipleActions => CompareTo(9, 0, 937) >= 0;

		internal bool IsRS2005_WithSpecialChunkSplit => CompareTo(9, 0, 937) >= 0;

		internal bool IsRS2005_IDW9_orOlder => CompareTo(9, 0, 951) <= 0;

		internal bool IsRS2005_WithTableDetailFix => CompareTo(10, 2, 0) >= 0;

		internal bool IsRS2005_WithPHFChunks => CompareTo(10, 3, 0) >= 0;

		internal bool IsRS2005_WithTableOptimizations => CompareTo(10, 4, 0) >= 0;

		internal bool IsRS2005_WithSharedDrillthroughParams => CompareTo(10, 8, 0) >= 0;

		internal bool IsRS2005_WithSimpleTextBoxOptimizations => CompareTo(10, 5, 0) >= 0;

		internal bool IsRS2005_WithChartHeadingInstanceFix => CompareTo(10, 6, 0) >= 0;

		internal bool IsRS2005_WithXmlDataElementOutputChange => CompareTo(10, 7, 0) >= 0;

		internal bool Is_WithUserSort => CompareTo(9, 0, 970) >= 0;

		internal IntermediateFormatVersion()
		{
			SetCurrent();
		}

		internal IntermediateFormatVersion(int major, int minor, int build)
		{
			m_major = major;
			m_minor = minor;
			m_build = build;
		}

		static IntermediateFormatVersion()
		{
			m_current_major = 10;
			m_current_minor = 8;
			int current_build = 0;
			RevertImpersonationContext.Run(delegate
			{
				string pathForVersion = typeof(IntermediateFormatVersion).Assembly.Location;
				if (String.IsNullOrEmpty(pathForVersion)) pathForVersion = System.Reflection.Assembly.GetExecutingAssembly().Location;
				if (String.IsNullOrEmpty(pathForVersion)) pathForVersion = Process.GetCurrentProcess().MainModule.FileName;
				current_build = EncodeFileVersion(FileVersionInfo.GetVersionInfo(pathForVersion));
			});
			m_current_build = current_build;
		}

		private static int EncodeFileVersion(FileVersionInfo fileVersion)
		{
			return ((fileVersion.FileMajorPart % 20 * 10 + fileVersion.FileMinorPart % 10) * 100000 + fileVersion.FileBuildPart % 100000) * 100 + fileVersion.FilePrivatePart % 100;
		}

		internal static void DecodeFileVersion(int version, out int major, out int minor, out int build, out int buildminor)
		{
			major = 0;
			minor = 0;
			build = 0;
			buildminor = 0;
			if (version > 0)
			{
				buildminor = version % 100;
				version -= buildminor;
				version /= 100;
				build = version % 100000;
				version -= build;
				version /= 100000;
				minor = version % 10;
				version -= minor;
				version /= 10;
				major = version % 20;
			}
		}

		internal void SetCurrent()
		{
			m_major = m_current_major;
			m_minor = m_current_minor;
			m_build = m_current_build;
		}

		private int CompareTo(int major, int minor, int build)
		{
			int num = Compare(m_major, major);
			if (num == 0)
			{
				num = Compare(m_minor, minor);
				if (num == 0 && m_major < 10)
				{
					num = Compare(m_build, build);
				}
			}
			return num;
		}

		private int Compare(int x, int y)
		{
			if (x < y)
			{
				return -1;
			}
			if (x > y)
			{
				return 1;
			}
			return 0;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersionMajor, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersionMinor, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.IntermediateFormatVersionBuild, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntermediateFormatVersion, memberInfoList);
		}

		public override string ToString()
		{
			if (m_major < 10)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", m_major, m_minor, m_build);
			}
			DecodeFileVersion(m_build, out int _, out int _, out int build, out int buildminor);
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", m_major, m_minor, build, buildminor);
		}
	}
}
