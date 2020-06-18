using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ThreeDProperties
	{
		internal enum ShadingTypes
		{
			None,
			Simple,
			Real
		}

		private bool m_enabled;

		private bool m_perspectiveProjectionMode = true;

		private int m_rotation;

		private int m_inclination;

		private int m_perspective;

		private int m_heightRatio;

		private int m_depthRatio;

		private ShadingTypes m_shading;

		private int m_gapDepth;

		private int m_wallThickness;

		private bool m_drawingStyleCube = true;

		private bool m_clustered;

		internal bool Enabled
		{
			get
			{
				return m_enabled;
			}
			set
			{
				m_enabled = value;
			}
		}

		internal bool PerspectiveProjectionMode
		{
			get
			{
				return m_perspectiveProjectionMode;
			}
			set
			{
				m_perspectiveProjectionMode = value;
			}
		}

		internal int Rotation
		{
			get
			{
				return m_rotation;
			}
			set
			{
				m_rotation = value;
			}
		}

		internal int Inclination
		{
			get
			{
				return m_inclination;
			}
			set
			{
				m_inclination = value;
			}
		}

		internal int Perspective
		{
			get
			{
				return m_perspective;
			}
			set
			{
				m_perspective = value;
			}
		}

		internal int HeightRatio
		{
			get
			{
				return m_heightRatio;
			}
			set
			{
				m_heightRatio = value;
			}
		}

		internal int DepthRatio
		{
			get
			{
				return m_depthRatio;
			}
			set
			{
				m_depthRatio = value;
			}
		}

		internal ShadingTypes Shading
		{
			get
			{
				return m_shading;
			}
			set
			{
				m_shading = value;
			}
		}

		internal int GapDepth
		{
			get
			{
				return m_gapDepth;
			}
			set
			{
				m_gapDepth = value;
			}
		}

		internal int WallThickness
		{
			get
			{
				return m_wallThickness;
			}
			set
			{
				m_wallThickness = value;
			}
		}

		internal bool DrawingStyleCube
		{
			get
			{
				return m_drawingStyleCube;
			}
			set
			{
				m_drawingStyleCube = value;
			}
		}

		internal bool Clustered
		{
			get
			{
				return m_clustered;
			}
			set
			{
				m_clustered = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Enabled, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PerspectiveProjectionMode, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Rotation, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Inclination, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Perspective, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeightRatio, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DepthRatio, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Shading, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.GapDepth, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.WallThickness, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DrawingStyleCube, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Clustered, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
