using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	[Serializable]
	internal class IntermediateFormatVersion : IPersistable
	{
		private int m_major;

		private int m_minor;

		private int m_build;

		[NonSerialized]
		private static readonly Declaration m_Declaration;

		[NonSerialized]
		private static readonly IntermediateFormatVersion m_current;

		[NonSerialized]
		private static IntermediateFormatVersion m_rtm2008;

		[NonSerialized]
		private static IntermediateFormatVersion m_biRefresh;

		[NonSerialized]
		private static IntermediateFormatVersion m_sql16;

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
				if (CompareTo(m_current.Major, m_current.Minor, m_current.Build) < 0)
				{
					return true;
				}
				return false;
			}
		}

		internal static IntermediateFormatVersion Current => m_current;

		internal static IntermediateFormatVersion SQL16
		{
			get
			{
				if (m_sql16 == null)
				{
					m_sql16 = new IntermediateFormatVersion(12, 3, 0);
				}
				return m_sql16;
			}
		}

		internal static IntermediateFormatVersion RTM2008
		{
			get
			{
				if (m_rtm2008 == null)
				{
					m_rtm2008 = new IntermediateFormatVersion(11, 2, 0);
				}
				return m_rtm2008;
			}
		}

		internal static IntermediateFormatVersion BIRefresh
		{
			get
			{
				if (m_biRefresh == null)
				{
					m_biRefresh = new IntermediateFormatVersion(12, 1, 0);
				}
				return m_biRefresh;
			}
		}

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
			m_Declaration = GetDeclaration();
			int majorVersion = PersistenceConstants.MajorVersion;
			int minorVersion = PersistenceConstants.MinorVersion;
			int current_build = 0;
			RevertImpersonationContext.Run(delegate
			{
				string pathForVersion = typeof(IntermediateFormatVersion).Assembly.Location;
				if (String.IsNullOrEmpty(pathForVersion)) pathForVersion = System.Reflection.Assembly.GetExecutingAssembly().Location;
				if (String.IsNullOrEmpty(pathForVersion)) pathForVersion = Process.GetCurrentProcess().MainModule.FileName;
				current_build = EncodeFileVersion(FileVersionInfo.GetVersionInfo(pathForVersion));
			});
			m_current = new IntermediateFormatVersion(majorVersion, minorVersion, current_build);
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
			m_major = m_current.Major;
			m_minor = m_current.Minor;
			m_build = m_current.Build;
		}

		internal int CompareTo(int major, int minor, int build)
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

		internal int CompareTo(IntermediateFormatVersion version)
		{
			int num = Compare(m_major, version.Major);
			if (num == 0)
			{
				num = Compare(m_minor, version.Minor);
				if (num == 0 && m_major < 10)
				{
					num = Compare(m_build, version.Build);
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

		public override string ToString()
		{
			if (m_major < 10)
			{
				return m_major.ToString(CultureInfo.InvariantCulture) + "." + m_minor.ToString(CultureInfo.InvariantCulture) + "." + m_build.ToString(CultureInfo.InvariantCulture);
			}
			DecodeFileVersion(m_build, out int _, out int _, out int build, out int buildminor);
			return m_major.ToString(CultureInfo.InvariantCulture) + "." + m_minor.ToString(CultureInfo.InvariantCulture) + "." + build.ToString(CultureInfo.InvariantCulture) + "." + buildminor.ToString(CultureInfo.InvariantCulture);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IntermediateFormatVersionMajor, Token.Int32));
			list.Add(new MemberInfo(MemberName.IntermediateFormatVersionMinor, Token.Int32));
			list.Add(new MemberInfo(MemberName.IntermediateFormatVersionBuild, Token.Int32));
			return new Declaration(ObjectType.IntermediateFormatVersion, ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IntermediateFormatVersionMajor:
					writer.Write(m_major);
					break;
				case MemberName.IntermediateFormatVersionMinor:
					writer.Write(m_minor);
					break;
				case MemberName.IntermediateFormatVersionBuild:
					writer.Write(m_build);
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
				case MemberName.IntermediateFormatVersionMajor:
					m_major = reader.ReadInt32();
					break;
				case MemberName.IntermediateFormatVersionMinor:
					m_minor = reader.ReadInt32();
					break;
				case MemberName.IntermediateFormatVersionBuild:
					m_build = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.IntermediateFormatVersion;
		}
	}
}
