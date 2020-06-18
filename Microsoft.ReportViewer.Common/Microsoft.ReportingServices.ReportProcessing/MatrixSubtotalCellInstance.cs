using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixSubtotalCellInstance : MatrixCellInstance
	{
		[Reference]
		private MatrixHeadingInstance m_subtotalHeadingInstance;

		internal MatrixHeadingInstance SubtotalHeadingInstance
		{
			get
			{
				return m_subtotalHeadingInstance;
			}
			set
			{
				m_subtotalHeadingInstance = value;
			}
		}

		internal MatrixSubtotalCellInstance(int rowIndex, int colIndex, Matrix matrixDef, int cellDefIndex, ReportProcessing.ProcessingContext pc, out NonComputedUniqueNames nonComputedUniqueNames)
			: base(rowIndex, colIndex, matrixDef, cellDefIndex, pc, out nonComputedUniqueNames)
		{
			Global.Tracer.Assert(pc.HeadingInstance != null);
			Global.Tracer.Assert(pc.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
			m_subtotalHeadingInstance = pc.HeadingInstance;
		}

		internal MatrixSubtotalCellInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.SubtotalHeadingInstance, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstance));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
