using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableColumn
	{
		private string m_width;

		private double m_widthValue;

		private Visibility m_visibility;

		private bool m_fixedHeader;

		[NonSerialized]
		private ReportSize m_widthForRendering;

		internal string Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal double WidthValue
		{
			get
			{
				return m_widthValue;
			}
			set
			{
				m_widthValue = value;
			}
		}

		internal Visibility Visibility
		{
			get
			{
				return m_visibility;
			}
			set
			{
				m_visibility = value;
			}
		}

		internal ReportSize WidthForRendering
		{
			get
			{
				return m_widthForRendering;
			}
			set
			{
				m_widthForRendering = value;
			}
		}

		internal bool FixedHeader
		{
			get
			{
				return m_fixedHeader;
			}
			set
			{
				m_fixedHeader = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			m_widthValue = context.ValidateSize(ref m_width, "Width");
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: true);
			}
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: false);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Width, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.FixedHeader, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
