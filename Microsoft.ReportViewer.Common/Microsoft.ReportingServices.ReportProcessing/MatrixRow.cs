using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixRow
	{
		private string m_height;

		private double m_heightValue;

		[NonSerialized]
		private int m_numberOfMatrixCells;

		internal string Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return m_heightValue;
			}
			set
			{
				m_heightValue = value;
			}
		}

		internal int NumberOfMatrixCells
		{
			get
			{
				return m_numberOfMatrixCells;
			}
			set
			{
				m_numberOfMatrixCells = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			m_heightValue = context.ValidateSize(ref m_height, "Height");
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Height, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
