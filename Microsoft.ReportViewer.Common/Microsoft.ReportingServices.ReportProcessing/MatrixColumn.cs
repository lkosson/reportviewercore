using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixColumn
	{
		private string m_width;

		private double m_widthValue;

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

		internal void Initialize(InitializationContext context)
		{
			m_widthValue = context.ValidateSize(ref m_width, "Width");
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Width, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
